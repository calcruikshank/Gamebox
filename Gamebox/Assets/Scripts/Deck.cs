using Shared.UI.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> cardsInDeck = new List<GameObject>();
    MovableObjectStateMachine movableObject;
    GameObject currentCardShowing;

    [SerializeField] LayerMask playerContainerLayerMask;
    void Start()
    {
        InitializeDeck();
        UpdateDeckInfo();
    }

    public void SetSelected(int id, Vector3 offset)
    {
        TouchScript.shuffleInitiated += ShuffleDeck;
    }
    public void SetUnselected(int id, Vector3 offset)
    {
        TouchScript.shuffleInitiated -= ShuffleDeck;
    }
    public void InitializeDeck()
    {
        currentCardShowing = this.transform.GetComponentInChildren<CardFront>().gameObject;
        movableObject = this.transform.GetComponent<MovableObjectStateMachine>();
    }

    public void UpdateDeckInfo()
    {
        SetSize(new Vector3(this.transform.localScale.x, cardsInDeck.Count, this.transform.localScale.z));
        SetCurrentCardShowing(cardsInDeck[cardsInDeck.Count - 1]);
        SetTopCard(cardsInDeck[0]);
    }

    public void AddToDeck(List<GameObject> cardsSents)
    {
        foreach (GameObject card in cardsSents)
        {
            cardsInDeck.Add(card);
        }
        UpdateDeckInfo();
    }
    public void AddToFrontOfList(List<GameObject> cardsSents)
    {
        for (int i = 0; i < cardsSents.Count; i++)
        {
            cardsInDeck.Add(cardsInDeck[i]);
            cardsInDeck[i] = cardsSents[i];
        }
        UpdateDeckInfo();
    }

    public void CheckToSeeIfDeckShouldBeAdded()
    {
        Collider colliderHit = transform.GetComponentInChildren<Collider>();
        RaycastHit hit;
        bool hitDetected = Physics.BoxCast(colliderHit.bounds.center, new Vector3(colliderHit.bounds.extents.x - (colliderHit.bounds.extents.x * 1.5f), colliderHit.bounds.extents.y - (colliderHit.bounds.extents.y * 1.5f), colliderHit.bounds.extents.z - (colliderHit.bounds.extents.z * 1.5f)), Vector3.down, out hit, Quaternion.identity, Mathf.Infinity);
        RaycastHit playerContainerHit;
        bool playerContainerHitBool = Physics.BoxCast(colliderHit.bounds.center, new Vector3(colliderHit.bounds.extents.x - (colliderHit.bounds.extents.x * 1.5f), colliderHit.bounds.extents.y - (colliderHit.bounds.extents.y * 1.5f), colliderHit.bounds.extents.z - (colliderHit.bounds.extents.z * 1.5f)), Vector3.down, out playerContainerHit, Quaternion.identity, playerContainerLayerMask);

        if (playerContainerHitBool)
        {
            Transform targetToMove = hit.transform;
            if (targetToMove.GetComponentInChildren<PlayerContainer>() != null)
            {
                PlayerContainer playerToAddCardTo = targetToMove.GetComponentInChildren<PlayerContainer>();
                playerToAddCardTo.AddCardToHand(this.gameObject);
                return;
            }
        }
        if (hitDetected)
        {
            Transform targetToMove = hit.transform;
            while (targetToMove.parent != null)
            {
                targetToMove = targetToMove.transform.parent;
            }

            if (targetToMove.GetComponent<Deck>() != null)
            {
                if (targetToMove.GetComponent<Deck>() != this)
                {
                    if (targetToMove.GetComponent<MovableObjectStateMachine>().faceUp)
                    {
                        Debug.Log("Hit detected " + targetToMove.name);
                        Deck deckToAddTo = targetToMove.GetComponent<Deck>();
                        deckToAddTo.AddToDeck(this.cardsInDeck);
                        Destroy(this.gameObject);
                    }
                    if (!targetToMove.GetComponent<MovableObjectStateMachine>().faceUp)
                    {
                        Debug.Log("Adding to front of list " + targetToMove.name);
                        Deck deckToAddTo = targetToMove.GetComponent<Deck>();
                        deckToAddTo.AddToFrontOfList(this.cardsInDeck);
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
        GetComponentInChildren<CardFront>().ChangeCardFront(cardSent.GetComponentInChildren<CardFront>().gameObject);

        currentCardShowing = GetComponentInChildren<CardFront>().gameObject;
        if (currentCardShowing.GetComponent<CardTilter>() != null)
        {
            currentCardShowing.GetComponent<CardTilter>().enabled = false;
        }
    }
    public void ShuffleDeck(Vector3 offset, int id)
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



    public void PickUpCards(int numOfCardsToPickUp)
    {
        if (cardsInDeck.Count == 1) //change thisd to say if cardsindeck.count is greater than number of cards to pick up
        {
            return;
        }
        if (movableObject.faceUp)
        {
            PickUpCardsFromBottom(numOfCardsToPickUp);
        }
        if (!movableObject.faceUp)
        {
            PickUpCardsFromTop(numOfCardsToPickUp);
        }
    }

    public void PickUpCardsFromBottom(int numOfCardsToPickUp)
    {
        GameObject newDeck;
        newDeck = Instantiate(this.gameObject, transform.position, Quaternion.identity);
        int iniatedI = cardsInDeck.Count;
        cardsInDeck.Clear();
        for (int i = iniatedI - numOfCardsToPickUp; i <= iniatedI - 1; i++)
        {
            GameObject cardToAddThenRemove = newDeck.GetComponent<Deck>().cardsInDeck[i];
            cardsInDeck.Add(cardToAddThenRemove);
            newDeck.GetComponent<Deck>().cardsInDeck.RemoveAt(i);
        }

        UpdateDeckInfo();

    }

    public void PickUpCardsFromTop(int numOfCardsToPickUp)
    {
        GameObject newDeck;
        newDeck = Instantiate(this.gameObject, transform.position, Quaternion.identity);
        int iniatedI = cardsInDeck.Count;
        cardsInDeck.Clear();
        for (int i = 0; i <= numOfCardsToPickUp - 1; i++)
        {
            Debug.Log(i);
            GameObject cardToAddThenRemove = newDeck.GetComponent<Deck>().cardsInDeck[i];
            cardsInDeck.Add(cardToAddThenRemove);
            newDeck.GetComponent<Deck>().cardsInDeck.RemoveAt(i);
        }

        UpdateDeckInfo();
    }
}
