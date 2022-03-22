using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Source.Model.Games.BlackJack.Users
{
    public class Player : User
    {
        private IUser diller;

        public Player(Func<IEnumerable<Card>, IEnumerable<int>> GetScores, IUser diller) : base(GetScores)
        {
            this.diller = diller;
        }

        public override PlayerStatus? GetStatus()
        {
            if (IsBust())
                return PlayerStatus.Bust;

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
            var dillerScores = diller.GetScores().Where(x => x <= 21);
            return playerScores.Any(p => dillerScores.All(d => p > d));
        }
        public bool IsLess()
        {
            var playerScores = this.GetScores().Where(x => x <= 21);
            var dillerScores = diller.GetScores().Where(x => x <= 21);
            return dillerScores.Any(d => playerScores.All(p => d > p));
        }

        public bool IsEquals()
        {
            var playerScores = this.GetScores().Where(x => x <= 21);
            var dillerScores = diller.GetScores().Where(x => x <= 21);

            return playerScores.Any(p => dillerScores.All(d => p >= d));
        }
    }

}
