using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Model.Games
{
    [CreateAssetMenu(fileName = "Card", menuName = "CARD/COLLECTION", order = 100000)]
    public class CardCollection : ScriptableObject
    {
        public List<CardObject> cards;

        public CardObject GetCardObject(ICard card)
        {
            if(card == null)
            {
                return null;
            }

            return cards.First(x => x.suit == card.suit && x.rank == card.rank);
        }
    }
}
