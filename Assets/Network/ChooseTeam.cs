using TMPro;
using UnityEngine;
using Mirror;


public class ChooseTeam : NetworkBehaviour
{
    [SyncVar] public int ice, fire;
    GameObject[] players;

    [Header("UI settings")]
    [SerializeField] TextMeshProUGUI[] teamsPlayers;

    void Start()
    {
        
    }

    void Update()
    {
        CmdTeamsInfo(ice, fire);

        players = GameObject.FindGameObjectsWithTag("Player");
    }

    [Command(requiresAuthority =false)]
    void CmdTeamsInfo(int ice, int fire) {
        RpcTeamsInfo(ice, fire);
    }

    [ClientRpc]
    void RpcTeamsInfo(int ice, int fire) {
        teamsPlayers[ 0 ].text = ice.ToString();
        teamsPlayers[ 1 ].text = fire.ToString();
    }

    [Command(requiresAuthority = false)]
    public void CmdJoinTeamIce() {
        ice++;
        RpcSetPlayerTeam(0);
    }

    [Command(requiresAuthority = false)]
    public void CmdJoinTeamFire() {
        fire++;
        RpcSetPlayerTeam(1);
    }

    public void JoinTeam() {
        gameObject.SetActive(false);
    }

    [ClientRpc]
    void RpcSetPlayerTeam(int team) {
        GameObject[] spawnPoints = team == 0 ? GameObject.FindGameObjectsWithTag("ICE_SpawPoints") : GameObject.FindGameObjectsWithTag("FIRE_SpawPoints");
        for ( int i = 0; i < players.Length; i++ ) {
            if ( !players[ i ] )
                continue;

            if ( players[ i ].GetComponent<PlayerMainController>().isLocalPlayer ) {
                players[ i ].GetComponent<PlayerMainController>().playerTeam = team;
               // players[ i ].GetComponent<PlayerMainController>().CmdDamage(100);

                int r = Random.Range(0, spawnPoints.Length);
                print(spawnPoints[ r ].transform.localPosition);
                players[ i ].transform.localPosition = spawnPoints[ r ].transform.localPosition;
            }
        }
    }
}