using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerSettings : MonoBehaviour
{
    [SerializeField] PlayerData playerData;

    [Header("Camera")]
    [SerializeField] new CinemachineVirtualCamera camera;

    [Header("Postprocess")]
    [SerializeField] PostProcessVolume postProcess;

    private void Start() {
        QualitySettings.SetQualityLevel(playerData.graphics);

        SetPlayerSettings(playerData.sensibility, playerData.sensibility, playerData.graphics);
    }

    private void Update() {

    }

    public void SetPlayerSettings(float senseX, float senseY, int graphics) {
        CinemachinePOV pov = camera.GetCinemachineComponent<CinemachinePOV>();

        pov.m_VerticalAxis.m_MaxSpeed = senseX;
        pov.m_HorizontalAxis.m_MaxSpeed = senseY;

        postProcess.weight = graphics > 1 ? 1 : 0.1f;
    }
}