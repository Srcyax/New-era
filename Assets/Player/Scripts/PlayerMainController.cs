using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMainController : MonoBehaviour {
    public static Action shootInput;
    public static Action reloadInput;
    public static Action playerDied;

    [Header("Player components")]
    [SerializeField] PlayerData playerData;
    [SerializeField] PlayerComponents components;
    [SerializeField] PlayerAnimations playerAnimations;
    [SerializeField] PlayerDamage playerDamage;
    [SerializeField] HealthUI playerHealthUI;
    [SerializeField] NameUI playerNameUI;
    [SerializeField] TeamArrowSpawn teamArrow;
    [SerializeField] Transform playerHeadLookAt;
    [SerializeField] public Camera playerCamera;

    [Header("Weapon settings")]
    [SerializeField] RecoilSystem recoilSystem;

    private GameObject[] spawnPoints;
    private Vector3 moveDirection = Vector3.zero;
    [HideInInspector]
    public CharacterController characterController;

    private GameObject waitingPlayers;
    private LobbyPlayers lobbyManager;

    void Start() {
        GameObject lobby = GameObject.FindGameObjectWithTag("Lobby");
        waitingPlayers = GameObject.FindGameObjectWithTag("WaitingPlayersCanvas");
        lobbyManager = FindObjectOfType<LobbyPlayers>();
        characterController = GetComponent<CharacterController>();
        Destroy(lobby);
        StartCoroutine(chooseTeam());

        if (components.localPlayer )
            playerNameUI.CmdSetPlayerName(playerData.name);
    }

    void Update() {
        if ( !components.localPlayer )
            return;

        if ( isLocalPlayerDead ) {
            playerDied?.Invoke();
            return;
        }

        if ( lobbyManager.canStart && waitingPlayers )
            Destroy(waitingPlayers);

        if ( isWaitingForPlayers )
            return;

        if ( !playerHasTeam )
            return;

        if ( isPlayerInSettingsMenu )
            return;

        teamArrow.SetTeamArrow(components.playerTeam);
        PlayerControler();
        playerAnimations.Animations();
        if ( Input.GetMouseButton(0) ) {
            shootInput?.Invoke();
        }
        else {
            recoilSystem.Reset();
        }

        if ( Input.GetKeyDown(KeyCode.R) ) {
            reloadInput?.Invoke();
        }
    }

    IEnumerator chooseTeam() {
        GameObject chooseTeam = GameObject.FindGameObjectWithTag("ChooseTeam");
        yield return new WaitForSeconds(.3f);
        chooseTeam.SetActive(false);
        yield return new WaitForSeconds(.1f);
        chooseTeam.SetActive(true);
    }

    public IEnumerator Respawn() {
        spawnPoints = components.playerTeam == 1 ? GameObject.FindGameObjectsWithTag("FIRE_SpawPoints") : GameObject.FindGameObjectsWithTag("ICE_SpawPoints");
        playerAnimations.animator.enabled = false;
        playerHealthUI.deadScreenUI.SetActive(true);
        characterController.enabled = false;
        yield return new WaitForSeconds(3.0f);
        int r = UnityEngine.Random.Range(0, spawnPoints.Length);
        transform.position = spawnPoints[ r ].transform.position;
        yield return new WaitForSeconds(.5f);
        characterController.enabled = true;
        playerAnimations.animator.enabled = true;
        components.playerHealth = 100;
        playerHealthUI.deadScreenUI.SetActive(false);
        playerDamage.playerWeapon.SetActive(true);
        components.CmdSpawning();
    }

    void PlayerControler() {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = (isPlayerRunning ? components.playerRunSpeed : components.playerWalkSpeed) * Input.GetAxis("Vertical");
        float curSpeedY = (isPlayerRunning ? components.playerRunSpeed - Input.GetAxis("Horizontal") : components.playerWalkSpeed) * Input.GetAxis("Horizontal");
        moveDirection = ( ( forward * Mathf.Clamp(curSpeedX, -components.playerRunSpeed, components.playerRunSpeed) ) + ( right * Mathf.Clamp(curSpeedY, -components.playerWalkSpeed, components.playerWalkSpeed) ) );

        moveDirection = Vector3.ClampMagnitude(moveDirection, 10.7f);

        if ( !characterController.isGrounded ) {
            moveDirection.y -= 10; // gravity
        }

        characterController.Move(moveDirection * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, playerCamera.transform.eulerAngles.y, 0);

        if ( transform.localPosition.y <= -10 )
            playerDamage.CmdDamage(100, "", "", "");
    }

    public bool playerHasTeam {
        get { return components.playerTeam is 0 or 1; }
    }

    public bool isLocalPlayerDead {
        get { return components.playerHealth <= 0; }
    }

    public bool isPlayerInSettingsMenu;

    public bool isWaitingForPlayers {
        get { return waitingPlayers; }
    }

    public bool isPlayerRunning {
        get {
            return Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift);
        }
    }
}