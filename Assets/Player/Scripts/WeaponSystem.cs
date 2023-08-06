using UnityEngine;
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
    [SerializeField] float spreadIncreaseRate;
    [SerializeField] float spreadDecreaseRate;
    private float currentSpread = 0f;
    [SerializeField] Image spreadImage;
    [SerializeField] TextMeshProUGUI ammoUI;

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

        playerAnimator.SetBool("shooting", Input.GetMouseButton(0) && CanShoot());
        playerAnimator.SetBool("idle", !Input.GetMouseButton(0));

        Spread();

        ammoUI.text = gunData.currentAmmo.ToString() + "/∞";
    }

    void StartReload() {
        if ( !isLocalPlayer )
            return;

        if ( gunData.reloading )
            return;

        if (gunData.currentAmmo >= gunData.magSize ) 
            return;

        animator.Play("Reload");
        playerAnimator.Play("Reload");
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
        animator.Play("Fire");
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

        RaycastHit hit;

        if ( Physics.Raycast(ray.origin, raycastDirection.normalized, out hit, gunData.maxDistance) ) {
#if UNITY_EDITOR
            GameObject obj = Instantiate(bulletImpact, new Vector3(hit.point.x, hit.point.y, hit.point.z+ -.05f), Quaternion.identity);
            obj.transform.localRotation = hit.transform.localRotation;
            Debug.DrawLine(ray.origin, hit.point, Color.red, 1);
#endif

            string[] hitboxes = {
                "Head",
                "Chest",
                "LowerChest",
                "Arms",
                "Legs"
            };

            int[] damages = {
                80,
                40,
                25,
                15,
                10
            };

            for ( int i = 0; i < hitboxes.Length; i++ ) {
                if ( hit.collider.CompareTag(hitboxes[i]) && hit.collider.gameObject != gameObject ) {
                    print(hit.collider.tag + " : " + damages[i]);
                    IDamageable damageable = hit.collider.transform.root.GetComponent<IDamageable>();
                    damageable?.CmdDamage(gunData.damage + damages[i]);
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