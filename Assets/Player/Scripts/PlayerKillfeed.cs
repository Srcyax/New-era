using UnityEngine;
using Mirror;
using TMPro;

public class PlayerKillfeed : NetworkBehaviour {
    [SerializeField] GameObject killFeedPrefab;

    Transform killfeed;
    void Start() {
        killfeed = GameObject.FindGameObjectWithTag("Killfeed").transform;
    }

    // Update is called once per frame
    void Update() {

    }

    [ClientRpc]
    public void RpcKillFeed(string killer_name, string killed_name) {
        TextMeshProUGUI kill = killFeedPrefab.GetComponent<TextMeshProUGUI>();

        kill.text = killer_name + " killed " + killed_name;

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