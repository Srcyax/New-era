using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvasUI : MonoBehaviour
{
    [SerializeField] PlayerMainController mainController;

    [Header("UI")]
    [SerializeField] GameObject canvas;

    void Update()
    {
        canvas.SetActive(mainController.playerHasTeam && !mainController.isLocalPlayerDead);
    }
}
