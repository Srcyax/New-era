using Mirror;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerDamage : NetworkBehaviour, IDamageable {
    [SerializeField] PlayerComponents components;
    [SerializeField] PlayerMainController mainController;
    [SerializeField] PlayerFootsteps footsteps;

    [Header("UI settings")]
    [SerializeField] Transform canvas;
    [SerializeField] GameObject blood;

    [Header("Camera Post Process")]
    [SerializeField] PostProcessVolume processVolume;

    Vignette vignette;

    Feed feed;
    MatchStatus matchStatus;

    [HideInInspector]
    public float fallDamage;

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
    public void CmdDamage(float damage, PlayerComponents killer_name, PlayerComponents killed_name, string reason) {
        RpcDamage(damage, killer_name, killed_name, reason);
    }

    [ClientRpc]
    void RpcDamage(float damage, PlayerComponents killer_name, PlayerComponents killed_name, string reason) {
        components.playerHealth -= damage;
        if ( mainController.isLocalPlayerDead ) {
            if ( killer_name ) {
                feed.KillFeed(killer_name.playerName, killed_name ? killed_name.playerName : "", reason);
                if ( killer_name.playerHealth > 0 )
                    killer_name.playerHealth = 100;
                if ( components.playerTeam == 0 ) {
                    matchStatus.fire_score++;
                    return;
                }
                else {
                    matchStatus.ice_score++;
                    return;
                }
            }
        }
        else if ( !mainController.isLocalPlayerDead && damage > 1f ) {
            Instantiate(blood, canvas);
        }
    }

    float time;

    public void SetFallDamage() {
        if ( fallDamage <= 0 )
            return;

        CmdDamage(fallDamage, components, null, "fell from a high place");
        footsteps.LandFootSound();
        fallDamage = 0;
        time = 0;
    }

    public void FallDamege() {
        time += Time.deltaTime * 3f;
        if ( time > 1 ) {
            fallDamage += Time.deltaTime * 20f;
        }
    }
}