using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public HandScript hand;
    public CardMono card;
    public CardsSequenceScript sequence;

    async void Start()
    {
        //await sequence.AddCard(new CardTranslateItem { transform = card.transform, storage = hand, destination = hand.transform });
        //Debug.Log("Card Delivered");
    }

    void Update()
    {
        
    }
}
