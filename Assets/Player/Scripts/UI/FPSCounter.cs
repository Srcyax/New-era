using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour {
    [SerializeField] TextMeshProUGUI Tfps;

    private float fpsUpdateInterval = 0.5f;
    private float accum = 0f;
    private int frames = 0;
    private float timeleft;

    private void Start() {
        timeleft = fpsUpdateInterval;
    }

    private void Update() {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        frames++;

        if ( timeleft <= 0.0 ) {
            float fps = accum / frames;
            string fpsText = string.Format("{0}", Mathf.Round(fps));
            Tfps.text = fpsText;

            timeleft = fpsUpdateInterval;
            accum = 0f;
            frames = 0;
        }
    }
}