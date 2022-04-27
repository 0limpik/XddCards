using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xdd.Scripts.Cards;

namespace Xdd.Model.Games
{
    [CreateAssetMenu(fileName = "CardCollection", menuName = "Xdd/CARD/Collection", order = 11)]
    internal class CardCollection : ScriptableObject
    {
        [SerializeField] CardMono cardInstanse;
        public List<CardObject> cards;

        public CardObject GetCardObject(ICard card)
        {
            if (card == null)
            {
                return null;
            }

            return cards.First(x => x.suit == card.suit && x.rank == card.rank);
        }

        public CardMono CreateCardMono(ICard card, Vector3 position, Quaternion rotation, Transform transform)
        {
            var cardMono = GameObject.Instantiate(this.cardInstanse, position, rotation, transform);
            cardMono.card = GetCardObject(card);

            return cardMono;
        }
    }
}
