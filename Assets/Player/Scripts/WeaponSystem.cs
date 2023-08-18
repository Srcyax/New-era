﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;
using Mirror;
using System;
using System.Collections;
using TMPro;

public class WeaponSystem : NetworkBehaviour
{
    [SerializeField] GunData gunData;
    [SerializeField] Transform muzzle; 
    [SerializeField] Animator animator;
    [SerializeField] Animator playerAnimator;
    [SerializeField] CharacterController playerController;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject bulletImpact;
    [SerializeField] GameObject soundEffect;

    [Header("Spread System")]
    [SerializeField]  float spreadIncreaseRate;
    [SerializeField]  float spreadDecreaseRate;
    [HideInInspector] float currentSpread = 0f;

    [SerializeField] Image spreadImage;
    [SerializeField] TextMeshProUGUI ammoUI;

    float timeSinceLastShot;

    void Start() {
        PlayerMainController.shootInput += Shoot;
        PlayerMainController.reloadInput += CmdStartReload;
        PlayerMainController.playerDied += WeaponReset;

        gunData.reloading = false;
    }

    void Update() {
        if ( !isLocalPlayer )
            return;

        timeSinceLastShot += Time.deltaTime;

        /*playerAnimator.SetBool("shooting", Input.GetMouseButton(0) && CanShoot());
        playerAnimator.SetBool("idle", !Input.GetMouseButton(0));*/

        Spread();

        ammoUI.text = gunData.currentAmmo.ToString() + "/∞";
    }

    [Command(requiresAuthority = true)]
    void CmdStartReload() {
        RpcStartReload();
        playerAnimator.Play("Reload");
    }

    [ClientRpc]
    void RpcStartReload() {
        if ( !isLocalPlayer )
            return;

        if ( gunData.reloading )
            return;

        if (gunData.currentAmmo >= gunData.magSize ) 
            return;

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
        Vector3 raycastDirection = ray.direction;
        Vector2 randomSpread = UnityEngine.Random.insideUnitCircle * currentSpread;
        raycastDirection += Camera.main.transform.right * randomSpread.x + Camera.main.transform.up * randomSpread.y;
        playerAnimator.Play("shooting");
        if ( Physics.Raycast(ray.origin, raycastDirection.normalized, out RaycastHit hit, gunData.maxDistance) ) {
            GameObject obj = Instantiate(bulletImpact, new Vector3(hit.point.x, hit.point.y, hit.point.z + -.04f), Quaternion.identity);
            obj.transform.rotation = Camera.main.transform.localRotation;
            obj.transform.parent = hit.transform;
/*#if UNITY_EDITOR
            Debug.DrawLine(ray.origin, hit.point, Color.red, 1);
#endif*/
            for ( int i = 0; i < gunData.hitboxes.Length; i++ ) {
                if ( hit.collider.CompareTag(gunData.hitboxes[ i]) && hit.collider.transform.root != gameObject.transform ) {
                    print(hit.collider.tag + " : " + gunData.damages[ i]);
                    IDamageable damageable = hit.collider.transform.root.GetComponent<IDamageable>();
                    damageable?.CmdDamage(gunData.damage + gunData.damages[ i]);
                }
            }
        }
    }

    void Spread() {
        float currentSpreadRatio = Mathf.Clamp01(currentSpread / gunData.spread);
        float circleSize = currentSpreadRatio;
        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        //print(currentSpread);
        spreadImage.rectTransform.sizeDelta = new Vector2(circleSize, circleSize);
        currentSpread = isMoving ? Mathf.Clamp(currentSpread + playerController.velocity.magnitude * Time.deltaTime, 0f, playerController.velocity.magnitude / gunData.spread) : Mathf.Clamp(currentSpread - spreadDecreaseRate * Time.deltaTime, 0f, playerController.velocity.magnitude / gunData.spread);
    }
}