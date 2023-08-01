using UnityEngine;

public class CameraBob : MonoBehaviour {
    [SerializeField] float walkBobFrequency = 1.5f;
    [SerializeField] float walkBobAmount = 0.05f;
    [SerializeField] float runBobFrequency = 3.0f;
    [SerializeField] float runBobAmount = 0.1f;
    [SerializeField] float midpoint = 2.0f;
    [SerializeField] float smoothTime = 0.1f;
    [SerializeField] float rotationBobAmount = 1.0f;

    [SerializeField] CharacterController characterController;

    private float timer = 0.0f;
    private float smoothVelocityPosition = 0.0f; 
    private bool isPlayerMoving = false;
    private bool isPlayerRunning = false;
    private bool isStartingMovement = true;

    private void Update() {
        isPlayerMoving = characterController.velocity.magnitude > 0.1f;
        isPlayerRunning = Input.GetKey(KeyCode.LeftShift);

        float currentBobFrequency = isPlayerRunning ? runBobFrequency : walkBobFrequency;
        float currentBobAmount = isPlayerRunning ? runBobAmount : walkBobAmount;

        if ( isPlayerMoving ) {
            timer += Time.deltaTime * currentBobFrequency;

            if ( isStartingMovement ) {
                float smoothStartBob = Mathf.SmoothDamp(0f, currentBobAmount, ref smoothVelocityPosition, smoothTime);
                transform.localPosition = new Vector3(transform.localPosition.x, midpoint + smoothStartBob, transform.localPosition.z);
                isStartingMovement = false;
            }
        }
        else {
            timer = 0f;
            isStartingMovement = true;
        }

        float verticalBob = Mathf.Sin(timer) * currentBobAmount;

        float rotationBobX = Mathf.Sin(timer) * rotationBobAmount;
        float rotationBobY = Mathf.Sin(timer * 1.2f) * rotationBobAmount;
        float rotationBobZ = Mathf.Sin(timer * 0.8f) * rotationBobAmount;

        Vector3 newPosition = transform.localPosition;
        newPosition.y = midpoint + verticalBob;
        transform.localPosition = newPosition;

        Vector3 newRotation = transform.localEulerAngles;
        newRotation.x = midpoint + rotationBobX;
        newRotation.y = midpoint + rotationBobY;
        newRotation.z = midpoint + rotationBobZ;
        transform.localEulerAngles = newRotation;
    }
}