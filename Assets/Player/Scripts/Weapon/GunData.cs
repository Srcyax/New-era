using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "Weapon/NewGun")]
public class GunData : ScriptableObject {
    [Header("Info")]
    public new string name;

    [Header("Shooting")]
    public float damage;
    public float maxDistance;
    public float spread;

    [Header("Reloading")]
    public int currentAmmo;
    public int magSize;
    public float fireRate;
    public float reloadTime;

    [HideInInspector]
    public bool reloading;
}
