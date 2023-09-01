using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerComponents : NetworkBehaviour {
    [Header("Player variables")]
    [SyncVar] public float  playerHealth = 100;
    [SyncVar] public string playerName;
    [SyncVar] public int    playerTeam = -1;
    [SyncVar] public bool   localPlayer;
    [SyncVar] public bool   spawning;
    [SyncVar] public float spawnTime = 3;
    [SyncVar] public int kills = 0;
    [SyncVar] public int deaths = 0;

    [Space(15)]

    public float playerRunSpeed = 8f;
    public float playerWalkSpeed = 3.5f;

    [Header("Components to disable")]
    [SerializeField] GameObject[] playerBody;
    [SerializeField] GameObject[] playerObjs;

    [Space(10)]
    [SerializeField] PlayerMainController mainController;
    [SerializeField] PlayerData playerData;

    private void Start() {
        mainController.playerCamera.enabled = isLocalPlayer && mainController.playerHasTeam;
        mainController.playerCamera.GetComponent<AudioListener>().enabled = isLocalPlayer;
        mainController.playerCamera.GetComponent<PostProcessVolume>().enabled = isLocalPlayer;

        localPlayer = isLocalPlayer;
        if ( localPlayer ) {
            kills = playerData.kills;
            deaths = playerData.deaths;
        }

        for ( int i = 0; i < playerObjs.Length; i++ ) {
            playerObjs[ i ].SetActive(isLocalPlayer);
        }

        for ( int i = 0; i < playerBody.Length; i++ ) {
            playerBody[ i ].SetActive(!isLocalPlayer);
        }
    }

    [Command(requiresAuthority =false)]
    public void CmdSpawning() {
        RpcSpawning();
    }

    [ClientRpc]
    void RpcSpawning() {
        StartCoroutine(Spawning());
    }

    IEnumerator Spawning() {
        spawning = true;
        yield return new WaitForSeconds(spawnTime);
        spawning = false;
    }
}
