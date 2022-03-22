using System;
using Assets.Source.Model.Games.BlackJack.Users;

namespace Assets.Source.Model.Games.BlackJack
{
    public interface IBlackJack
    {
        IUser player { get; }
        IUser diller { get; }
        void Start();
        void Hit();
        void Stand();

        event Action<GameResult> OnGameResult;
        event Action<Card> OnDillerUpHiddenCard;
    }
}
