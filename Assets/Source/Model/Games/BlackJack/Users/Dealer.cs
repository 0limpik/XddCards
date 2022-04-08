﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Source.Model.Games.BlackJack.Users
{
    internal class Dealer : User
    {
        public Dealer(Func<IEnumerable<Card>, IEnumerable<int>> GetScores)
            : base(GetScores)
        {
        }

        public override void AddCard(Card card)
        {
            base.AddCard(card);

            CheckTurn();

            InvokeOnCardAdd(card);
        }

        public void AddCardHidden(Card card)
        {
            base.AddCard(card);

            CheckTurn();
        }

        private void CheckTurn()
        {
            if (GetScores().Where(x => x <= 21).Any(x => x >= 17))
                CanTurn = false;
        }

        public override PlayerStatus? GetStatus()
        {
            if (IsBust())
                return PlayerStatus.Bust;

            return null;
        }
    }
}