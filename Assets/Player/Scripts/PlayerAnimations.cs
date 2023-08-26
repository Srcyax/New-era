using UnityEngine;

public class PlayerAnimations : MonoBehaviour {
    [Header("Player animator")]
    [SerializeField] public Animator animator;

    [Header("Player foot position")]
    [SerializeField] Transform footPos;

    [SerializeField] PlayerMainController mainController;
    float smoothing = 0.2f;
    float smoothInputX;
    float smoothInputY;

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

        animator.SetFloat("inputX", smoothInputX);
        animator.SetFloat("inputY", smoothInputY);
    }

    bool isGrounded() {
        if ( Physics.Raycast(footPos.transform.position, footPos.transform.forward, out RaycastHit hit, .5f) ) {
            Debug.DrawLine(footPos.transform.position, hit.point, Color.red, 1);
            if ( hit.collider.gameObject.layer == 6 )
                return true;
        }
        return false;
    }
}
