using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimations : MonoBehaviour {
    [Header("Player weapon")]
    [SerializeField] GetCurrentWeapon getCurrentWeapon;

    [Header("Player animator")]
    [SerializeField] public Animator animator;

    [Header("Player foot position")]
    [SerializeField] Transform footPos;

    [Header("Ground layer")]
    [SerializeField] int layerMask;

    [Header("Player main controller")]
    [SerializeField] PlayerMainController mainController;

    [Header("Player Damage")]
    [SerializeField] PlayerDamage damage;

    float smoothing = 8f;
    float smoothInputX;
    float smoothInputY;
    float smoothRifleState;


    string[] weapons ={
        "AK-103",
        "SCAR-L",
        "Pistol"
    };


    public void Animations() {
        Vector2 moveDir = mainController.inputActions.Player.Movement.ReadValue<Vector2>();

        smoothInputX = Mathf.Lerp(smoothInputX, moveDir.x, Time.deltaTime * smoothing);
        if ( isGrounded() ) {
            smoothInputY = Mathf.Lerp(smoothInputY, mainController.isPlayerRunning ? 2 : moveDir.y, Time.deltaTime * smoothing);
        }
        else {
            smoothInputY = Mathf.Lerp(smoothInputY, -2, Time.deltaTime * smoothing);
        }

        if ( mainController.characterController.velocity.magnitude > 0 ) {
            smoothRifleState = Mathf.Lerp(smoothRifleState, 1, Time.deltaTime * smoothing);
        }
        else {
            smoothRifleState = Mathf.Lerp(smoothRifleState, 0, Time.deltaTime * smoothing);
        }

        string weaponName = getCurrentWeapon.currentWeapon.GetComponent<WeaponInfo>().gunData.name;

        for (int i = 0; i < weapons.Length; i++) {
            if ( weapons[ i ] == weaponName )               
                continue;

            animator.SetBool(weapons[i], false);
        }

        animator.SetBool(weaponName, true);

        animator.SetFloat("inputX", smoothInputX);
        animator.SetFloat("inputY", smoothInputY);
        animator.SetFloat("rifle_state", smoothRifleState);
    }

    public bool isGrounded() {
        if ( Physics.Raycast(footPos.transform.position, footPos.transform.forward, out RaycastHit hit, .5f) ) {
            Debug.DrawLine(footPos.transform.position, hit.point, Color.red, 1);
            if ( hit.collider.gameObject.layer == layerMask ) {
                damage.SetFallDamage();
                damage.FallDamege(false);
                return true;
            }
        }
        damage.FallDamege(true);
        return false;
    }
}
