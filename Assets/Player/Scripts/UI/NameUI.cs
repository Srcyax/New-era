using Mirror;
using UnityEngine;

public class NameUI : NetworkBehaviour {
    [SerializeField] PlayerComponents components;

    [Command(requiresAuthority = false)]
    public void CmdSetPlayerName(string name) {
        RpcSetPlayerName(name);
    }

    [ClientRpc]
    void RpcSetPlayerName(string name) {
        components.playerName = name;
    }
}
