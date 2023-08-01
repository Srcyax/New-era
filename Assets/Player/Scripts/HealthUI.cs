using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] PlayerMainController playerMain;

    [Header("UI")]
    [SerializeField] Slider healthSlider;
    [SerializeField] TextMeshProUGUI healthUI;

    float healthHolder = 0;

    void Update()
    {
        healthHolder = Mathf.Lerp(healthHolder, playerMain.playerHealth, Time.deltaTime * 5);

        healthUI.text = Mathf.RoundToInt(healthHolder).ToString();

        healthSlider.value = Mathf.RoundToInt(healthHolder);
    }
}