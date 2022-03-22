using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Source.Model.Games.BlackJack.Users
{
    public abstract class User : IUser
    {
        public event Action<Card> OnCardAdd;
        public event Action<GameResult> OnGameResult;

        protected List<Card> cards = new List<Card>();
        private Func<IEnumerable<Card>, IEnumerable<int>> GetScoresIternal;

        public User(Func<IEnumerable<Card>, IEnumerable<int>> GetScores)
        {
            this.GetScoresIternal = GetScores;
        }
        public abstract PlayerStatus? GetStatus();

        public void AddCard(Card card)
        {
            cards.Add(card);
            OnCardAdd?.Invoke(card);
        }

        public void Reset()
        {
            cards.Clear();
        }

        public IEnumerable<int> GetScores()
            => GetScoresIternal(cards);

        public bool IsBust()
            => this.GetScores().All(x => x > 21);

        public bool IsBlackJack()
            => this.GetScores().Any(x => x == 21);
    }


    public enum PlayerStatus
    {
        Win,
        Lose,
        Push,
        Bust
    }
}
