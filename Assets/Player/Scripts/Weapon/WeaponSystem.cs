using Cinemachine;
using Mirror;
using System.Collections;
using TMPro;
using UnityEngine;

public class WeaponSystem : NetworkBehaviour {
    [SerializeField] GetCurrentWeapon weapon;
    [SerializeField] GunData gunData;
    [SerializeField] Animator animator;

    [Header("Weapon components")]
    public bool gunReady;

    [Header("Player components")]
    [SerializeField] PlayerAnimations playerAnimations;
    [SerializeField] PlayerComponents components;
    [SerializeField] CharacterController characterController;
    [SerializeField] public PlayerData playerData;
    [SerializeField] AudioSource playerAudioSource;
    [SerializeField] Transform playerCharacterWeapons;

    [Header("Recoil components")]
    [SerializeField] CinemachineImpulseSource cameraShake;
    [SerializeField] Sway sway;

    [Header("Weapon components")]
    [SerializeField] RecoilSystem recoilSystem;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject bulletImpact;
    [SerializeField] GameObject bulletHitImpact;
    [SerializeField] AudioSource soundEffect;
    [SerializeField] Transform bocal;


    [SerializeField] TextMeshProUGUI ammoUI;
    [SerializeField] Transform canvas;
    [SerializeField] GameObject hitMarker;

    [HideInInspector] public Vector2 cameraAxis;

    float timeSinceLastShot;

    void Start() {
        WeaponSet();
        gunReady = true;
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

        playerAnimations?.animator.Play(gunData.name + "_reload");
        animator?.Play("Reload");
        cameraShake.GenerateImpulse();
        sway.ShootSway(0f);
        StartCoroutine(Reload());
    }

    IEnumerator Reload() {
        gunData.reloading = true;
        yield return new WaitForSeconds(gunData.reloadTime);
        gunData.currentAmmo = gunData.magSize;
        gunData.reloading = false;
        UpdateUI();
    }

    bool CanShoot() => !gunData.reloading && gunData.currentAmmo > 0 && timeSinceLastShot > 1f / ( gunData.fireRate / 60.0f ) && gunReady;

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
        if ( !isLocalPlayer )
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
        animator = weapon.currentWeapon.GetComponent<Animator>();
        SetShootSound();
        gunData.reloading = false;
        gunReady = false;
        UpdateUI();
        sway.ShootSway(0f);
    }

    void SetShootSound() {
        soundEffect.clip = weapon.currentWeapon?.GetComponent<WeaponInfo>().shotSound;
    }

    [Command(requiresAuthority = true)]
    public void CmdSwitchCharacterWeapon(int current) {
        RpcSwitchCharacterWeapon(current);
    }

    [ClientRpc]
    void RpcSwitchCharacterWeapon(int current) {
        for ( int i = 0; i < playerCharacterWeapons.childCount; i++ ) {
            if ( !playerCharacterWeapons.GetChild(i).gameObject.CompareTag("CharacterWeapon") )
                continue;

            if ( i == current )
                continue;

            playerCharacterWeapons.GetChild(i).gameObject.SetActive(false);
        }

        playerCharacterWeapons.GetChild(current).gameObject.SetActive(true);
    }

    void UpdateUI() {
        ammoUI.text = gunData.currentAmmo.ToString() + "/∞";
    }

    [Command(requiresAuthority = true)]
    void CmdShoot(Ray ray) {
        RpcShoot(ray);
    }

    [ClientRpc]
    void RpcShoot(Ray ray) {
        soundEffect.PlayOneShot(soundEffect.clip);

        Vector3 direction = GetSpreadDirection(ray.direction);
        playerAnimations?.animator.Play(gunData.name + "_shooting");
        int hitBoxLayer = LayerMask.GetMask("Hitboxes");
        int worldLayer = LayerMask.GetMask("World");
        if ( Physics.Raycast(ray.origin, direction, out RaycastHit hit, gunData.maxDistance, hitBoxLayer) ) {
            Debug.DrawLine(ray.origin, hit.point, Color.red, 1);
            switch ( hit.collider.tag ) {
                case "Head":
                    GiveHit(0, hit);
                    return;
                case "Chest":
                    GiveHit(1, hit);
                    return;
                case "LowerChest":
                    GiveHit(2, hit);
                    return;
                case "Arms":
                    GiveHit(3, hit);
                    return;
                case "Legs":
                    GiveHit(4, hit);
                    return;
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

        if ( player.playerHealth <= 0 )
            return;

        if ( components.playerTeam == player.playerTeam )
            return;

        Instantiate(hitMarker, canvas);
        Instantiate(bulletHitImpact, hit.collider.transform).transform.parent = null;
        playerAudioSource.Play();
        print(hit.collider.tag + " : " + gunData.damages[ i ]);
        IDamageable damageable = hit.collider.transform.root.GetComponent<IDamageable>();
        damageable?.CmdDamage(gunData.damages[ i ], components, player, "killed");
    }

    private Vector3 GetSpreadDirection(Vector3 dir) {
        Vector3 direction = dir;

        if ( characterController.velocity.magnitude > 4 ) {
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