using TMPro;
using Mirror;
using UnityEngine;

public class IPAddress : MonoBehaviour
{
    [SerializeField] TMP_InputField ipText;
    [SerializeField] NetworkManager transport;

    void Update()
    {
        transport.networkAddress = ipText.text;
    }
}