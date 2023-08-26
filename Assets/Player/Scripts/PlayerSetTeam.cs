using Mirror;

public class PlayerSetTeam : NetworkBehaviour
{
    PlayerMainController mainController => GetComponent<PlayerMainController>();

    [Command(requiresAuthority = false)]
    public void CmdSetPlayerTeam(int team) {
        RpcSetPlayerTeam(team);
    }

    [ClientRpc]
    void RpcSetPlayerTeam(int team) {
        mainController.playerTeam = team;
        mainController.playerCamera.enabled = true;
    }
}