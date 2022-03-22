using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckScript : MonoBehaviour, ICardStorage
{
    public List<CardMono> cards = new List<CardMono>();

    public Vector3 cardOffset;

    public void AddCard(CardMono card)
    {
        cards.Add(card);
        card.transform.parent = this.transform;
    }

    public Vector3 GetCardPosition()
    {
        return this.transform.position + (cardOffset * (cards.Count + 1));
    }

    public void RemoveCard(CardMono card)
    {
        cards.Remove(card);
    }

    public void UpdateScores()
    {

    }
}
