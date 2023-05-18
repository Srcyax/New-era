using UnityEngine;
using Mirror;

public class PlayerMainController : NetworkBehaviour
{
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;
    [SerializeField] private Animator animator;
    [SerializeField] private Camera playerCamera;
    private float lookXLimit = 80.0f;
    private float rotationX = 0;

    void Start() {
        GameObject lobby = GameObject.FindGameObjectWithTag("Lobby");
        {
            Destroy(lobby);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerCamera.enabled = isLocalPlayer;
        characterController = GetComponent<CharacterController>();
    }

    void Update() {
        if ( !isLocalPlayer )
            return;

        PlayerControler();
        Animations();
    }

    void PlayerControler() {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W) && 2 > 1;

        float curSpeedX = (isRunning ? 5f : 2f) * Input.GetAxis("Vertical");
        float curSpeedY = (isRunning ? 5f : 2f) * Input.GetAxis("Horizontal");
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

        bool walkForward = Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.LeftShift);
        bool walkBackward = Input.GetKey(KeyCode.S);
        bool walkStrafeRight = Input.GetKey(KeyCode.D);
        bool walkStrafeLeft = Input.GetKey(KeyCode.A);
        bool run = Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift);
        bool idle = !walkForward && !walkBackward && !walkStrafeRight && !walkStrafeLeft && !run;
        animator.SetBool("idle", idle);
        animator.SetBool("walk", walkForward);
        animator.SetBool("run", run);
    }
}
