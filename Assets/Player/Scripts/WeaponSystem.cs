using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;
using System.Collections;

public class WeaponSystem : NetworkBehaviour
{
    [SerializeField] GunData gunData;
    [SerializeField] Transform muzzle; 
    [SerializeField] Animator animator;
    [SerializeField] Animator Playeranimator;
    [SerializeField] GameObject impactShot;

    [Header("Spread System")]
    [SerializeField] float maxSpread;
    [SerializeField] float spreadIncreaseRate;
    [SerializeField] float spreadDecreaseRate;
    private float currentSpread = 0f;
    [SerializeField] Image spreadImage;
  

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

        Playeranimator.SetBool("shooting", Input.GetMouseButton(0) && CanShoot());
        Playeranimator.SetBool("idle", !Input.GetMouseButton(0));

        Spread();
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

        Vector3 raycastDirection = ray.direction;

        Vector2 randomSpread = UnityEngine.Random.insideUnitCircle * currentSpread;
        raycastDirection += Camera.main.transform.right * randomSpread.x + Camera.main.transform.up * randomSpread.y;

        RaycastHit hit;

        if ( Physics.Raycast(ray.origin, raycastDirection.normalized, out hit, gunData.maxDistance) ) {
            Instantiate(impactShot, new Vector3(hit.point.x, hit.point.y, hit.point.z), Quaternion.identity);
            Debug.DrawLine(ray.origin, hit.point, Color.red, 1);
            if ( hit.collider.CompareTag("Player") && hit.collider.gameObject != gameObject ) {
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                damageable?.CmdDamage(gunData.damage);
            }
        }
    }


    void Spread() {
        float currentSpreadRatio = Mathf.Clamp01(currentSpread / maxSpread);
        float circleSize = currentSpreadRatio;
        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        spreadImage.rectTransform.sizeDelta = new Vector2(circleSize, circleSize);
        currentSpread = isMoving ? Mathf.Clamp(currentSpread + spreadIncreaseRate * Time.deltaTime, 0f, maxSpread) : Mathf.Clamp(currentSpread - spreadDecreaseRate * Time.deltaTime, 0f, maxSpread);
    }
}