using System;
using System.Collections.Generic;

namespace Xdd.Model.Games.BlackJack.Users
{
    public interface IPlayer
    {
        event Action<Card> OnCardAdd;
        event Action<GameResult> OnResult;
        bool CanTurn { get; }
        IEnumerable<int> GetScores();
        PlayerStatus? GetStatus();
    }
}
