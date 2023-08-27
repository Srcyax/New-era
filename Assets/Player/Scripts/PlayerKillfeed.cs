using UnityEngine;
using Mirror;
using TMPro;

public class PlayerKillfeed : MonoBehaviour {
    [SerializeField] GameObject killFeedPrefab;

    Transform killfeed;
    void Start() {
        killfeed = GameObject.FindGameObjectWithTag("Killfeed").transform;
    }

    public void RpcKillFeed(string killer_name, string killed_name, string reason) {
        TextMeshProUGUI kill = killFeedPrefab.GetComponent<TextMeshProUGUI>();

        kill.text = killer_name + " " + reason + " " + killed_name;

        if ( killfeed.childCount > 0 ) {
            Transform lastChild = killfeed.GetChild(killfeed.childCount - 1);
            Vector3 newPosition = new Vector3(0f, lastChild.localPosition.y - 25f, 0f);

            GameObject obj = Instantiate(killFeedPrefab, killfeed);
            obj.transform.localPosition = newPosition;
        }
        else {
            Instantiate(killFeedPrefab, killfeed);
        }
    }
}