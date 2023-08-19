using Mirror;
using System.Collections;
using UnityEngine;

public class DeleteAfter : NetworkBehaviour
{
    [SerializeField] float destroyTime = 3;
    private void Start() {
        CmdDestroy();
    }

    [Command(requiresAuthority = false)]
    void CmdDestroy() {
        StartCoroutine(destroy());
    }

    IEnumerator destroy() {
        yield return new WaitForSeconds(destroyTime);
        NetworkServer.Destroy(gameObject);
    }
}
