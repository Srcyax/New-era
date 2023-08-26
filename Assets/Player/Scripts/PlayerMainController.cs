using Mirror;
using System;
using System.Collections;
using UnityEngine;

public class PlayerMainController : NetworkBehaviour {
    public static Action shootInput;
    public static Action reloadInput;
    public static Action playerDied;

    [Header("Player components")]
    [SerializeField] PlayerData playerData;
    [SerializeField] PlayerAnimations playerAnimations;
    [SerializeField] PlayerDamage playerDamage;
    [SerializeField] HealthUI playerHealthUI;
    [SerializeField] NameUI playerNameUI;
    [SerializeField] public Camera playerCamera;

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

    private GameObject[] spawnPoints;
    private Vector3 moveDirection = Vector3.zero;
    [HideInInspector]
    public CharacterController characterController;

    private GameObject waitingPlayers;
    private LobbyPlayers lobbyManager;

    private float lookXLimit = 80.0f;
    private float rotationX = 0;

    void Start() {
        GameObject lobby = GameObject.FindGameObjectWithTag("Lobby");
        waitingPlayers = GameObject.FindGameObjectWithTag("WaitingPlayersCanvas");
        lobbyManager = FindObjectOfType<LobbyPlayers>();
        characterController = GetComponent<CharacterController>();
        
        playerCamera.enabled = isLocalPlayer && playerHasTeam;
        playerCamera.GetComponent<AudioListener>().enabled = isLocalPlayer;
        Destroy(lobby);
        localPlayer = isLocalPlayer;
        for ( int i = 0; i < playerObjs.Length; i++ ) {
            playerObjs[ i ].SetActive(isLocalPlayer);
        }

        for ( int i = 0; i < playerBody.Length; i++ ) {
            playerBody[ i ].SetActive(!isLocalPlayer);
        }
    }

    void Update() {
        if ( !isLocalPlayer )
            return;

        if ( isLocalPlayerDead )
            return;

        if ( lobbyManager.canStart && waitingPlayers )
            Destroy(waitingPlayers);

        if ( isWaitingForPlayers )
            return;

        if ( !playerHasTeam )
            return;

        PlayerControler();
        playerAnimations.Animations();
        playerNameUI.CmdSetPlayerName(playerData.name);

        if ( Input.GetMouseButton(0) ) {
            shootInput?.Invoke();
        }

        if ( Input.GetKeyDown(KeyCode.R) ) {
            reloadInput?.Invoke();
        }
    }

    public IEnumerator Respawn() {
        spawnPoints = playerTeam == 1 ? GameObject.FindGameObjectsWithTag("FIRE_SpawPoints") : GameObject.FindGameObjectsWithTag("ICE_SpawPoints");
        playerAnimations.animator.enabled = false;
        playerHealthUI.deadScreenUI.SetActive(true);
        characterController.enabled = false;
        yield return new WaitForSeconds(3.0f);
        int r = UnityEngine.Random.Range(0, spawnPoints.Length);
        transform.position = spawnPoints[ r ].transform.position;
        yield return new WaitForSeconds(.5f);
        characterController.enabled = true;
        playerAnimations.animator.enabled = true;
        playerHealth = 100;
        playerHealthUI.deadScreenUI.SetActive(false);
        playerDied?.Invoke();
    }

    void PlayerControler() {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = (isPlayerRunning ? playerRunSpeed : playerWalkSpeed) * Input.GetAxis("Vertical");
        float curSpeedY = (isPlayerRunning ? playerRunSpeed - Input.GetAxis("Horizontal") : playerWalkSpeed) * Input.GetAxis("Horizontal");
        moveDirection = ( ( forward * Mathf.Clamp(curSpeedX, -8, 8) ) + ( right * Mathf.Clamp(curSpeedY, -5, 5) ) );

        moveDirection = Vector3.ClampMagnitude(moveDirection, 10.7f);

        if ( !characterController.isGrounded ) {
            moveDirection.y -= 8f; // gravity
        }

        characterController.Move(moveDirection * Time.deltaTime);
        rotationX += -Input.GetAxis("Mouse Y") * 3f;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * 3f, 0);

        if ( transform.localPosition.y <= -10 )
            playerDamage.CmdDamage(100, "", "");
    }

    public bool playerHasTeam {
        get { return playerTeam is 0 or 1; }
    }

    public bool isLocalPlayerDead {
        get { return playerHealth <= 0; }
    }

    public bool isWaitingForPlayers {
        get { return waitingPlayers; }
    }

    public bool isPlayerRunning {
        get {
            return Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift);
        }
    }
}