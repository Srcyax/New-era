using Cinemachine;
using Mirror;
using System.Collections;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class WeaponSystem : NetworkBehaviour {
    [SerializeField] GunData gunData;

    [Header("Player components")]
    [SerializeField] PlayerAnimations playerAnimations;
    [SerializeField] PlayerComponents components;
    [SerializeField] CharacterController characterController;
    [SerializeField] public PlayerData playerData;
    [SerializeField] AudioSource playerAudioSource;

    [Header("Recoil components")]
    [SerializeField] CinemachineImpulseSource cameraShake;

    [Header("Weapon components")]
    [SerializeField] RecoilSystem recoilSystem;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject bulletImpact;
    [SerializeField] GameObject soundEffect;
    [SerializeField] Transform muzzle;
    [SerializeField] Animator animator;

    [SerializeField] TextMeshProUGUI ammoUI;
    [SerializeField] Transform canvas;
    [SerializeField] GameObject hitMarker;

    [HideInInspector] public Vector2 cameraAxis;

    float timeSinceLastShot;

    void Start() {
        PlayerMainController.shootInput += Shoot;
        PlayerMainController.reloadInput += StartReload;
        PlayerMainController.playerDied += WeaponReset;

        gunData.reloading = false;
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

        playerAnimations.animator.Play("Reload");
        animator.Play("Reload");
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

    bool CanShoot() => !gunData.reloading && gunData.currentAmmo > 0 && timeSinceLastShot > 1f / ( gunData.fireRate / 60.0f );

    void Shoot() {
        if ( !CanShoot() )
            return;

        CmdShoot(Camera.main.ScreenPointToRay(Input.mousePosition));
        gunData.currentAmmo--;
        timeSinceLastShot = 0.0f;
        OnGunShot();
    }

    void OnGunShot() {
        animator.Play("Fire");
        recoilSystem.GenerateRecoil();
        UpdateUI();
    }

    void WeaponReset() {
        if (!isLocalPlayer)
            return;

        gunData.reloading = false;
        gunData.currentAmmo = gunData.magSize;
        recoilSystem.Reset();
    }

    void UpdateUI() {
        ammoUI.text = gunData.currentAmmo.ToString() + "/∞";
    }

    [Command(requiresAuthority = true)]
    void CmdShoot(Ray ray) {
        Instantiate(muzzleFlash, muzzle);
        GameObject obj = Instantiate(soundEffect, muzzle);
        obj.transform.parent = null;
        NetworkServer.Spawn(obj);
        RpcShoot(ray);
    }

    [ClientRpc]
    void RpcShoot(Ray ray) {
        Vector3 direction = GetSpreadDirection(ray.direction);
        playerAnimations.animator.Play("shooting");
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

            /*for ( int i = 0; i < gunData.hitboxes.Length; i++ ) {
                PlayerComponents player = hit.collider.transform.root.GetComponent<PlayerComponents>();
                if ( player && components.playerTeam != player.playerTeam && player.playerHealth > 0 ) {
                    if ( hit.collider.transform.root != gameObject.transform && hit.collider.CompareTag(gunData.hitboxes[ i ]) ) {
                        Instantiate(hitMarker, canvas);
                        playerAudioSource.Play();
                        print(hit.collider.tag + " : " + gunData.damages[ i ]);
                        IDamageable damageable = hit.collider.transform.root.GetComponent<IDamageable>();
                        damageable?.CmdDamage(gunData.damage + gunData.damages[ i ], playerData.name, player.playerName, "killed");
                    }
                }
                if ( !hit.collider.CompareTag(gunData.hitboxes[ i ]) ) {
                    GameObject obj = Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal));
                    obj.transform.parent = hit.transform;
                }
            }*/
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

        if ( player.playerHealth <= 0 )
            return;

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