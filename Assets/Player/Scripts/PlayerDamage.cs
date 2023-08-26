using System.Collections;
using UnityEngine;
using Mirror;

public class PlayerDamage : NetworkBehaviour, IDamageable
{
    [SerializeField] PlayerComponents components;
    [SerializeField] PlayerMainController mainController;
    PlayerKillfeed killFeed => GetComponent<PlayerKillfeed>();
    MatchStatus matchStatus;

    void Start()
    {
        matchStatus = FindObjectOfType<MatchStatus>();
    }

    [Command(requiresAuthority = false)]
    public void CmdDamage(float damage, string killer_name, string killed_name) {
        RpcDamage(damage, killer_name, killed_name);
    }

    [ClientRpc]
    void RpcDamage(float damage, string killer_name, string killed_name) {
        components.playerHealth -= damage;
        if ( mainController.isLocalPlayerDead ) {
            if ( killer_name.Length > 0 ) {
                if ( components.playerTeam == 0 ) {
                    matchStatus.fire_score++;
                }
                else {
                    matchStatus.ice_score++;
                }
            }
            killFeed.RpcKillFeed(killer_name, killed_name);
            StartCoroutine(mainController.Respawn());
        }
    }
}
