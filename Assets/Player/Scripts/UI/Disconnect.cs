using Mirror;
using UnityEngine.SceneManagement;

public class Disconnect : NetworkBehaviour {
    public void DisconnectPlayer() {
        if ( NetworkServer.active ) {
            NetworkManager.singleton.StopHost();
        }
        else if ( NetworkClient.active ) {
            NetworkManager.singleton.StopClient();
        }
        SceneManager.LoadScene("Game");
    }
}