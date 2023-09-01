using UnityEngine;
using Mirror;
using TMPro;

public class Feed : MonoBehaviour {
    PlayerComponents components => GetComponent<PlayerComponents>();

    [SerializeField] GameObject killFeedPrefab;
    [SerializeField] GameObject chatFeedPrefab;

    Transform killfeed;
    Transform chatFeed;
    void Start() {
        killfeed = GameObject.FindGameObjectWithTag("Killfeed").transform;
        chatFeed = GameObject.FindGameObjectWithTag("ChatFeed").transform;
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

    public void RpcFeedPlayerTeamJoined(string player, string team) {
        TextMeshProUGUI chat = chatFeedPrefab.GetComponent<TextMeshProUGUI>();

        chat.text = player + " joined team " + team;

        if ( chatFeed.childCount > 0 ) {
            Transform lastChild = chatFeed.GetChild(chatFeed.childCount - 1);
            Vector3 newPosition = new Vector3(0f, lastChild.localPosition.y + 25f, 0f);

            GameObject obj = Instantiate(chatFeedPrefab, chatFeed);
            obj.transform.localPosition = newPosition;
        }
        else {
            Instantiate(chatFeedPrefab, chatFeed);
        }
    }
}