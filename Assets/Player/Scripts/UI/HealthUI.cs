using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {
    [Header("UI")]
    [SerializeField] Slider healthSlider;
    [SerializeField] TextMeshProUGUI healthUI;
    [SerializeField] public GameObject deadScreenUI;

    float healthHolder = 0;
    PlayerMainController mainController => GetComponent<PlayerMainController>();

    void Update() {
        healthHolder = Mathf.Lerp(healthHolder, mainController.playerHealth, Time.deltaTime * 5);

        healthUI.text = Mathf.RoundToInt(healthHolder).ToString();

        healthSlider.value = Mathf.RoundToInt(healthHolder);
    }
}