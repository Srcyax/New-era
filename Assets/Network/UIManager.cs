using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Net;

public class UIManager : MonoBehaviour {
    [SerializeField] PlayerData playerData;
    [SerializeField] JsonSaveSystem jsonSystem;

    [Header("Buttons")]
    [SerializeField] Button HostButton;
    [SerializeField] Button ClientButton;

    [SerializeField] TMP_Dropdown maxClient;
    [SerializeField] NetworkManager transport;
    [SerializeField] TextMeshProUGUI ipAdress;
    [SerializeField] TMP_InputField playerName;

    [Header("Network status")]
    [SerializeField] GameObject connectionStatus;
    [SerializeField] Transform connectionTransform;

    [Header("UI components")]
    [SerializeField] Slider sensibility;
    [SerializeField] TMP_Dropdown graphics;

    [Header("Player status components")]
    [SerializeField] TextMeshProUGUI kills;
    [SerializeField] TextMeshProUGUI deaths;


    private void Start() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        jsonSystem.SettingsDataLoadFromJson(sensibility, graphics, playerName);
        //jsonSystem.StatusDataLoadFromJson(playerData);

        QualitySettings.SetQualityLevel(graphics.value);

        HostButton?.onClick.AddListener(() => {
            if ( ( maxClient.value + 1 ) > 1 ) {
                NetworkManager.singleton.StartHost();
                transport.maxConnections = maxClient.value;
                jsonSystem.SettingsDataSaveToJson(sensibility.value, graphics.value, playerName.text);
            }
        });

        ClientButton?.onClick.AddListener(() => {
            NetworkManager.singleton.StartClient();
            jsonSystem.SettingsDataSaveToJson(sensibility.value, graphics.value, playerName.text);
        });
    }

    private void Update() {
        playerData.name = playerName.text;

        kills.text = playerData.kills.ToString();
        deaths.text = playerData.deaths.ToString();

        HostButton.interactable = ipAdress.text.Length > 1 && playerName.text.Length > 1;
        ClientButton.interactable = ipAdress.text.Length > 1 && playerName.text.Length > 1;
    }
}