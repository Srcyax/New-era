using TMPro;
using UnityEngine;
using Mirror;
using System.Collections;

public class ChooseTeam : NetworkBehaviour {
    [SyncVar] public int ice, fire;
    GameObject[] players;
    LobbyPlayers lobbyManager;

    [Header("UI settings")]
    [SerializeField] TextMeshProUGUI[] teamsPlayers;

    void Start() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        lobbyManager = FindObjectOfType<LobbyPlayers>();
    }

    void Update() {
        CmdTeamsInfo(ice, fire);
    }

    [Command(requiresAuthority = false)]
    void CmdTeamsInfo(int ice, int fire) {
        RpcTeamsInfo(ice, fire);
    }

    [ClientRpc]
    void RpcTeamsInfo(int ice, int fire) {
        teamsPlayers[ 0 ].text = ice.ToString();
        teamsPlayers[ 1 ].text = fire.ToString();
    }

    [Command(requiresAuthority = false)]
    public void CmdJoinTeamIce(int team) {
        if ( !lobbyManager.canStart )
            return;

        ice++;
    }

    [Command(requiresAuthority = false)]
    public void CmdJoinTeamFire(int team) {
        if ( !lobbyManager.canStart )
            return;

        fire++;
    }

    public void JoinTeam() {
        if ( !lobbyManager.canStart )
            return;

        gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetPlayerTeam(int team) {
        players = GameObject.FindGameObjectsWithTag("Player");
        for ( int i = 0; i < players.Length; i++ ) {
            if ( !players[ i ] )
                continue;
            
            if ( !players[ i ].GetComponent<PlayerMainController>().localPlayer )
                continue;

            players[ i ].GetComponent<PlayerSetTeam>().CmdSetPlayerTeam(team);
            players[ i ].GetComponent<PlayerDamage>().CmdDamage(100, "", "");
            break;
        }
    }
}