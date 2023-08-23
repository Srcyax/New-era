using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] PlayerData playerData;
    [SerializeField] private Button HostButton;
    [SerializeField] private Button ClientButton;

    [SerializeField] private TMP_Dropdown maxClient;
    [SerializeField] private NetworkManager transport;
    [SerializeField] private TextMeshProUGUI ipAdress;
    [SerializeField] private TextMeshProUGUI playerName;

    private void Start()
    {
        HostButton?.onClick.AddListener( () => {
            if ( ( maxClient.value + 1 ) > 1 ) {
                NetworkManager.singleton.StartHost();
                transport.maxConnections = maxClient.value;
            }
        } );

        ClientButton?.onClick.AddListener( () => {
            NetworkManager.singleton.StartClient();
        } );
    }
    private void Update() {
        playerData.name = playerName.text;
        HostButton.interactable =  ipAdress.text.Length > 1 && playerName.text.Length > 1;
        ClientButton.interactable = ipAdress.text.Length > 1 && playerName.text.Length > 1;
    }
}