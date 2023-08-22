using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchStatus : NetworkBehaviour {
    [Header("UI settings")]
    [SerializeField] Slider sliderIce;
    [SerializeField] Slider sliderFire;
    [SerializeField] TextMeshProUGUI tIce;
    [SerializeField] TextMeshProUGUI tFire;
    [SerializeField] TextMeshProUGUI tTime;

    [SyncVar] public int ice_score;
    [SyncVar] public int fire_score;
    [SyncVar] public float matchTime;

    [HideInInspector]
    [SyncVar] public float minutes, seconds;

    void Update() {
        PlayerMainController[] players = FindObjectsOfType<PlayerMainController>();

        foreach (PlayerMainController player in players) {
            if ( !player.playerHasTeam )
                return;

            CmdUISettings(ice_score, fire_score);

            if ( matchTime > 0 ) {
                matchTime -= Time.fixedDeltaTime * .5f;

                minutes = Mathf.FloorToInt(Mathf.Round(matchTime) / 60);
                seconds = Mathf.FloorToInt(Mathf.Round(matchTime) % 60);

                string timerText = string.Format("{0:0}:{1:00}", Mathf.Round(minutes), Mathf.Round(seconds));
                tTime.text = timerText;
            }
        }
    }

    [Command(requiresAuthority = false)]
    void CmdUISettings(int ice_score, int fire_score) {
        RpcUISettings(ice_score, fire_score);
    }

    [ClientRpc]
    void RpcUISettings(int ice_score, int fire_score) {
        sliderIce.value = ice_score;
        sliderFire.value = fire_score;

        tIce.text = ice_score.ToString();
        tFire.text = fire_score.ToString();
    }
}