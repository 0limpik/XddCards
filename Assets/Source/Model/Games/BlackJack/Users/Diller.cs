using System;
using System.Collections.Generic;

namespace Assets.Source.Model.Games.BlackJack.Users
{
    public class Diller : User
    {
        public Diller(Func<IEnumerable<Card>, IEnumerable<int>> GetScores)
            : base(GetScores)
        {
        }

        public void AddCardHidden(Card card)
        {
            cards.Add(card);
        }

        public override PlayerStatus? GetStatus()
        {
            if (IsBust())
                return PlayerStatus.Bust;

            return null;
        }
    }
}
