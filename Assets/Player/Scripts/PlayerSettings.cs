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
        postProcess.weight = playerData.graphics > 1 ? 1 : 0.1f;

    }

    private void Update() {
        CinemachinePOV pov = camera.GetCinemachineComponent<CinemachinePOV>();

        pov.m_VerticalAxis.m_MaxSpeed = playerData.sensibility;
        pov.m_HorizontalAxis.m_MaxSpeed = playerData.sensibility;
    }
}