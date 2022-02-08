using Shared.UI.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]List<GameObject> cardsInDeck = new List<GameObject>();

    GameObject currentCardShowing;
    void Start()
    {
        
        InitializeDeck();
        UpdateDeckInfo();
        ShuffleDeck();
    }

    public void InitializeDeck()
    {
        currentCardShowing = this.transform.GetChild(0).gameObject;
        Debug.Log(currentCardShowing.name);
    }

    public void UpdateDeckInfo()
    {
        SetSize(new Vector3(this.transform.localScale.x, cardsInDeck.Count, this.transform.localScale.z));
        SetCurrentCardShowing(cardsInDeck[cardsInDeck.Count - 1]);
        SetTopCard(cardsInDeck[0]);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            ShuffleDeck();
        }
    }

    public void AddToDeck(List<GameObject> cardsSents)
    {
        foreach (GameObject card in cardsSents)
        {
            cardsInDeck.Add(card);
        }
        UpdateDeckInfo();
    }

    public void CheckToSeeIfDeckShouldBeAdded()
    {
        Collider colliderHit = transform.GetComponentInChildren<Collider>();
        RaycastHit hit;
        bool hitDetected = Physics.BoxCast(colliderHit.bounds.center, new Vector3(colliderHit.bounds.extents.x - (colliderHit.bounds.extents.x * 1.5f), colliderHit.bounds.extents.y - (colliderHit.bounds.extents.y * 1.5f), colliderHit.bounds.extents.z - (colliderHit.bounds.extents.z * 1.5f)), Vector3.down, out hit, Quaternion.identity, Mathf.Infinity);

        if (hitDetected)
        {
            Transform targetToMove = hit.transform;
            while (targetToMove.parent != null)
            {
                targetToMove = targetToMove.transform.parent;
            }
            if (targetToMove.parent == null)
            {
                if (targetToMove.GetComponent<Deck>() != null)
                {
                    if (targetToMove.GetComponent<Deck>() != this)
                    {
                        Debug.Log("Hit detected " + targetToMove.name);
                        Deck deckToAddTo = targetToMove.GetComponent<Deck>();
                        deckToAddTo.AddToDeck(this.cardsInDeck);
                        Destroy(this.gameObject);
                    }
                }
            }
            targetToMove = null;
        }
    }

    public void SetSize(Vector3 localSizeSent)
    {
        this.transform.localScale = localSizeSent;
    }

    public void SetTopCard(GameObject cardSent)
    {
    }

    public void SetCurrentCardShowing(GameObject cardSent)
    {
        GameObject bottomCard = Instantiate(cardSent, this.transform);
        if (currentCardShowing != null)
        {
            Destroy(currentCardShowing);
        }
        currentCardShowing = bottomCard;
        if (currentCardShowing.GetComponent<CardTilter>() != null)
        {
            currentCardShowing.GetComponent<CardTilter>().enabled = false;
        }
    }

    public void ShuffleDeck()
    {
        Shuffle(cardsInDeck);
        SetCurrentCardShowing(cardsInDeck[cardsInDeck.Count - 1]);
    }

    public void Shuffle<GameObject>(List<GameObject> listToShuffle)
    {
        for (int i = 0; i < listToShuffle.Count - 1; i++)
        {
            GameObject temp = listToShuffle[i];
            int rand = UnityEngine.Random.Range(i, listToShuffle.Count);
            listToShuffle[i] = listToShuffle[rand];
            listToShuffle[rand] = temp;
        }
    }
}
