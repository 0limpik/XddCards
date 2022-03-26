using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.Scripts.BlackJack
{
    [Serializable]
    internal class CardsManager
    {
        public float moveTime;
        public float flipTime;
        public AnimationCurve moveCurve;

        public Task cardTranslate { get; private set; } = Task.CompletedTask;

        public Task MoveCard(CardMono card, Func<CardMono, Vector3> GetPosition)
            => MoveCards(new CardMono[] { card }, new Func<CardMono, Vector3>[] { GetPosition });

        public Task MoveCards(CardMono[] cards, Func<CardMono, Vector3> GetPosition)
            => MoveCards(cards, cards.Select(x => GetPosition).ToArray());

        public Task MoveCards(CardMono[] cards, Func<CardMono, Vector3>[] positions)
            => Wrap(() => MoveCardInternal(cards, positions));

        public Task FlipCard(CardMono card)
            => Wrap(() => FlipCardInternal(card));

        private async Task Wrap(Func<Task> action)
        {
            await Wait();
            cardTranslate = action();
            await cardTranslate;
        }

        private async Task MoveCardInternal(CardMono[] cards, Func<CardMono, Vector3>[] positions)
        {
            var moveTime = this.moveTime;

            var items = new (CardMono card, Vector3 startPosition, Vector3 endPosition)[cards.Length];

            for (int i = 0; i < items.Length; i++)
            {
                items[i] = (cards[i], cards[i].transform.position, positions[i](cards[i]));
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
                        item.card.transform.position = Vector3.Lerp(item.endPosition, item.startPosition, moveCurve.Evaluate(relation));
                }
            }
        }

        private async Task FlipCardInternal(CardMono card)
        {
            var flipTime = this.flipTime;
            var startRotation = card.transform.rotation;
            var startPosition = card.transform.position;
            var flippedRotation = startRotation * Quaternion.AngleAxis(180, Vector3.forward);

            if (flipTime != 0)
            {
                while ((flipTime -= Time.deltaTime) > 0)
                {
                    var relation = flipTime / this.flipTime;
                    Rotate(relation);
                    await Task.Yield();
                }
            }
            Rotate(0);

            void Rotate(float relation)
            {
                if (card != null)
                {
                    card.transform.rotation = Quaternion.Lerp(flippedRotation, startRotation, relation);
                    card.transform.position = startPosition + new Vector3(0, CardObject.cardWidth * card.transform.localScale.z * Mathf.Sin(relation * Mathf.PI), 0);
                }
            }
        }

        public async Task Wait()
        {
            do
            {
                await Task.Yield();
            } while (!cardTranslate.IsCompleted);
        }
    }
}
