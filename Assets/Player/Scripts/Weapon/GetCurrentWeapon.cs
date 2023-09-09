using Mirror;
using System.Collections;
using UnityEngine;

public class GetCurrentWeapon : MonoBehaviour
{
    [Header("Player components")]
    [SerializeField] PlayerComponents components;

    [Header("Weapon system")]
    [SerializeField] WeaponSystem system;

    [Header("Current weapon")]
    public GameObject currentWeapon;

    [Header("Player Weapons")]
    [SerializeField] Transform weapons;

    private void Start() {
        for(int i = 0;  i < weapons.childCount; i++ ) {
            if ( weapons.GetChild(i).gameObject == currentWeapon ) {
                weapons.GetChild(i).gameObject.SetActive(true);
                continue;
            }

            weapons.GetChild(i).gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if ( !components.localPlayer )
            return;

        if ( currentWeapon.GetComponent<WeaponInfo>().gunData.reloading )
            return;

        if ( Input.GetKeyDown(KeyCode.Alpha1) ) {
            SwitchWeapon(0);
        }
        else if ( Input.GetKeyDown(KeyCode.Alpha2) ) {
            SwitchWeapon(1);
        }
        else if ( Input.GetKeyDown(KeyCode.Alpha3) ) {
            SwitchWeapon(2);
        }
    }


    void SwitchWeapon(int weapon) {
        for (int i = 0; i < weapons.childCount; i++ ) {
            if ( i == weapon )
                continue;

            weapons.GetChild(i).gameObject.SetActive(false);
        }

        weapons.GetChild(weapon).gameObject.SetActive(true);
        system.CmdSwitchCharacterWeapon(weapon);
        currentWeapon = weapons.GetChild(weapon).gameObject;
        system.WeaponSet();
        StartCoroutine(Ready());
    }

    IEnumerator Ready() {
        yield return new WaitForSeconds(.8f);
        system.gunReady = true;
    }
}