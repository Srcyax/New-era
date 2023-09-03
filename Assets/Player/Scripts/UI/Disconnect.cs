using Mirror;
using UnityEngine.SceneManagement;

public class Disconnect : NetworkBehaviour {
    public void DisconnectPlayer() {
        SceneManager.LoadScene("Game");
    }
}