using System;
using Assets.Source.Model.Games.BlackJack.Users;

namespace Assets.Source.Model.Games.BlackJack
{
    public interface IBlackJack
    {
        event Action OnGameEnd;
        event Action<Card> OnDillerUpHiddenCard;

        bool isGame { get; }

        IPlayer[] players { get; }
        IPlayer dealer { get; }

        void Init(int playerCount);
        void Start();

        void Hit(IPlayer player);
        void Stand(IPlayer player);
    }
}
