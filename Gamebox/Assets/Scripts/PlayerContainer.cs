using Gameboard.Examples;
using Gameboard.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContainer : MonoBehaviour
{
    int numberOfCardsInHand = 0;
    float movableObjectPadding = 1f;
    List<GameObject> cardsInHand = new List<GameObject>();
    PlayerPresenceDrawer inPlayer;
    string cardHandID;
    Dictionary<CardDefinition, string> cardDefinitions = new Dictionary<CardDefinition, string>();
    private void Start()
    {
        inPlayer = Crutilities.singleton.GetFinalParent(this.transform).GetComponentInChildren<PlayerPresenceDrawer>();
    }
    public void AddCardToHand(GameObject cardToAdd)
    {
        //position = (this.transform.bounds.x - this.transform.bounds.x + padding)
        //cardToAdd.transform.position = new Vector3(((this.transform.position.x + (this.transform.GetComponent<Collider>().bounds.size.x / 2) - (this.transform.GetComponent<Collider>().bounds.size.x / 2)) + (cardToAdd.transform.GetComponentInChildren<Collider>().bounds.size.x * cardsInHand.Count) + movableObjectPadding * cardsInHand.Count), cardToAdd.transform.position.y, this.transform.position.z);
        cardToAdd.transform.rotation = this.transform.rotation;
        cardsInHand.Add(cardToAdd);
        UpdateCardPositions();
        cardToAdd.GetComponent<MovableObjectStateMachine>().GivePlayerOwnership(this);
        AddToCompanion(cardToAdd);
    }

    private void AddToCompanion(GameObject cardToAdd)
    {
        Texture2D cardImageTexture = (Texture2D)cardToAdd.GetComponentInChildren<Renderer>().material.mainTexture;
        byte[] textureArray = DeCompress(cardImageTexture).EncodeToPNG();
        CardDefinition newCardDef = new CardDefinition(cardImageTexture.name, textureArray, "", null, cardImageTexture.width / 2, cardImageTexture.height / 2);
        cardDefinitions.Add(newCardDef, CardsTool.singleton.GetCurrentLocationOfCardByGUID(newCardDef.cardGuid));
        cardToAdd.GetComponent<Deck>().CardCompanionDefiniiton = newCardDef;
        AddCardToHand(newCardDef);
    }
    
    private async void AddCardToHand(CardDefinition newCardDef)
    {
        cardHandID = CardsTool.singleton.GetCardHandDisplayedForPlayer(inPlayer.userId);
        await CardsTool.singleton.PlaceCardInPlayerHand_Async(inPlayer.userId, cardHandID, newCardDef);
    }
    public void RemoveCardFromHand(GameObject cardToRemove)
    {
        cardsInHand.Remove(cardToRemove);
        cardToRemove.GetComponent<MovableObjectStateMachine>().RemovePlayerOwnership(this);
        UpdateCardPositions();
        RemoveFromCompanion(cardToRemove);
    }

    void RemoveFromCompanion(GameObject cardToRemove)
    {
        RemoveCardFromPlayerHand(cardToRemove);
    }
    private async void RemoveCardFromPlayerHand(GameObject cardToRemove)
    {
        await  CardsTool.singleton.RemoveCardFromPlayerHand_Async(inPlayer.userId, cardHandID, cardToRemove.GetComponent<Deck>().CardCompanionDefiniiton);
    }

    void UpdateCardPositions()
    {
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            cardsInHand[i].transform.position = new Vector3((this.transform.position.x + (cardsInHand[i].transform.GetComponentInChildren<Collider>().bounds.size.x * i) + movableObjectPadding * i), cardsInHand[i].transform.position.y, this.transform.position.z);
        }
    }
    public Texture2D DeCompress(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
}
