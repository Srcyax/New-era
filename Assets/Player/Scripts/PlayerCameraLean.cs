using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraLean : MonoBehaviour
{
    [Header("Player components")]
    [SerializeField] PlayerComponents components;
    [SerializeField] PlayerMainController mainController;
    [SerializeField] Animator animator;
    [Space(10)]

    [Header("Camera components")]
    [SerializeField] new Cinemachine.CinemachineVirtualCamera camera;
    [SerializeField] Transform cameraPos;

    float smoothing = 8f;
    float smoothX;
    float smoothBodyLean;
    void Update()
    {
        if ( !components.localPlayer )
            return;

        if ( mainController.isLocalPlayerDead || mainController.isPlayerRunning ) {
            Reset();
            return;
        }

        if (Input.GetKey(KeyCode.Q)) {
            smoothBodyLean = Mathf.Lerp(smoothBodyLean, 1, Time.deltaTime * smoothing);
            camera.m_Lens.Dutch = Mathf.Lerp(camera.m_Lens.Dutch, 15, Time.deltaTime * smoothing);
            smoothX = Mathf.Lerp(smoothX, -.25f, Time.deltaTime * smoothing);
            cameraPos.transform.localPosition = new Vector3(smoothX, 0.603f, 0.122f);
        }
        else if (Input.GetKey(KeyCode.E)) {
            smoothBodyLean = Mathf.Lerp(smoothBodyLean, -1, Time.deltaTime * smoothing);
            camera.m_Lens.Dutch = Mathf.Lerp(camera.m_Lens.Dutch, -15, Time.deltaTime * smoothing);
            smoothX = Mathf.Lerp(smoothX, .25f, Time.deltaTime * smoothing);
            cameraPos.transform.localPosition = new Vector3(smoothX, 0.603f, 0.122f);
        }
        else {
            Reset();
        }

        animator.SetFloat("lean", smoothBodyLean);
    }

    private void Reset() {
        camera.m_Lens.Dutch = Mathf.Lerp(camera.m_Lens.Dutch, 0, Time.deltaTime * smoothing);
        smoothX = Mathf.Lerp(smoothX, 0, Time.deltaTime * smoothing);
        smoothBodyLean = Mathf.Lerp(smoothBodyLean, 0, Time.deltaTime * smoothing);
        animator.SetFloat("lean", smoothBodyLean);
        cameraPos.transform.localPosition = new Vector3(smoothX, 0.603f, 0.122f);
    }
}
