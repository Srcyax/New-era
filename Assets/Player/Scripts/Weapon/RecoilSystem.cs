using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RecoilSystem : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera playerCamera;
    public WeaponSystem weaponSystem;
    [HideInInspector] public Cinemachine.CinemachineImpulseSource cameraShake;
    [SerializeField] PlayerCinemachinePOVExtension extension;

    public Vector2[] recoilPatern;
    public float duration;

    float verticalRecoil;
    float horizontalRecoil;

    float time;
    int index;

    private void Awake() {
        cameraShake = GetComponent<CinemachineImpulseSource>();
    }

    public void Reset() {
        index = 0;
        ResetCamPos();
    }

    int NextIndex(int index) {
        return (index + 1) % recoilPatern.Length;
    }

    public void GenerateRecoil() {
        time = duration;

        cameraShake.GenerateImpulse(Camera.main.transform.forward);

        horizontalRecoil = recoilPatern[ index ].x;
        verticalRecoil = recoilPatern[ index ].y;

        index = NextIndex(index);
    }

    private void Update() {
        if (time > 0 ) {
            weaponSystem.cameraAxis.y -= ( ( verticalRecoil / 1000 ) * Time.deltaTime ) / duration;
            weaponSystem.cameraAxis.x -= ( ( horizontalRecoil / 10 ) * Time.deltaTime ) / duration;

            time -= Time.deltaTime;
        }
        else {
            weaponSystem.cameraAxis = Vector2.zero;
            ResetCamPos();
        }
    }

    void ResetCamPos() {
        extension.startingRotation.y = Mathf.Lerp(extension.startingRotation.y, 0, Time.deltaTime);
        extension.startingRotation.x = Mathf.Lerp(extension.startingRotation.x, 0, Time.deltaTime);
    }
}
