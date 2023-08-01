using Mirror;
using System.Collections;
using UnityEngine;

public class DeleteAfter : NetworkBehaviour
{
    private void Start() {
        CmdDestroy();
    }

    [Command(requiresAuthority = false)]
    void CmdDestroy() {
        StartCoroutine(destroy());
    }

    IEnumerator destroy() {
        yield return new WaitForSeconds(3f);
        NetworkServer.Destroy(gameObject);
    }
}
