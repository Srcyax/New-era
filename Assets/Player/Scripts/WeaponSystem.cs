using UnityEngine;
using Mirror;

public class WeaponSystem : NetworkBehaviour
{
    [SerializeField] private GameObject BulletPrefab;
    [SerializeField] private Transform Bocal;
    [SerializeField] private float fireRate;
    private float shootTimer = 0f;

    void Update() {
        if ( !isLocalPlayer )
            return;

        shootTimer += Time.deltaTime;

        if ( Input.GetMouseButton(0) && shootTimer >= fireRate ) {
            CmdSpawnShoot(Camera.main.ScreenPointToRay(Input.mousePosition));
            shootTimer = 0f;
        }
    }

    [Command(requiresAuthority = true)]
    void CmdSpawnShoot(Ray ray) {
        RpcSpawnShoot(ray);
    }

    [ClientRpc]
    void RpcSpawnShoot(Ray ray) {
        RaycastHit hit;

        if ( Physics.Raycast(ray, out hit) ) {
            if ( hit.collider.CompareTag("Player") ) {
                hit.collider.GetComponent<PlayerMainController>().CmdLessHealth();
                print(hit.collider.GetComponent<PlayerMainController>().playerHealth);
            }
        }
    }
}