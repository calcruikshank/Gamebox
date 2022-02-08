using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    // Start is called before the first frame update
    List<GameObject> cardsInDeck = new List<GameObject>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddToDeck(List<GameObject> cardsSents)
    {
        foreach (GameObject card in cardsSents)
        {
            cardsInDeck.Add(card);
        }
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
}
