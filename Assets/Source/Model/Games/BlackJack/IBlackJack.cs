using System;
using Assets.Source.Model.Games.BlackJack.Users;

namespace Assets.Source.Model.Games.BlackJack
{
    public interface IBlackJack
    {
        event Action OnGameEnd;
        event Action<Card> OnDillerUpHiddenCard;

        bool isGame { get; }

        IUser[] players { get; }
        IUser dealer { get; }

        void Init(int playerCount);
        void Start();
        void Turn(IUser user, BlackJackTurn turn);
    }
}
