using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSettingsMenu : MonoBehaviour
{
    [Header("Player components")]
    [SerializeField] PlayerMainController mainController;
    [SerializeField] PlayerComponents components;
    [SerializeField] PlayerSettings playerSettings;
    [SerializeField] PlayerData playerData;
    JsonSaveSystem json;

    [SerializeField] GameObject playerUI;
    [SerializeField] GameObject settingsUI;

    [Header("UI components")]
    [SerializeField] Slider sensibility;
    [SerializeField] TMP_Dropdown graphics;

    void Start()
    {
        json = GameObject.FindObjectOfType<JsonSaveSystem>();

        sensibility.value = playerData.sensibility;
        graphics.value = playerData.graphics;
    }

    void Update()
    {
        if ( !components.localPlayer )
            return;

        if ( !mainController.playerHasTeam )
            return;

        if (Input.GetKeyDown(KeyCode.Escape)) {
            playerUI.SetActive(!playerUI.activeSelf);
            settingsUI.SetActive(!settingsUI.activeSelf);
            if ( settingsUI.activeSelf ) {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                playerSettings.SetPlayerSettings(0, 0, graphics.value);
            }
            else {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                playerSettings.SetPlayerSettings(sensibility.value, sensibility.value, graphics.value);
                json.SettingsDataSaveToJson(sensibility.value, graphics.value, components.playerName);
                QualitySettings.SetQualityLevel(graphics.value);
            }

            mainController.isPlayerInSettingsMenu = !mainController.isPlayerInSettingsMenu;
        }
    }
}
