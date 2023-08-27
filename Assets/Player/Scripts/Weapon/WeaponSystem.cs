using Cinemachine;
using Mirror;
using System.Collections;
using TMPro;
using UnityEngine;

public class WeaponSystem : NetworkBehaviour {
    [SerializeField] GunData gunData;

    [Header("Player components")]
    [SerializeField] PlayerAnimations playerAnimations;
    [SerializeField] PlayerComponents components;
    [SerializeField] CharacterController characterController;
    [SerializeField] public PlayerData playerData;

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
        ammoUI.text = gunData.currentAmmo.ToString() + "/∞";
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
    }

    void WeaponReset() {
        if (!isLocalPlayer)
            return;

        gunData.reloading = false;
        gunData.currentAmmo = gunData.magSize;
        recoilSystem.Reset();
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
        if ( Physics.Raycast(ray.origin, direction, out RaycastHit hit, gunData.maxDistance) ) {
            Debug.DrawLine(ray.origin, hit.point, Color.red, 1);
            for ( int i = 0; i < gunData.hitboxes.Length; i++ ) {
                PlayerComponents player = hit.collider.transform.root.GetComponent<PlayerComponents>();
                if ( player && components.playerTeam != player.playerTeam && player.playerHealth > 0 ) {
                    if ( hit.collider.transform.root != gameObject.transform && hit.collider.CompareTag(gunData.hitboxes[ i ]) ) {
                        print(hit.collider.tag + " : " + gunData.damages[ i ]);
                        IDamageable damageable = hit.collider.transform.root.GetComponent<IDamageable>();
                        damageable?.CmdDamage(gunData.damage + gunData.damages[ i ], playerData.name, player.playerName, "killed");
                    }
                }
                else {
                    GameObject obj = Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal));
                    obj.transform.parent = hit.transform;
                }
            }
        }
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