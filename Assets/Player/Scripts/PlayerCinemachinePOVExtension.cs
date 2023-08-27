using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCinemachinePOVExtension : CinemachineExtension
{
    [SerializeField] private float horizontalSpeed = 10f;
    [SerializeField] private float verticalSpeed = 10f;
    [SerializeField] private float clampAngle = 80f;

    [SerializeField] public WeaponSystem WeaponSystem;

    public Vector3 startingRotation;

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime) {
        if ( vcam.Follow ) {
            if (stage == CinemachineCore.Stage.Aim ) {
                Vector2 deltaInput = WeaponSystem.cameraAxis;
                startingRotation.x += deltaInput.x * verticalSpeed * Time.deltaTime;
                startingRotation.y += deltaInput.y * horizontalSpeed * Time.deltaTime;
                startingRotation.y = Mathf.Clamp(startingRotation.y, -clampAngle, clampAngle);
                state.RawOrientation.eulerAngles += new Vector3(startingRotation.y, startingRotation.x, 0f);
            }
        }
    }
}