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

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        playersLogged = players.Length;

        //CmdLobbyStart();
    }

    [Command(requiresAuthority = false)]
    public void CmdLobbyStart() {
        if ( playersLogged >= networkManager.maxConnections ) {
            StartCoroutine(CanStart());
        }
    }

    IEnumerator CanStart() {
        yield return new WaitForSeconds(.2f);
        canStart = true;
    }
}