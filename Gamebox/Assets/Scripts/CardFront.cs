using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFront : MonoBehaviour
{
    public void ChangeCardFront(GameObject cardFrontSent)
    {
        Debug.Log(cardFrontSent + " card front sent");
        Instantiate(cardFrontSent, this.transform.parent);
        Destroy(this.gameObject);
    }
}
