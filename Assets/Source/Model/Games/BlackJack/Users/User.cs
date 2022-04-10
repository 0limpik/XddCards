using System;
using System.Collections.Generic;
using System.Linq;

namespace Xdd.Model.Games.BlackJack.Users
{
    internal abstract class User : IPlayer
    {
        public event Action<ICard> OnCardAdd;
        public event Action<GameResult> OnResult;

        protected List<ICard> cards = new List<ICard>();
        private Func<IEnumerable<ICard>, IEnumerable<int>> GetScoresIternal;

        public bool CanTurn { get; set; } = true;

        public User(Func<IEnumerable<ICard>, IEnumerable<int>> GetScores)
        {
            this.GetScoresIternal = GetScores;
        }

        public virtual void AddCard(ICard card)
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

        protected void InvokeOnCardAdd(ICard card)
            => OnCardAdd?.Invoke(card);

        public abstract PlayerStatus? GetStatus();
    }

    public enum PlayerStatus
    {
        Win,
        Lose,
        Push,
        Bust
    }
}
