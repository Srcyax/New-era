using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {
    [Header("UI")]
    [SerializeField] Slider healthSlider;
    [SerializeField] TextMeshProUGUI healthUI;
    [SerializeField] public GameObject deadScreenUI;
    [SerializeField] PlayerComponents components;

    float healthHolder = 0;

    void Update() {
        if ( !components.localPlayer )
            return;

        healthHolder = Mathf.Lerp(healthHolder, components.playerHealth, Time.deltaTime * 5);

        healthUI.text = Mathf.RoundToInt(healthHolder).ToString();

        healthSlider.value = Mathf.RoundToInt(healthHolder);
    }
}