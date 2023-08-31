using Mirror;
using UnityEngine;

public class PlayerChangeTeam : NetworkBehaviour {
    [SerializeField] PlayerComponents components;
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

        if ( Input.GetKeyDown(KeyCode.M) ) {
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
        if ( !chooseTeam.activeSelf )
            return;

        components.playerTeam = -1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void ChangeTeam() {
        chooseTeam.SetActive(true);
    }
}
