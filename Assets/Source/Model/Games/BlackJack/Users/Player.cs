using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Source.Model.Games.BlackJack.Users
{
    internal class Player : User
    {
        private Dealer dealer;

        public bool isNotifiedResult;

        public Player(Func<IEnumerable<Card>, IEnumerable<int>> GetScores, Dealer dealer)
            : base(GetScores)
        {
            OnResult += (result) => isNotifiedResult = true;
            this.dealer = dealer;
        }

        public override void AddCard(Card card)
        {
            base.AddCard(card);

            InvokeOnCardAdd(card);
        }

        public override void Reset()
        {
            base.Reset();
            isNotifiedResult = false;
        }

        public override PlayerStatus? GetStatus()
        {
            if (IsBust())
                return PlayerStatus.Bust;

            if (dealer.IsBlackJack())
                return PlayerStatus.Lose;

            if (IsMore())
                return PlayerStatus.Win;

            if (IsEquals())
                return PlayerStatus.Push;

            if (IsLess())
                return PlayerStatus.Lose;

            return null;
        }

        public bool IsMore()
        {
            var playerScores = this.GetScores().Where(x => x <= 21);
            var dillerScores = dealer.GetScores().Where(x => x <= 21);
            return playerScores.Any(p => dillerScores.All(d => p > d));
        }

        public bool IsLess()
        {
            var playerScores = this.GetScores().Where(x => x <= 21);
            var dillerScores = dealer.GetScores().Where(x => x <= 21);
            return dillerScores.Any(d => playerScores.All(p => d > p));
        }

        public bool IsEquals()
        {
            var playerScores = this.GetScores().Where(x => x <= 21);
            var dillerScores = dealer.GetScores().Where(x => x <= 21);

            return playerScores.Any(p => dillerScores.All(d => p >= d));
        }
    }
}
