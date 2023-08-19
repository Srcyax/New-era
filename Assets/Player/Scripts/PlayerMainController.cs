using Mirror;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerMainController : NetworkBehaviour, IDamageable {
    private LobbyPlayers lobbyManager;

    [Space(10)]

    [SerializeField] private Animator animator;
    [SerializeField] private Camera playerCamera;
    [SyncVar] public int playerTeam = -1;
    [SerializeField] public static Action shootInput;
    [SerializeField] public static Action reloadInput;
    [SerializeField] public static Action playerDied;
    [SyncVar] public float playerHealth = 100;

    [Header("Components to hide")]
    [SerializeField] private GameObject[] playerBody;
    [SerializeField] private GameObject[] playerObjs;

    [SerializeField] private TextMeshProUGUI healthTextPro;
    [SerializeField] private GameObject waitingPlayers;
    [SerializeField] private GameObject playerDeadUI;
    [SerializeField] private GameObject playerRagdoll;

    private GameObject[] spawnPoints;
    private Vector3 moveDirection = Vector3.zero;
    private Vector2 playerInput;
    private CharacterController characterController;

    private float lookXLimit = 80.0f;
    private float rotationX = 0;

    bool isRunning => Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift);
    private float shifitingSpeed = 8f;
    private float walkSpeed = 4f;

    void Start() {

        GameObject lobby = GameObject.FindGameObjectWithTag("Lobby");
        spawnPoints = playerTeam > 0 ? GameObject.FindGameObjectsWithTag("FIRE_SpawPoints") : GameObject.FindGameObjectsWithTag("ICE_SpawPoints");
        lobbyManager = FindObjectOfType<LobbyPlayers>();
        Destroy(lobby);

        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        playerCamera.enabled = isLocalPlayer && playerTeam >= 0;
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
        if ( !isLocalPlayer || playerHealth <= 0 )
            return;

        if ( lobbyManager.canStart && waitingPlayers )
            Destroy(waitingPlayers, 5f);

        if ( waitingPlayers || playerTeam < 0 )
            return;

        playerCamera.enabled = isLocalPlayer && playerTeam >= 0;

        PlayerControler();
        Animations();
        healthTextPro.text = playerHealth.ToString();

        if ( Input.GetMouseButton(0) ) {
            shootInput?.Invoke();
        }

        if ( Input.GetKeyDown(KeyCode.R) ) {
            reloadInput?.Invoke();
        }
    }


    [Command(requiresAuthority = false)]
    public void CmdDamage(float damage) {
        RpcDamage(damage);
    }

    [ClientRpc]
    void RpcDamage(float damage) {
        playerHealth -= damage;
        if ( playerHealth <= 0 ) {
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn() {
        animator.enabled = false;
        playerDeadUI.SetActive(true);
        int r = UnityEngine.Random.Range(spawnPoints.Length - spawnPoints.Length, spawnPoints.Length);
        yield return new WaitForSeconds(5.0f);
        animator.enabled = true;
        playerHealth = 100;
        transform.position = spawnPoints[ r ].transform.localPosition;
        playerDeadUI.SetActive(false);
        playerDied?.Invoke();
    }

    void PlayerControler() {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = (isRunning ? shifitingSpeed : walkSpeed) * Input.GetAxis("Vertical");
        float curSpeedY = (isRunning ? shifitingSpeed - Input.GetAxis("Horizontal") : walkSpeed) * Input.GetAxis("Horizontal");
        moveDirection = ( ( forward * Mathf.Clamp(curSpeedX, -8, 8) ) + ( right * Mathf.Clamp(curSpeedY, -5, 5) ) );

        moveDirection = Vector3.ClampMagnitude(moveDirection, 10.7f);

        if ( !characterController.isGrounded ) {
            moveDirection.y -= 20f; // gravity
        }

        characterController.Move(moveDirection * Time.deltaTime);
        rotationX += -Input.GetAxis("Mouse Y") * 3f;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * 3f, 0);

        if ( transform.localPosition.y <= -10 )
            CmdDamage(100);
    }

    float smoothing = 0.1f;
    float smoothInputX;
    float smoothInputY;

    void Animations() {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        smoothInputX = Mathf.Lerp(smoothInputX, horizontalInput, smoothing);
        smoothInputY = Mathf.Lerp(smoothInputY, isRunning ? 2 : verticalInput, smoothing);

        animator.SetFloat("inputX", smoothInputX);
        animator.SetFloat("inputY", smoothInputY);
    }
}