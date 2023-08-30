using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JsonSaveSystem : MonoBehaviour {
    public int key = 1337;

    public void SettingsDataSaveToJson(float sensibility, int graphics, string name) {
        PlayerSettingsData data = new PlayerSettingsData();
        data.sensibility = sensibility;
        data.graphics = graphics;
        data.name = name;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText("C:/userdata/settingsData.json", EncryptDecrypt(json, key));
    }

    public void SettingsDataLoadFromJson(Slider sensibility, TMP_Dropdown graphics, TMP_InputField name) {
        if ( !File.Exists("C:/userdata/settingsData.json") ) {
            SettingsDataSaveToJson(1f, 0, "");
            return;
        }

        string json = File.ReadAllText("C:/userdata/settingsData.json");
        PlayerSettingsData data = JsonUtility.FromJson<PlayerSettingsData>(EncryptDecrypt(json, key));
        sensibility.value = data.sensibility;
        graphics.value = data.graphics;
        name.text = data.name;
    }

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
