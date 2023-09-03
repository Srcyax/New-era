using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JsonSaveSystem : MonoBehaviour {
    public int key = 1337;

    public void SettingsDataSaveToJson(float sensibility, int graphics, string name) {
        if ( !Directory.Exists("C:/userdata") )
            Directory.CreateDirectory("C:/userdata");

        PlayerSettingsData data = new PlayerSettingsData();
        data.sensibility = sensibility;
        data.graphics = graphics;
        data.name = name;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText("C:/userdata/settingsData.json", EncryptDecrypt(json, key));
    }

    public void SettingsDataLoadFromJson(Slider sensibility, TMP_Dropdown graphics, TMP_InputField name) {
        if ( !File.Exists("C:/userdata/settingsData.json") || !Directory.Exists("C:/userdata") ) {
            SettingsDataSaveToJson(1f, 4, "");
            return;
        }

        string json = File.ReadAllText("C:/userdata/settingsData.json");
        PlayerSettingsData data = JsonUtility.FromJson<PlayerSettingsData>(EncryptDecrypt(json, key));
        sensibility.value = data.sensibility;
        graphics.value = data.graphics;
        name.text = data.name;
    }

    /*public void StatusDataSaveToJson(int kills, int deaths) {
        PlayerStatusData data = new PlayerStatusData();
        data.kills = kills;
        data.deaths = deaths;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText("C:/userdata/statusData.json", EncryptDecrypt(json, key));
    }

    public void StatusDataLoadFromJson(PlayerData pData) {
        if ( !File.Exists("C:/userdata/statusData.json") ) {
            StatusDataSaveToJson(0, 0);
            return;
        }

        string json = File.ReadAllText("C:/userdata/statusData.json");
        PlayerStatusData data = JsonUtility.FromJson<PlayerStatusData>(EncryptDecrypt(json, key));
        pData.kills = data.kills;
        pData.deaths = data.deaths;
    }*/

    public string EncryptDecrypt(string data, int key) {
        StringBuilder input = new StringBuilder(data);
        StringBuilder output = new StringBuilder(data.Length);

        char character;

        for ( int i = 0; i < data.Length; ++i ) {
            character = input[ i ];
            character = ( char )( character ^ key );
            output.Append(character);
        }

        return output.ToString();
    }
}
