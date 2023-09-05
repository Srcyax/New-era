using TMPro;
using UnityEngine;
using Mirror;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class ChooseTeam : NetworkBehaviour {
    [SyncVar] public int ice, fire;
    LobbyPlayers lobbyManager;
    Feed feed;

    [SerializeField] NetworkManager networkManager;

    [Header("UI settings")]
    [SerializeField] TextMeshProUGUI[] teamsPlayers;
    [SerializeField] TextMeshProUGUI playersUI;
    [SerializeField] GameObject startButton;

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

        WaitingPlayers();
    }

    void WaitingPlayers() {
        if ( !startButton )
            return;

        startButton.SetActive(lobbyManager.playersLogged >= networkManager.maxConnections);
        playersUI.text = lobbyManager.playersLogged.ToString() + "/" + networkManager.maxConnections.ToString();
    }


    [Command(requiresAuthority = false)]
    public void CmdJoinTeamIce(string team) {
        ice++;
    }

    [Command(requiresAuthority = false)]
    public void CmdJoinTeamFire(string team) {
        fire++;
    }

    public void JoinTeam() {
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