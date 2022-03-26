using UnityEngine;

public class TestScript : MonoBehaviour
{
    public BJHandScript hand;
    public CardMono card;
    public CardsSequenceScript sequence;

    void Start()
    {
        //await sequence.AddCard(new CardTranslateItem { transform = card.transform, storage = hand, destination = hand.transform });
        //Debug.Log("Card Delivered");
    }

    void Update()
    {

    }
}
