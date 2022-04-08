using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Source.Model.Games.BlackJack.Users
{
    internal abstract class User : IPlayer
    {
        public event Action<Card> OnCardAdd;
        public event Action<GameResult> OnResult;

        protected List<Card> cards = new List<Card>();
        private Func<IEnumerable<Card>, IEnumerable<int>> GetScoresIternal;

        public bool CanTurn { get; set; } = true;

        public User(Func<IEnumerable<Card>, IEnumerable<int>> GetScores)
        {
            this.GetScoresIternal = GetScores;
        }
        public abstract PlayerStatus? GetStatus();

        public virtual void AddCard(Card card)
        {
            cards.Add(card);

            if (IsBust())
            {
                CanTurn = false;
                OnResult?.Invoke(GameResult.Lose);
            }

            if (IsBlackJack())
            {
                CanTurn = false;
            }
        }

        public virtual void Reset()
        {
            CanTurn = true;
            cards.Clear();
        }

        public IEnumerable<int> GetScores()
            => GetScoresIternal(cards);

        public bool IsBust()
            => this.GetScores().All(x => x > 21);

        public bool IsBlackJack()
            => this.GetScores().Any(x => x == 21);

        public void InvokeOnResult(GameResult result)
            => OnResult?.Invoke(result);

        protected void InvokeOnCardAdd(Card card)
            => OnCardAdd?.Invoke(card);

    }

    public enum PlayerStatus
    {
        Win,
        Lose,
        Push,
        Bust
    }
}
