using UnityEngine;

public class BulletSystem : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;

    private void Start() {
        rigidbody.velocity = transform.forward * 10f;
    }

    void Update()
    {
        
    }
}
