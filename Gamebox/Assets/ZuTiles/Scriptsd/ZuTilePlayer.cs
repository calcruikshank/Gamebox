using Gameboard.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZuTilePlayer : MonoBehaviour
{

    GameObject ChosenDeck;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void ShowPlayerDeckToChooseFrom(GameObject[] decksToChooseFrom)
    {
        for (int i = 0; i < decksToChooseFrom.Length; i++)
        {
            //GameObject buttonSelectionDeck = Instantiate(decksToChooseFrom[i], transform.position, transform.rotation);
            Debug.Log("Showing Player Deck");
        }
    }

    internal void SetDeckToDeckToInstantiate(GameObject deckToInstantiate)
    {
        Debug.Log("Instantiating deck  " + deckToInstantiate);
        ChosenDeck = deckToInstantiate;
        GameObject newDeck = Instantiate(deckToInstantiate, this.transform.position, Quaternion.identity);

        //lock the choice in here but instantiate the deck on game start instead
    }
}
