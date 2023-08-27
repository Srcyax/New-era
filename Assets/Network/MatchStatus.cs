using Mirror;
using System.Collections;
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

    [Header("Winner settings")]
    [SerializeField] GameObject ice;
    [SerializeField] GameObject fire;
    [SerializeField] GameObject draw;

    [SyncVar] public int ice_score;
    [SyncVar] public int fire_score;
    [SyncVar] public float matchTime;

    [HideInInspector]
    [SyncVar] public float minutes, seconds;

    void FixedUpdate() {
        PlayerMainController[] players = FindObjectsOfType<PlayerMainController>();

        foreach (PlayerMainController player in players) {
            if ( !player.playerHasTeam )
                return;

            CmdUISettings(ice_score, fire_score);

            if ( matchTime > 0 ) {
                matchTime -= Time.fixedDeltaTime;

                minutes = Mathf.FloorToInt(Mathf.Round(matchTime) / 60);
                seconds = Mathf.FloorToInt(Mathf.Round(matchTime) % 60);

                string timerText = string.Format("{0:0}:{1:00}", Mathf.Round(minutes), Mathf.Round(seconds));
                tTime.text = timerText;
            }
            else {
                if ( ice_score > fire_score ) {
                    StartCoroutine(Winner(ice));
                }
                else if ( fire_score > ice_score ) {
                    StartCoroutine(Winner(fire));
                }
                else {
                    StartCoroutine(Winner(draw));
                }
            }
        }
    }

    IEnumerator Winner(GameObject winner) {
        Time.timeScale = .1f;
        winner.SetActive(true);
        yield return new WaitForSeconds(1f);
        winner.SetActive(false);
        ice_score = 0;
        fire_score = 0;
        matchTime = 160;
        Time.timeScale = 1;
        PlayerDamage[] players = FindObjectsOfType<PlayerDamage>();
        foreach(PlayerDamage player in players) {
            player.CmdDamage(100, "", "", "");
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