using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/NewPlayerData")]
[System.Serializable]
public class PlayerData : ScriptableObject {
    public new string name;

    [Header("Settings")]
    public float sensibility;
    public int graphics;

    [Header("Player Status")]
    public int kills;
    public int deaths;
}