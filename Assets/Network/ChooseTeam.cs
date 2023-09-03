using TMPro;
using UnityEngine;
using Mirror;
using System.Collections;
using System.Runtime.InteropServices;

public class ChooseTeam : NetworkBehaviour {
    [SyncVar] public int ice, fire;
    LobbyPlayers lobbyManager;
    Feed feed;

    [Header("UI settings")]
    [SerializeField] TextMeshProUGUI[] teamsPlayers;
    [SerializeField] GameObject playerName;
    [SerializeField] Transform waitingPlayers;

    Canvas matchStatusCanvas;

    void Start() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        lobbyManager = FindObjectOfType<LobbyPlayers>();
        matchStatusCanvas = GameObject.FindGameObjectWithTag("MatchStatus").GetComponent<Canvas>();
        feed = FindObjectOfType<Feed>();
    }

    void Update() {
        teamsPlayers[ 0 ].text = ice.ToString();
        teamsPlayers[ 1 ].text = fire.ToString();

        if ( !isServer )
            return;

        //CmdWaitingPlayers();
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

        ice++;
    }

    [Command(requiresAuthority = false)]
    public void CmdJoinTeamFire(string team) {
        if ( !lobbyManager.canStart)
            return;

        fire++;
    }

    public void JoinTeam() {
        if ( !lobbyManager.canStart )
            return;

        gameObject.SetActive(false);
        matchStatusCanvas.enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
            feed.CmdFeedPlayerTeamJoined(players[ i ].GetComponent<PlayerComponents>().playerName, team == 0 ? "ICE" : "FIRE");
            break;
        }
    }
}