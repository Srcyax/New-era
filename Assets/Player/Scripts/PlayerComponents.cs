using Mirror;
using UnityEngine;

public class PlayerComponents : NetworkBehaviour {
    [Header("Player variables")]
    [SyncVar] public float  playerHealth = 100;
    [SyncVar] public string playerName;
    [SyncVar] public int    playerTeam = -1;
    public float playerRunSpeed = 8f;
    public float playerWalkSpeed = 3.5f;
    [SyncVar] public bool   localPlayer;

    [Header("Components to disable")]
    [SerializeField] GameObject[] playerBody;
    [SerializeField] GameObject[] playerObjs;

    [Space(10)]
    [SerializeField] PlayerMainController mainController;

    private void Start() {
        mainController.playerCamera.enabled = isLocalPlayer && mainController.playerHasTeam;
        mainController.playerCamera.GetComponent<AudioListener>().enabled = isLocalPlayer;

        localPlayer = isLocalPlayer;
        for ( int i = 0; i < playerObjs.Length; i++ ) {
            playerObjs[ i ].SetActive(isLocalPlayer);
        }

        for ( int i = 0; i < playerBody.Length; i++ ) {
            playerBody[ i ].SetActive(!isLocalPlayer);
        }
    }
}
