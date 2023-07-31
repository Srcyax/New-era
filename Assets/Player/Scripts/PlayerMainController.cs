using Mirror;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerMainController : NetworkBehaviour, IDamageable {
    [SerializeField] private Animator animator;
    [SerializeField] private Camera playerCamera;
    [SerializeField] public static Action shootInput;
    [SerializeField] public static Action reloadInput;
    [SyncVar] public float playerHealth = 100;

    [Header("Components to hide")]
    [SerializeField] private GameObject[] playerBody;
    [SerializeField] private GameObject[] playerObjs;
    
    [SerializeField] private TextMeshProUGUI healthTextPro;
    [SerializeField] private GameObject playerDeadUI;
    [SerializeField] private GameObject playerRagdoll;

    private GameObject[] spawnPoints;
    private Vector3 moveDirection = Vector3.zero;
    private Vector2 playerInput;
    private CharacterController characterController;

    private float lookXLimit = 80.0f;
    private float rotationX = 0;

    void Start() {

        GameObject lobby = GameObject.FindGameObjectWithTag("Lobby");
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints");
        Destroy(lobby);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerCamera.enabled = isLocalPlayer;
        playerCamera.GetComponent<AudioListener>().enabled = isLocalPlayer;
        characterController = GetComponent<CharacterController>();
        /*for ( int i = 0; i < playerObjs.Length; i++ ) {
            playerObjs[ i ].SetActive(isLocalPlayer);
        }

        for ( int i = 0; i < playerBody.Length; i++ ) {
            playerBody[ i ].SetActive(!isLocalPlayer);
        }*/
    }

    void Update() {
        if ( !isLocalPlayer || playerHealth <= 0 )
            return;

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
        transform.localPosition = spawnPoints[ r ].transform.localPosition;
        playerDeadUI.SetActive(false);
    }

    void PlayerControler() {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W) && 2 > 1;

        float curSpeedX = (isRunning ? 4f : 1.5f) * Input.GetAxis("Vertical");
        float curSpeedY = (isRunning ? 4f : 1.5f) * Input.GetAxis("Horizontal");
        moveDirection = ( ( forward * curSpeedX ) + ( right * curSpeedY ) );

        moveDirection = Vector3.ClampMagnitude(moveDirection, 10.7f);

        if ( !characterController.isGrounded ) {
            moveDirection.y -= 20f; // gravity
        }

        characterController.Move(moveDirection * Time.deltaTime);
        rotationX += -Input.GetAxis("Mouse Y") * 10f;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * 10f, 0);

        if ( transform.localPosition.y <= -10 )
            transform.localPosition = new Vector3(69.2f, 15.6f, 46f);
    }

    void Animations() {

        playerInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W);

        animator.SetFloat("inputX", playerInput.x);
        animator.SetFloat("inputY", isRunning ? 2 : playerInput.y);
    }
}