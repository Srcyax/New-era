using UnityEngine;

[System.Serializable]
public class PlayerSettingsData 
{
    public string name;
    public float sensibility;
    public int graphics;
}

[System.Serializable]
public class PlayerStatusData {
    public int kills;
    public int deaths;
}