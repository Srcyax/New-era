using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraLean : MonoBehaviour
{
    [Header("Main controller")]
    [SerializeField] PlayerMainController mainController;
    [Space(10)]

    [Header("Camera components")]
    [SerializeField] new Cinemachine.CinemachineVirtualCamera camera;
    [SerializeField] Transform cameraPos;

    float smoothX;
    void Update()
    {
        if (mainController.isLocalPlayerDead) 
            return;

        if (Input.GetKey(KeyCode.Q)) {
            camera.m_Lens.Dutch = Mathf.Lerp(camera.m_Lens.Dutch, 15, .1f);
            smoothX = Mathf.Lerp(smoothX, -.25f, .1f);
            cameraPos.transform.localPosition = new Vector3(smoothX, 0.603f, 0.122f);
        }
        else if (Input.GetKey(KeyCode.E)) {
            camera.m_Lens.Dutch = Mathf.Lerp(camera.m_Lens.Dutch, -15, .1f);
            smoothX = Mathf.Lerp(smoothX, .25f, .1f);
            cameraPos.transform.localPosition = new Vector3(smoothX, 0.603f, 0.122f);
        }
        else {
            camera.m_Lens.Dutch = Mathf.Lerp(camera.m_Lens.Dutch, 0, .1f);
            smoothX = Mathf.Lerp(smoothX, 0, .1f);
            cameraPos.transform.localPosition = new Vector3(smoothX, 0.603f, 0.122f);
        }     
    }
}
