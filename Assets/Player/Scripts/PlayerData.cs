using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/NewPlayerData")]
public class PlayerData : ScriptableObject {
    public new string name;

    [Header("Settings")]
    public float sensibility;
    public int graphics;
}
