using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Source.Scripts.Cards;
using UnityEngine;

namespace Assets.Source.Scripts.Hands
{
    internal class HandStorageScript : MonoBehaviour
    {
        public event Action OnCardsChange;
        public List<CardMono> cards { get; private set; } = new List<CardMono>();

        public float moveTime;

        private Vector3 cardOffset = new Vector3(1.3f, 0.01f, 1.75f);
        private Vector3 NextPosition => this.transform.position + (cardOffset * cards.Count);

        public ITaskItem AddCard(TaskQueue manager, CardMono card)
        {
            card.OnCardChange += () => OnCardsChange?.Invoke();
            var task = manager.AddTask((x) => MoveCard(card, NextPosition), name: $"move {card.card:n}");
            task.OnEnd += (x) =>
            {
                cards.Add(card);
                card.transform.parent = this.transform;
                OnCardsChange?.Invoke();
            };
            return task;
        }

        public void ClearCards()
        {
            foreach (var card in cards)
            {
                card.transform.parent = null;
            }
            cards.Clear();
            OnCardsChange?.Invoke();
        }

        private async Task MoveCard(CardMono card, Vector3 positions)
        {
            var moveTime = this.moveTime;

            var startPosition = card.transform.position;

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
                if (card != null)
                    card.transform.position = Vector3.Lerp(positions, startPosition, relation);
            }
        }

    }
}
