using UnityEngine;

public class TeamArrowSpawn : MonoBehaviour {
    [SerializeField] GameObject prefabTeamArrow;

    public void SetTeamArrow(int playerTeam) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for ( int i = 0; i < players.Length; i++ ) {
            if ( !players[ i ] )
                continue;

            if ( players[ i ].gameObject == gameObject )
                continue;

            if ( players[ i ].GetComponent<PlayerComponents>().playerTeam != playerTeam )
                continue;

            if ( players[ i ].transform.GetChild(0).childCount > 5 )
                continue;

            prefabTeamArrow.GetComponent<SpriteRenderer>().color = playerTeam == 0 ? Color.blue : Color.red;

            Instantiate(prefabTeamArrow, players[ i ].transform.GetChild(0));
        }
    }
}
