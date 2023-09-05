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
    }

    void SwitchWeapon(int weapon) {

        for (int i = 0; i < weapons.childCount; i++ ) {
            if ( i == weapon )
                continue;

            weapons.GetChild(i).gameObject.SetActive(false);
        }

        weapons.GetChild(weapon).gameObject.SetActive(true);    
        currentWeapon = weapons.GetChild(weapon).gameObject;
        system.WeaponSet();
        StartCoroutine(Ready());
    }

    IEnumerator Ready() {
        yield return new WaitForSeconds(.8f);
        currentWeapon.GetComponent<WeaponInfo>().gunData.ready = true;
    }
}
