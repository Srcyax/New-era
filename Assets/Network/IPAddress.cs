using Mirror;
using TMPro;
using UnityEngine;

public class IPAddress : MonoBehaviour {
    [SerializeField] TMP_InputField ipText;
    [SerializeField] NetworkManager transport;

    void Update() {
        transport.networkAddress = ipText.text;
    }
}