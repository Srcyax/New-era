using UnityEngine;

public class PlayerAnimations : MonoBehaviour {
    [Header("Player animator")]
    [SerializeField] public Animator animator;

    [Header("Player foot position")]
    [SerializeField] Transform footPos;

    [Header("Ground layer")]
    [SerializeField] int layerMask;

    [Header("Player main controller")]
    [SerializeField] PlayerMainController mainController;

    float smoothing = 0.2f;
    float smoothInputX;
    float smoothInputY;
    float smoothRifleState;


    public void Animations() {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        smoothInputX = Mathf.Lerp(smoothInputX, horizontalInput, smoothing);
        if ( isGrounded() ) {
            smoothInputY = Mathf.Lerp(smoothInputY, mainController.isPlayerRunning ? 2 : verticalInput, smoothing);
        }
        else {
            smoothInputY = Mathf.Lerp(smoothInputY, -2, smoothing);
        }

        if ( mainController.characterController.velocity.magnitude > 0 ) {
            smoothRifleState = Mathf.Lerp(smoothRifleState, 1, smoothing);
        }
        else {
            smoothRifleState = Mathf.Lerp(smoothRifleState, 0, smoothing);
        }


        //animator.SetBool("crounch", Input.GetKey(KeyCode.LeftControl) && isGrounded() && !mainController.isPlayerRunning);

        animator.SetFloat("inputX", smoothInputX);
        animator.SetFloat("inputY", smoothInputY);
        animator.SetFloat("rifle_state", smoothRifleState);
    }

    public bool isGrounded() {
        if ( Physics.Raycast(footPos.transform.position, footPos.transform.forward, out RaycastHit hit, .5f) ) {
            Debug.DrawLine(footPos.transform.position, hit.point, Color.red, 1);
            if ( hit.collider.gameObject.layer == layerMask )
                return true;
        }
        return false;
    }
}
