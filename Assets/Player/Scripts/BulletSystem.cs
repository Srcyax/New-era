using UnityEngine;
using Mirror;

public class BulletSystem : MonoBehaviour
{
    Rigidbody rigdBody => GetComponent<Rigidbody>();

    private void Start() {
        //rigdBody.velocity = transform.forward * 100f;
    }

    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        if ( !other.CompareTag("Player") )
            return;

        if ( other.gameObject == gameObject )
            return;

        //other.GetComponent<PlayerMainController>().CmdLessHealth();
        Destroy(gameObject);
    }
}