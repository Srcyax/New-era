using UnityEngine;
using Mirror;
using System.Collections;

public class LobbyPlayers : NetworkBehaviour
{
    [SyncVar] public int playersLogged;
    [SyncVar] public bool canStart;

    [Header("Network settings")]
    [SerializeField] private NetworkManager networkManager;

    void Update()
    {
        if ( !isServer && canStart )
            return;

        CmdLobbyStart();
    }

    [Command(requiresAuthority = false)]
    public void CmdLobbyStart() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        playersLogged = players.Length;

        if ( playersLogged >= networkManager.maxConnections ) {
            StartCoroutine(CanStart());
        }
    }

    IEnumerator CanStart() {
        yield return new WaitForSeconds(5f);
        canStart = true;
    }
}