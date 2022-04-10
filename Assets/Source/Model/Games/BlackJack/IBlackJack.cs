using System;
using Xdd.Model.Games.BlackJack.Users;

namespace Xdd.Model.Games.BlackJack
{
    public interface IBlackJack
    {
        event Action OnGameEnd;
        event Action<ICard> OnDillerUpHiddenCard;

        bool isGame { get; }

        IPlayer[] players { get; }
        IPlayer dealer { get; }

        void Init(int playerCount);
        void Start();

        void Hit(IPlayer player);
        void Stand(IPlayer player);
    }
}
