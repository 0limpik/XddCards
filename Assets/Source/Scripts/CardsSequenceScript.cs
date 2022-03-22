using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CardsSequenceScript : MonoBehaviour
{
    private Queue<CardTranslateItem[]> sequence = new Queue<CardTranslateItem[]>();

    public float moveTime;

    public async Task TranslateCard(CardTranslateItem card)
    {
        var item = new CardTranslateItem[] { card };
        sequence.Enqueue(item);

        if (sequence.Count == 1)
        {
            TaskLoop();
        }

        while (sequence.Contains(item))
        {
            await Task.Yield();
        }
    }

    public async Task TranslateCards(CardTranslateItem[] card)
    {
        sequence.Enqueue(card);

        if (sequence.Count == 1)
        {
            TaskLoop();
        }

        while (sequence.Contains(card))
        {
            await Task.Yield();
        }
    }

    private async void TaskLoop()
    {
        //while (sequence.Count > 0)
        //{
        //    var items = sequence.Peek();

        //    var positions = new List<(CardTranslateItem card, Vector3 destPos, Vector3 startPos)>();

        //    foreach (var item in items)
        //    {
        //        positions.Add((item, item.storage.AddCard(item.card), item.card.transform.position));
        //    }

        //    var moveTime = this.moveTime;
        //    while ((moveTime -= Time.deltaTime) > 0)
        //    {
        //        foreach (var item in positions)
        //        {
        //            item.card.card.transform.position = Vector3.Lerp(item.destPos, item.startPos, moveTime / this.moveTime);
        //        }

        //        await Task.Yield();
        //    }
        //    sequence.Dequeue();
        //}
    }
}

public class CardTranslateItem
{
    public CardMono card;
    public ICardStorage storage;
}

public interface ICardStorage
{
    void AddCard(CardMono card);
    Vector3 GetCardPosition();
}
