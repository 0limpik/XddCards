using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Source.Model.Enums;
using Random = UnityEngine.Random;

namespace Assets.Source.Model.Games
{
    public class Deck
    {
        public List<Card> cards;

        public Deck()
        {
            Reload();
        }

        public bool TryPeek(out Card card)
        {
            if (cards.Count > 0)
            {
                card = cards[0];
                cards.Remove(card);

                return cards.Count > 0;
            }

            card = null;
            return false;
        }

        public void Reload()
        {
            cards = Create().ToList();
            Shuffle();
        }

        private void Shuffle()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                var rand = Random.Range(0, cards.Count);

                (cards[i], cards[rand]) = (cards[rand], cards[i]);
            }
        }

        public static Card[] Create()
        {
            var ranks = Enum.GetValues(typeof(Ranks)).Cast<Ranks>();
            var suits = Enum.GetValues(typeof(Suits)).Cast<Suits>();

            var cards = new Card[ranks.Count() * suits.Count()];

            var num = 0;

            foreach (var suit in suits)
            {
                foreach (var rank in ranks)
                {
                    cards[num++] = new Card { rank = rank, suit = suit };
                }
            }

            return cards;
        }
    }
}
