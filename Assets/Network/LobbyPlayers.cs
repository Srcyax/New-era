using UnityEngine;
using Mirror;
public class LobbyPlayers : NetworkBehaviour
{
    [SyncVar] public int playersLogged;
    [SyncVar] public bool canStart;

    [Header("Network settings")]
    [SerializeField] private NetworkManager networkManager;

    void Update()
    {
        if ( !isServer )
            return;

        CmdLobbyStart();
    }

    [Command(requiresAuthority = false)]
    void CmdLobbyStart() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        playersLogged = players.Length;

        canStart = playersLogged >= networkManager.maxConnections;
    }
}