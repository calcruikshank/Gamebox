using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContainer : MonoBehaviour
{
    int numberOfCardsInHand = 0;
    float movableObjectPadding = 1f;
    List<GameObject> cardsInHand = new List<GameObject>();
    public void AddCardToHand(GameObject cardToAdd)
    {
        //position = (this.transform.bounds.x - this.transform.bounds.x + padding)
        cardToAdd.transform.position = new Vector3(((this.transform.position.x + 2f - (this.transform.GetComponent<Collider>().bounds.size.x / 2)) + (cardToAdd.transform.GetComponentInChildren<Collider>().bounds.size.x * cardsInHand.Count) + movableObjectPadding * cardsInHand.Count), cardToAdd.transform.position.y, this.transform.position.z);
        cardsInHand.Add(cardToAdd);
        cardToAdd.GetComponent<MovableObject>().GivePlayerOwnership(this);
    }
    public void RemoveCardFromHand(GameObject cardToRemove)
    {
        Debug.Log("Removing Card " + cardToRemove);
        cardsInHand.Remove(cardToRemove);
        cardToRemove.GetComponent<MovableObject>().RemovePlayerOwnership(this);
        UpdateCardPositions();
    }

    void UpdateCardPositions()
    {
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            cardsInHand[i].transform.position = new Vector3(((this.transform.position.x + 2f - (this.transform.GetComponent<Collider>().bounds.size.x / 2)) + (cardsInHand[i].transform.GetComponentInChildren<Collider>().bounds.size.x * i) + movableObjectPadding * i), cardsInHand[i].transform.position.y, this.transform.position.z);
        }
    }
}
