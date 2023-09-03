using System.ComponentModel;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerDied : MonoBehaviour {
    [Header("Player components")]
    [SerializeField] PlayerComponents components;
    [SerializeField] PlayerMainController mainController;

    [Header("Camera components")]
    [SerializeField] ParentConstraint cameraPos;
    [SerializeField] GameObject[] objsToDisable;
    [SerializeField] GameObject[] objsToEnable;

    float smooth;
    bool respawn;
    void Update() {
        if ( !mainController.isLocalPlayerDead ) {
            Reset();
            return;
        }

        foreach ( var obj in objsToDisable ) {
            obj.SetActive(false);
        }
        foreach ( var obj in objsToEnable ) {
            obj.layer = 8;
        }
        smooth = Mathf.Lerp(smooth, 1f, Time.deltaTime);
        cameraPos.weight = smooth;
        if ( respawn ) {
            StartCoroutine(mainController.Respawn());
            respawn = false;
        }
    }

    private void Reset() {
        respawn = true;
        cameraPos.weight = 0f;
        foreach ( var obj in objsToDisable ) {
            obj.SetActive(true);
        }
        foreach ( var obj in objsToEnable ) {
            obj.layer = components.localPlayer ? 11 : 8;
        }
    }
}