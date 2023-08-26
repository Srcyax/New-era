using Mirror;

public class NameUI : NetworkBehaviour {
    PlayerMainController mainController => GetComponent<PlayerMainController>();

    [Command(requiresAuthority = false)]
    public void CmdSetPlayerName(string name) {
        RpcSetPlayerName(name);
    }

    [ClientRpc]
    void RpcSetPlayerName(string name) {
        mainController.playerName = name;
    }
}
