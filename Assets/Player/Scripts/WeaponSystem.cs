using UnityEngine;
using Mirror;
using System;
using System.Collections;

public class WeaponSystem : NetworkBehaviour
{
    [SerializeField] GunData gunData;
    [SerializeField] Transform muzzle; 
    [SerializeField] Animator animator;
    [SerializeField] Animator Playeranimator;

    float timeSinceLastShot;

    private void Start() {
        PlayerMainController.shootInput += Shoot;
        PlayerMainController.reloadInput += StartReload;

        gunData.reloading = false;
    }

    void Update() {
        if ( !isLocalPlayer )
            return;

        timeSinceLastShot += Time.deltaTime;

        Debug.DrawRay(muzzle.position, muzzle.forward, Color.green);

        print(CanShoot());

        Playeranimator.SetBool("shooting", Input.GetMouseButton(0) && CanShoot());
        Playeranimator.SetBool("idle", !Input.GetMouseButton(0));
    }

    void StartReload() {
        if ( !isLocalPlayer )
            return;

        if ( gunData.reloading )
            return;

        animator.Play("Reload");
        Playeranimator.Play("Reload");
        StartCoroutine(Reload());
    }

    IEnumerator Reload() {
        gunData.reloading = true;
        yield return new WaitForSeconds(gunData.reloadTime);
        gunData.currentAmmo = gunData.magSize;
        gunData.reloading = false;
    }

    private bool CanShoot() => !gunData.reloading && gunData.currentAmmo > 0 && timeSinceLastShot > 1f / ( gunData.fireRate / 60.0f );

    void Shoot() {

        if ( !CanShoot() )
            return;

        CmdShoot(Camera.main.ScreenPointToRay(Input.mousePosition));
        gunData.currentAmmo--;
        timeSinceLastShot = 0.0f;
        OnGunShot();
    }

    private void OnGunShot() {
        animator.Play("Fire");
    }

    [Command(requiresAuthority = true)]
    void CmdShoot(Ray ray) {
        RpcShoot(ray);
    }

    [ClientRpc]
    void RpcShoot(Ray ray) {
        RaycastHit hit;

        if ( Physics.Raycast(ray, out hit, gunData.maxDistance) ) {
            if ( hit.collider.CompareTag("Player") && hit.collider.gameObject != gameObject ) {
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                damageable?.CmdDamage(gunData.damage);
            }
        }
    }
}