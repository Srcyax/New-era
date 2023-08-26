using Mirror;
using UnityEngine;

public class PlayerSetTeam : NetworkBehaviour
{
    [SerializeField] PlayerComponents components;
    [SerializeField] PlayerMainController mainController;

    [Command(requiresAuthority = false)]
    public void CmdSetPlayerTeam(int team) {
        RpcSetPlayerTeam(team);
    }

    [ClientRpc]
    void RpcSetPlayerTeam(int team) {
        components.playerTeam = team;
        mainController.playerCamera.enabled = true;
    }
}