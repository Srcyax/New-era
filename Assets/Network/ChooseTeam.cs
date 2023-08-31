using TMPro;
using UnityEngine;
using Mirror;
using System.Collections;
using System.Runtime.InteropServices;

public class ChooseTeam : NetworkBehaviour {
    [SyncVar] public int ice, fire;
    LobbyPlayers lobbyManager;

    [Header("UI settings")]
    [SerializeField] TextMeshProUGUI[] teamsPlayers;
    [SerializeField] GameObject playerName;
    [SerializeField] Transform waitingPlayers;

    void Start() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        lobbyManager = FindObjectOfType<LobbyPlayers>();
    }

    void Update() {
        if ( !isServer )
            return;

        CmdTeamsInfo();
        CmdWaitingPlayers();
    }

    [Command(requiresAuthority =false)]
    void CmdTeamsInfo() {
        RpcTeamsInfo();
    }

    [ClientRpc]
    void RpcTeamsInfo() {
        teamsPlayers[ 0 ].text = ice.ToString();
        teamsPlayers[ 1 ].text = fire.ToString();
    }

    int index = -1;
    [Command(requiresAuthority =false)]
    void CmdWaitingPlayers() {
        RpcWaitingPlayuers();
    }

    [ClientRpc]
    void RpcWaitingPlayuers() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if ( index != players.Length ) {
            TextMeshProUGUI name = playerName.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            //name.text = players[ players.Length - 1 ].GetComponent<PlayerComponents>().playerName;
            if ( waitingPlayers.childCount > 0 ) {
                Transform lastChild = waitingPlayers.GetChild(waitingPlayers.childCount - 1);
                Vector3 newPosition = new Vector3(0f, lastChild.localPosition.y - 60f, 0f);
                GameObject pl = Instantiate(playerName, waitingPlayers);
                pl.transform.localPosition = newPosition;
            }
            else {
                Instantiate(playerName, waitingPlayers);
                print(players[ players.Length - 1 ].GetComponent<PlayerComponents>().playerName);
            }

            index = players.Length;
        }
    }


    [Command(requiresAuthority = false)]
    public void CmdJoinTeamIce(string team) {
        if ( !lobbyManager.canStart)
            return;

        RpcChatFeed(team);
        ice++;
    }

    [Command(requiresAuthority = false)]
    public void CmdJoinTeamFire(string team) {
        if ( !lobbyManager.canStart)
            return;
        RpcChatFeed(team);
        fire++;
    }

    public void JoinTeam() {
        if ( !lobbyManager.canStart )
            return;

        gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    [ClientRpc]
    void RpcChatFeed(string team) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for ( int i = 0; i < players.Length; i++ ) {
            if ( !players[ i ] )
                continue;

            if ( !players[ i ].GetComponent<PlayerComponents>().localPlayer )
                continue;

            string name = players[ i ].GetComponent<PlayerComponents>().playerName;

            players[ i ].GetComponent<Feed>().FeedPlayerTeamJoined(name, team);
            break;
        }
    }

    public void SetPlayerTeam(int team) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for ( int i = 0; i < players.Length; i++ ) {
            if ( !players[ i ] )
                continue;
            
            if ( !players[ i ].GetComponent<PlayerComponents>().localPlayer )
                continue;

            players[ i ].GetComponent<PlayerSetTeam>().CmdSetPlayerTeam(team);
            players[ i ].GetComponent<PlayerDamage>().CmdDamage(100, "", "", "");
            break;
        }
    }
}