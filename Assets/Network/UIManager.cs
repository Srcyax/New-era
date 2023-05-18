using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button HostButton;
    [SerializeField] private Button ClientButton;

    [SerializeField] private TMP_Dropdown maxClient;
    [SerializeField] private NetworkManager transport;
    [SerializeField] private TextMeshProUGUI ipAdress;

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
    private void Update()
    {
        HostButton.interactable =  ipAdress.text.Length > 1;
        ClientButton.interactable = ipAdress.text.Length > 1;
    }
}