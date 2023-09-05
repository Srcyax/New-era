using Mirror;
using UnityEngine;

public class PlayerNetworkClient : MonoBehaviour {
    LobbyPlayers info;

    private void Start() {
       /* info = FindObjectOfType<LobbyPlayers>();

        if ( info && info.canStart ) {
            NetworkManager.singleton.StopClient();
        }*/
    }
}