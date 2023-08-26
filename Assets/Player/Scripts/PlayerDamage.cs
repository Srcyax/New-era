using System.Collections;
using UnityEngine;
using Mirror;

public class PlayerDamage : NetworkBehaviour, IDamageable
{
    PlayerMainController player => GetComponent<PlayerMainController>();
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
        player.playerHealth -= damage;
        if ( player.isLocalPlayerDead ) {
            if ( killer_name.Length > 0 ) {
                if ( player.playerTeam == 0 ) {
                    matchStatus.fire_score++;
                }
                else {
                    matchStatus.ice_score++;
                }
            }
            killFeed.RpcKillFeed(killer_name, killed_name);
            StartCoroutine(player.Respawn());
        }
    }
}
