using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetPlayerSettings : MonoBehaviour
{
    [SerializeField] PlayerData playerData;

    [Header("UI components")]
    [SerializeField] Slider sensibility;
    [SerializeField] TMP_Dropdown graphics;

    public void SetSettings() {
        playerData.sensibility = sensibility.value;
        playerData.graphics = graphics.value;
    }
}
