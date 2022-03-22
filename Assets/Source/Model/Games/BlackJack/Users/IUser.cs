using System;
using System.Collections.Generic;

namespace Assets.Source.Model.Games.BlackJack.Users
{
    public interface IUser
    {
        event Action<Card> OnCardAdd;
        void AddCard(Card card);
        void Reset();
        IEnumerable<int> GetScores();
        PlayerStatus? GetStatus();
    }

    public interface IUserTurn
    {
        void Hit();
        void Stand();
    }
}
