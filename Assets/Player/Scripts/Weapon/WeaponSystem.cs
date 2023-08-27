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
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject bulletImpact;
    [SerializeField] GameObject soundEffect;
    [SerializeField] Transform muzzle;
    [SerializeField] Animator animator;

    [SerializeField] TextMeshProUGUI ammoUI;

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
        cameraShake.GenerateImpulse();
    }

    void WeaponReset() {
        gunData.reloading = false;
        gunData.currentAmmo = gunData.magSize;
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
            GameObject obj = Instantiate(bulletImpact, new Vector3(hit.point.x, hit.point.y, hit.point.z + -.04f), Quaternion.identity);
            obj.transform.rotation = Camera.main.transform.localRotation;
            obj.transform.parent = hit.transform;
            Debug.DrawLine(ray.origin, hit.point, Color.red, 1);
            for ( int i = 0; i < gunData.hitboxes.Length; i++ ) {
                if ( hit.collider.CompareTag(gunData.hitboxes[ i ]) && hit.collider.transform.root != gameObject.transform ) {
                    PlayerComponents player = hit.collider.transform.root.GetComponent<PlayerComponents>();
                    if ( components.playerTeam != player.playerTeam ) {
                        print(hit.collider.tag + " : " + gunData.damages[ i ]);
                        IDamageable damageable = hit.collider.transform.root.GetComponent<IDamageable>();
                        damageable?.CmdDamage(gunData.damage + gunData.damages[ i ], playerData.name, player.playerName);
                    }
                }
            }
        }
    }

    private Vector3 GetSpreadDirection(Vector3 dir) {
        Vector3 direction = dir;

        if ( characterController.velocity.magnitude > 0 ) {
            float value =  characterController.velocity.magnitude * .005f;
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