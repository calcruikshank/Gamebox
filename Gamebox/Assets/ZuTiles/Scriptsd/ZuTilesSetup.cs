using Gameboard.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZuTilesSetup : MonoBehaviour
{
    [SerializeField] GameObject[] decksToChooseFrom;

    public static ZuTilesSetup singleton;
    // Start is called before the first frame update
    void Start()
    {
        if (singleton != null)
        {
            Destroy(this);
        }
        singleton = this;
        //display decks to choose from to the players
        //shuffle and load the decks for each player
    }

    // Update is called once per frame
    void Update()
    {

    }

    void AddZuTileToEachPlayer(PlayerPresenceDrawer ppd)
    {
        for (int i = 0; i < UserPresenceTest.singleton.playerList.Count; i++)
        {
            ZuTilePlayer ztp = UserPresenceTest.singleton.playerList[i].gameObject.AddComponent<ZuTilePlayer>();
        }
    }
    public void AddZuTilePlayer(PlayerPresenceDrawer ppd)
    {
        ZuTilePlayer ztp = ppd.gameObject.AddComponent<ZuTilePlayer>();

        //ztp.ShowPlayerDeckToChooseFrom(decksToChooseFrom);
    }
}
