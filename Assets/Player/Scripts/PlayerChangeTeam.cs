using Mirror;
using UnityEngine;

public class PlayerChangeTeam : NetworkBehaviour {
    [SerializeField] PlayerComponents components;
    [SerializeField] TeamArrowSpawn teamArrow;
    GameObject chooseTeam;
    ChooseTeam team;
    void Start() {
        chooseTeam = GameObject.FindGameObjectWithTag("ChooseTeam");
        team = chooseTeam.GetComponent<ChooseTeam>();
    }

    void Update() {

        if ( !components.localPlayer )
            return;

        if ( components.spawning )
            return;

        if ( chooseTeam.activeSelf ) {
            teamArrow.ResetTeamArrow();
            return;
        }

        if ( Input.GetKeyDown(KeyCode.M) ) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            ChangeTeam();
            CmdChangeTeam();
        }
    }

    [Command(requiresAuthority =false)]
    void CmdChangeTeam() {
        if ( components.playerTeam == 0 ) {
            team.ice--;
        }
        else if ( components.playerTeam == 1 ) {
            team.fire--;
        }
        RpcChangeTeam();
    }

    [ClientRpc]
    void RpcChangeTeam() {
        components.playerTeam = -1;
    }

    void ChangeTeam() {
        chooseTeam.SetActive(true);
    }
}
