using UnityEngine;
using Mirror;
using TMPro;
using System;
using System.Collections.Generic;
using System.Collections;

public class PlayerMainController : NetworkBehaviour, IDamageable
{
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;
    [SerializeField] private Animator animator;
    [SerializeField] private Camera playerCamera;
    [Header("Components to hide")]
    [SerializeField] private GameObject[] playerBody;
    [SerializeField] private GameObject[] playerObjs;
    private float lookXLimit = 80.0f;
    private float rotationX = 0;

    [SyncVar] public float playerHealth = 100;
    [SerializeField] TextMeshProUGUI healthTextPro;
    [SerializeField] GameObject playerRagdoll;
    GameObject[] spawnPoints;

    Vector2 playerInput;

    public static Action shootInput;
    public static Action reloadInput;

    void Start() {

        GameObject lobby = GameObject.FindGameObjectWithTag("Lobby");
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints");
        Destroy(lobby);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerCamera.enabled = isLocalPlayer;
        playerCamera.GetComponent<AudioListener>().enabled = isLocalPlayer;
        characterController = GetComponent<CharacterController>();
        for (int i = 0; i < playerObjs.Length; i++ ) {
            playerObjs[ i ].SetActive(isLocalPlayer);
        }

        for ( int i = 0; i < playerBody.Length; i++ ) {
            playerBody[ i ].SetActive(!isLocalPlayer);
        }
    }

    void Update() {
        if ( !isLocalPlayer || playerHealth <= 0)
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
        if ( playerHealth < 0 ) {
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn() {
        animator.enabled = false;
        yield return new WaitForSeconds(5.0f);
        playerHealth = 100;
        int r = UnityEngine.Random.Range(spawnPoints.Length - spawnPoints.Length, spawnPoints.Length);
        transform.localPosition = spawnPoints[ r ].transform.position;
        animator.enabled = true;
    }


    void PlayerControler() {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W) && 2 > 1;

        float curSpeedX = (isRunning ? 5f : 1.5f) * Input.GetAxis("Vertical");
        float curSpeedY = (isRunning ? 5f : 1.5f) * Input.GetAxis("Horizontal");
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

        if ( transform.localPosition.y < 0 )
            transform.localPosition = new Vector3(69.2f, 15.6f, 46f);
    }

    void Animations() {

        playerInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        animator.SetFloat("inputX", playerInput.x);
        animator.SetFloat("inputY", playerInput.y);
    }
}
