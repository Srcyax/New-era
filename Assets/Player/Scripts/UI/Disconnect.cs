using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Disconnect : NetworkBehaviour {
    public void DisconnectPlayer() {
        if ( NetworkServer.active ) {
            NetworkManager.singleton.StopHost();
        }
        else if ( NetworkClient.active ) {
            NetworkManager.singleton.StopClient();
        }
        //Time.timeScale = 1;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}