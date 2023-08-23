using Mirror;
using System.Collections;
using TMPro;
using UnityEngine;
using System;

public class PlayerMainController : NetworkBehaviour, IDamageable {
    private LobbyPlayers lobbyManager;
    private MatchStatus matchStatus;
    private PlayerKillfeed killFeed => GetComponent<PlayerKillfeed>();

    [SerializeField] PlayerData playerData;

    [Space(10)]

    [SerializeField] public static Action shootInput;
    [SerializeField] public static Action reloadInput;
    [SerializeField] public static Action playerDied;
    [SerializeField] private Animator animator;
    [SerializeField] private Camera playerCamera;
    [SyncVar] public float playerHealth = 100;
    [SyncVar] public string playerName;
    [SyncVar] public int playerTeam = -1;
    [SyncVar] public bool localPlayer;

    [Header("Components to hide")]
    [SerializeField] private GameObject[] playerBody;
    [SerializeField] private GameObject[] playerObjs;
    [SerializeField] private Transform footPos;

    [SerializeField] private TextMeshProUGUI healthTextPro;
    private GameObject waitingPlayers;
    [SerializeField] private GameObject playerDeadUI;
    [SerializeField] private GameObject playerRagdoll;

    private GameObject[] spawnPoints;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;

    private float lookXLimit = 80.0f;
    private float rotationX = 0;

    private bool isRunning => Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift);
    private float shifitingSpeed = 8f;
    private float walkSpeed = 3.5f;

    void Start() {

        GameObject lobby = GameObject.FindGameObjectWithTag("Lobby");
        waitingPlayers = GameObject.FindGameObjectWithTag("WaitingPlayersCanvas");
        lobbyManager = FindObjectOfType<LobbyPlayers>();
        matchStatus = FindObjectOfType<MatchStatus>();
        Destroy(lobby);
        localPlayer = isLocalPlayer;
        playerCamera.enabled = isLocalPlayer && playerHasTeam;
        playerCamera.GetComponent<AudioListener>().enabled = isLocalPlayer;
        characterController = GetComponent<CharacterController>();
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


        playerCamera.enabled = playerHasTeam;

        PlayerControler();
        Animations();
        healthTextPro.text = playerHealth.ToString();
        CmdSetPlayerName(playerData.name);

        if ( Input.GetMouseButton(0) ) {
            shootInput?.Invoke();
            //CmdDamage(100);
        }

        if ( Input.GetKeyDown(KeyCode.R) ) {
            reloadInput?.Invoke();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdDamage(float damage, string killer_name, string killed_name) {
        RpcDamage(damage, killer_name, killed_name);
    }

    [ClientRpc]
    void RpcDamage(float damage, string killer_name, string killed_name) {
        playerHealth -= damage;
        if ( isLocalPlayerDead ) {
            if ( playerTeam == 0 ) {
                matchStatus.fire_score++;
            }
            else {
                matchStatus.ice_score++;
            }
            killFeed.RpcKillFeed(killer_name, killed_name);
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn() {
        spawnPoints = playerTeam == 1 ? GameObject.FindGameObjectsWithTag("FIRE_SpawPoints") : GameObject.FindGameObjectsWithTag("ICE_SpawPoints");
        animator.enabled = false;
        playerDeadUI.SetActive(true);
        characterController.enabled = false;
        yield return new WaitForSeconds(3.0f);
        int r = UnityEngine.Random.Range(0, spawnPoints.Length);
        transform.position = spawnPoints[ r ].transform.position;
        yield return new WaitForSeconds(.5f);
        characterController.enabled = true;
        animator.enabled = true;
        playerHealth = 100;
        playerDeadUI.SetActive(false);
        playerDied?.Invoke();
    }

    [Command(requiresAuthority = false)]
    public void CmdSetPlayerTeam(int team) {
        RpcSetPlayerTeam(team);
    }

    [ClientRpc]
    void RpcSetPlayerTeam(int team) {
        playerTeam = team;
    }

    [Command(requiresAuthority =false)]
    void CmdSetPlayerName(string name) {
        RpcSetPlayerName(name);
    }

    [ClientRpc]
    void RpcSetPlayerName(string name) {
        playerName = name;
    }

    void PlayerControler() {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = (isRunning ? shifitingSpeed : walkSpeed) * Input.GetAxis("Vertical");
        float curSpeedY = (isRunning ? shifitingSpeed - Input.GetAxis("Horizontal") : walkSpeed) * Input.GetAxis("Horizontal");
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
            CmdDamage(100, "", "");
    }

    float smoothing = 0.2f;
    float smoothInputX;
    float smoothInputY;
    void Animations() {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        smoothInputX = Mathf.Lerp(smoothInputX, horizontalInput, smoothing);
        if ( isGrounded()) {
            smoothInputY = Mathf.Lerp(smoothInputY, isRunning ? 2 : verticalInput, smoothing);
        }
        else {
            smoothInputY = Mathf.Lerp(smoothInputY, -2, smoothing);
        }

        animator.SetFloat("inputX", smoothInputX);
        animator.SetFloat("inputY", smoothInputY);
    }

    bool isGrounded() {
        if ( Physics.Raycast(footPos.transform.position, footPos.transform.forward, out RaycastHit hit, .5f) ) {        
            Debug.DrawLine(footPos.transform.position, hit.point, Color.red, 1);
            if (hit.collider.gameObject.layer == 6)
                return true;
        }
        return false;
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
}