using Cinemachine;
using Mirror;
using System.Collections;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class WeaponSystem : NetworkBehaviour {
    [SerializeField] GetCurrentWeapon weapon;
    [SerializeField] GunData gunData;
    [SerializeField] Animator animator;

    [Header("Player components")]
    [SerializeField] PlayerAnimations playerAnimations;
    [SerializeField] PlayerComponents components;
    [SerializeField] CharacterController characterController;
    [SerializeField] public PlayerData playerData;
    [SerializeField] AudioSource playerAudioSource;

    [Header("Recoil components")]
    [SerializeField] CinemachineImpulseSource cameraShake;
    [SerializeField] Sway sway;

    [Header("Weapon components")]
    [SerializeField] RecoilSystem recoilSystem;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject bulletImpact;
    [SerializeField] GameObject soundEffect;
    [SerializeField] Transform bocal;


    [SerializeField] TextMeshProUGUI ammoUI;
    [SerializeField] Transform canvas;
    [SerializeField] GameObject hitMarker;

    [HideInInspector] public Vector2 cameraAxis;

    float timeSinceLastShot;

    void Start() {
        WeaponSet();
        gunData.ready = true;
        gunData.reloading = false;

        PlayerMainController.shootInput += Shoot;
        PlayerMainController.reloadInput += StartReload;
        PlayerMainController.playerDied += WeaponReset;
    }

    void Update() {
        if ( !isLocalPlayer )
            return;

        timeSinceLastShot += Time.deltaTime;
    }

    void StartReload() {
        if ( !isLocalPlayer )
            return;

        if ( gunData.reloading )
            return;

        if ( gunData.currentAmmo >= gunData.magSize )
            return;

        playerAnimations?.animator.Play("Reload");
        animator?.Play("Reload");
        cameraShake.GenerateImpulse();
        StartCoroutine(Reload());
    }

    IEnumerator Reload() {
        gunData.reloading = true;
        yield return new WaitForSeconds(gunData.reloadTime);
        gunData.currentAmmo = gunData.magSize;
        gunData.reloading = false;
        UpdateUI();
    }

    bool CanShoot() => !gunData.reloading && gunData.currentAmmo > 0 && timeSinceLastShot > 1f / ( gunData.fireRate / 60.0f ) && gunData.ready;

    void Shoot() {
        if ( !CanShoot() )
            return;

        CmdShoot(Camera.main.ScreenPointToRay(Input.mousePosition));
        OnGunShot();
    }

    void OnGunShot() {
        gunData.currentAmmo--;
        timeSinceLastShot = 0.0f;
        Instantiate(muzzleFlash, bocal);
        animator?.Play("Fire");
        recoilSystem.GenerateRecoil();
        UpdateUI();
        sway.ShootSway(.55f);
    }

    void WeaponReset() {
        if (!isLocalPlayer)
            return;

        gunData.reloading = false;
        gunData.currentAmmo = gunData.magSize;
        recoilSystem.Reset();
        UpdateUI();
        sway.ShootSway(0f);
    }

    public void WeaponSet() {
        gunData = weapon.currentWeapon.GetComponent<WeaponInfo>().gunData;
        bocal = weapon.currentWeapon.GetComponent<WeaponInfo>().bocal;
        soundEffect.GetComponent<AudioSource>().clip = weapon.currentWeapon.GetComponent<WeaponInfo>().shotSound;
        animator = weapon.currentWeapon.GetComponent<Animator>();
        gunData.reloading = false;
        gunData.ready = false;
        UpdateUI();
        sway.ShootSway(0f);
    }

    void UpdateUI() {
        ammoUI.text = gunData.currentAmmo.ToString() + "/∞";
    }

    [Command(requiresAuthority = true)]
    void CmdShoot(Ray ray) {
        GameObject obj = Instantiate(soundEffect, transform.GetChild(0));
        obj.transform.parent = null;
        NetworkServer.Spawn(obj);
        RpcShoot(ray);
    }

    [ClientRpc]
    void RpcShoot(Ray ray) {
        Vector3 direction = GetSpreadDirection(ray.direction);
        playerAnimations?.animator.Play("shooting");
        int hitBoxLayer = LayerMask.GetMask("Hitboxes");
        int worldLayer = LayerMask.GetMask("World");
        if ( Physics.Raycast(ray.origin, direction, out RaycastHit hit, gunData.maxDistance, hitBoxLayer) ) {
            Debug.DrawLine(ray.origin, hit.point, Color.red, 1);
            switch( hit.collider.tag ) {
                case "Head":
                    GiveHit(0, hit);
                    break;
                case "Chest":
                    GiveHit(1, hit);
                    break;
                case "Arms":
                    GiveHit(2, hit);
                    break;
                case "Legs":
                    GiveHit(3, hit);
                    break;
            }
        }

        if ( Physics.Raycast(ray.origin, direction, out RaycastHit hitInfo, gunData.maxDistance, worldLayer) ) {
            GameObject obj = Instantiate(bulletImpact, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            obj.transform.parent = hitInfo.transform;
        }
    }

    void GiveHit(int i, RaycastHit hit) {
        PlayerComponents player = hit.collider.transform.root.GetComponent<PlayerComponents>();

        if ( hit.collider.transform.root == gameObject.transform )
            return;

        if ( player.spawning )
            return;

        if ( player.playerHealth <= 0 ) {
            components.kills++;
            return;
        }

        if ( components.playerTeam == player.playerTeam )
            return;

        Instantiate(hitMarker, canvas);
        playerAudioSource.Play();
        print(hit.collider.tag + " : " + gunData.damages[ i ]);
        IDamageable damageable = hit.collider.transform.root.GetComponent<IDamageable>();
        damageable?.CmdDamage(gunData.damage + gunData.damages[ i ], playerData.name, player.playerName, "killed");
    }

    private Vector3 GetSpreadDirection(Vector3 dir) {
        Vector3 direction = dir;

        if ( characterController.velocity.magnitude > 0 ) {
            float value = playerAnimations.isGrounded() ? 0 : gunData.spread.x + gunData.spread.y + gunData.spread.z;
            direction += new Vector3(
                Random.Range(-gunData.spread.x + value, gunData.spread.x + value),
                Random.Range(-gunData.spread.y + value, gunData.spread.y + value),
                Random.Range(-gunData.spread.z + value, gunData.spread.z + value)
                );
            direction.Normalize();
        }
        return direction;
    }
}