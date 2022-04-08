using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Source.Scripts.Cards;
using UnityEngine;

public class DeckScript : MonoBehaviour
{
    public float moveTime;

    public List<CardMono> cards { get; } = new List<CardMono>();

    public Vector3 cardOffset;

    private Vector3 NextPosition => this.transform.position + (cardOffset * cards.Count);

    public ITaskItem AddCard(TaskQueue manager, CardMono[] cards)
    {
        var positions = new Vector3[cards.Length];
        for (int i = 0; i < cards.Length; i++)
        {
            positions[i] = NextPosition + cardOffset * i;
        }

        var task = manager.AddTask((x) => MoveCard(cards, positions), name:$"move to deck {cards.Length} cards");
        task.OnEnd += (x) =>
        {
            this.cards.AddRange(cards);
            this.cards.ForEach(x => x.transform.parent = this.transform);
        };
        return task;
    }


    private async Task MoveCard(CardMono[] cards, Vector3[] positions)
    {
        var moveTime = this.moveTime;

        var items = new (CardMono card, Vector3 startPosition, Vector3 endPosition)[cards.Length];

        for (int i = 0; i < items.Length; i++)
        {
            items[i] = (cards[i], cards[i].transform.position, positions[i]);
        }

        if (moveTime != 0)
        {
            while ((moveTime -= Time.deltaTime) > 0)
            {
                var relation = moveTime / this.moveTime;
                Move(relation);
                await Task.Yield();
            }
        }
        Move(0);

        void Move(float relation)
        {
            foreach (var item in items)
            {
                if (item.card != null)
                    item.card.transform.position = Vector3.Lerp(item.endPosition, item.startPosition, relation);
            }
        }
    }
}
