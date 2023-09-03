using Mirror;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerDamage : NetworkBehaviour, IDamageable {
    [SerializeField] PlayerComponents components;
    [SerializeField] PlayerMainController mainController;

    [Header("UI settings")]
    [SerializeField] Transform canvas;
    [SerializeField] GameObject blood;

    [Header("Camera Post Process")]
    [SerializeField] PostProcessVolume processVolume;

    Vignette vignette;

    Feed feed;
    MatchStatus matchStatus;

    void Start() {
        matchStatus = FindObjectOfType<MatchStatus>();

        vignette = processVolume.profile.GetSetting<Vignette>();

        feed = FindObjectOfType<Feed>();
    }

    void Update() {
        if ( !isLocalPlayer )
            return;

        CameraDamageEffect();
    }

    void CameraDamageEffect() {
        if ( components.playerHealth < 40 ) {
            vignette.intensity.value = Mathf.Lerp(vignette.intensity, 0.4f, Time.deltaTime * 3f);
        }
        else {
            vignette.intensity.value = Mathf.Lerp(vignette.intensity, 0.15f, Time.deltaTime * 3f);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdDamage(float damage, string killer_name, string killed_name, string reason) {
        RpcDamage(damage, killer_name, killed_name, reason);
    }

    [ClientRpc]
    void RpcDamage(float damage, string killer_name, string killed_name, string reason) {
        components.playerHealth -= damage;
        if ( mainController.isLocalPlayerDead ) {
            if ( killer_name.Length > 0 ) {
                if ( components.playerTeam == 0 ) {
                    matchStatus.fire_score++;
                }
                else {
                    matchStatus.ice_score++;
                }
                feed.KillFeed(killer_name, killed_name, reason);
            }
            if ( reason != "" ) {
                components.deaths++;
            }
        }
        else {
            Instantiate(blood, canvas);
        }
    }
}
