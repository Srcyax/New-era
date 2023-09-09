using UnityEngine;
using Cinemachine;

public class PlayerCameraLook : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    public PlayerMainController mainController;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 delta = mainController.inputActions.Player.Look.ReadValue<Vector2>();
    }
}
