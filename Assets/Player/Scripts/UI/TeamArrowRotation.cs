using UnityEngine;

public class TeamArrowRotation : MonoBehaviour {
    void Update() {
        if ( !Camera.main )
            return;

        Vector3 directionToTarget = Camera.main.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
    }
}
