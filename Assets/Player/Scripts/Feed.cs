using Mirror;
using TMPro;
using UnityEngine;

public class Feed : NetworkBehaviour {
    [SerializeField] GameObject killFeedPrefab;
    [SerializeField] GameObject chatFeedPrefab;

    GameObject killfeed;
    GameObject chatFeed;
    void Start() {
        killfeed = GameObject.FindGameObjectWithTag("Killfeed");
        chatFeed = GameObject.FindGameObjectWithTag("ChatFeed");
    }


    public void KillFeed(string killer_name, string killed_name, string reason) {
        TextMeshProUGUI kill = killFeedPrefab.GetComponent<TextMeshProUGUI>();

        kill.text = killer_name + " " + reason + " " + killed_name;
        if ( killfeed.transform.childCount > 0 ) {
            Transform lastChild = killfeed.transform.GetChild(killfeed.transform.childCount - 1);
            Vector3 newPosition = new Vector3(0f, lastChild.localPosition.y - 25f, 0f);

            GameObject obj = Instantiate(killFeedPrefab, killfeed.transform);
            obj.transform.localPosition = newPosition;
        }
        else {
            Instantiate(killFeedPrefab, killfeed.transform);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdFeedPlayerTeamJoined(string player, string team) {
        RpcFeedPlayerTeamJoined(player, team);
    }

    [ClientRpc]
    void RpcFeedPlayerTeamJoined(string player, string team) {
        TextMeshProUGUI chat = chatFeedPrefab.GetComponent<TextMeshProUGUI>();

        chat.text = player + " joined team " + team;

        if ( chatFeed.transform.childCount > 0 ) {
            Transform lastChild = chatFeed.transform.GetChild(chatFeed.transform.childCount - 1);
            Vector3 newPosition = new Vector3(0f, lastChild.localPosition.y + 25f, 0f);

            GameObject obj = Instantiate(chatFeedPrefab, chatFeed.transform);
            obj.transform.localPosition = newPosition;
        }
        else {
            Instantiate(chatFeedPrefab, chatFeed.transform);
        }
    }
}