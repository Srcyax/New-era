using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [SerializeField] private GameObject Bullet;
    [SerializeField] private Transform Bocal;
    void Start()
    {
        
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            if ( Input.GetMouseButton(0) ) {
                Instantiate(Bullet, Bocal).transform.parent = null;
            }
        }
    }
}
