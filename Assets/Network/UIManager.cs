using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
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

    private void Start()
    {
        jsonSystem.SettingsDataLoadFromJson(sensibility, graphics, playerName);

        QualitySettings.SetQualityLevel(graphics.value);

        HostButton?.onClick.AddListener( () => {
            if ( ( maxClient.value + 1 ) > 1 ) {
                try {
                    NetworkManager.singleton.StartHost();
                    NetworkManager.singleton.OnStartHost();
                    transport.maxConnections = maxClient.value;
                }
                catch {
                    ConnectionStatus("error creating server.");
                }
            }
        } );

        ClientButton?.onClick.AddListener( () => {
            try {
                NetworkManager.singleton.StartClient();
                NetworkManager.singleton.authenticator.OnStartClient();
            } 
            catch {
                ConnectionStatus("connection error.");
            }   
        } );
    }

    void ConnectionStatus(string status) {
        if ( connectionTransform.childCount > 0 ) {
            for ( int i = 0; i < connectionTransform.childCount; i++ ) {
                Destroy(connectionTransform.GetChild(i).gameObject);
            }
            GameObject conn = Instantiate(connectionStatus, connectionTransform);
            conn.GetComponent<TextMeshProUGUI>().text = status;
        }
        else {
            GameObject conn = Instantiate(connectionStatus, connectionTransform);
            conn.GetComponent<TextMeshProUGUI>().text = status;
        }
    }

    private void Update() {
        playerData.name = playerName.text;
        HostButton.interactable =  ipAdress.text.Length > 1 && playerName.text.Length > 1;
        ClientButton.interactable = ipAdress.text.Length > 1 && playerName.text.Length > 1;
    }
}