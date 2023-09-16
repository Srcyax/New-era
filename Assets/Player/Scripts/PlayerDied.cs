using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerDied : MonoBehaviour {
    [Header("Player components")]
    [SerializeField] PlayerComponents components;
    [SerializeField] PlayerMainController mainController;
    [SerializeField] PlayerAnimations playerAnimations;
    [SerializeField] HealthUI playerHealthUI;

    [Header("Camera components")]
    [SerializeField] ParentConstraint cameraPos;
    [SerializeField] GameObject[] objsToDisable;
    [SerializeField] GameObject[] objsToEnable;

    private GameObject[] spawnPoints;

    float smooth;
    bool respawn;

    void Update() {
        if ( !mainController.isLocalPlayerDead ) {
            Reset();
            return;
        }

        if ( components.localPlayer )
            PlayerMainController.playerDied?.Invoke();

        foreach ( var obj in objsToDisable ) {
            obj.SetActive(false);
        }
        foreach ( var obj in objsToEnable ) {
            obj.layer = 8;
        }

        smooth = Mathf.Lerp(smooth, 1f, Time.deltaTime);
        cameraPos.constraintActive = true;
        cameraPos.weight = smooth;

        if ( respawn ) {
            StartCoroutine(Respawn());
            respawn = false;
        }
    }

    public IEnumerator Respawn() {
        spawnPoints = components.playerTeam == 1 ? GameObject.FindGameObjectsWithTag("FIRE_SpawPoints") : GameObject.FindGameObjectsWithTag("ICE_SpawPoints");
        playerAnimations.animator.enabled = false;
        mainController.characterController.enabled = false;
        yield return new WaitForSeconds(3.0f);
        playerHealthUI.deadScreenUI.SetActive(true);
        int r = Random.Range(0, spawnPoints.Length);
        transform.position = spawnPoints[ r ].transform.position;
        yield return new WaitForSeconds(.5f);
        mainController.characterController.enabled = true;
        playerAnimations.animator.enabled = true;
        components.playerHealth = 100;
        playerHealthUI.deadScreenUI.SetActive(false);
        components.CmdSpawning();
    }

    private void Reset() {
        respawn = true;
        cameraPos.constraintActive = false;
        cameraPos.weight = 0f;

        foreach ( var obj in objsToDisable ) {
            obj.SetActive(true);
        }

        foreach ( var obj in objsToEnable ) {
            obj.layer = components.localPlayer ? 11 : 8;
        }
    }
}