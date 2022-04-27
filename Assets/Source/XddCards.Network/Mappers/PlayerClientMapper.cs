using System;
using Xdd.Model.Enums;
using Xdd.Model.Games;
using Xdd.Model.Games.BlackJack;
using Xdd.Model.Games.BlackJack.Users;
using Xdd.Scripts.Cards;
using XddCards.Grpc.Games;
using XddCards.Grpc.Games.BlackJack;
using OnResultReply = XddCards.Grpc.Games.BlackJack.OnResultReply;
using OnResultReplyControllers = XddCards.Grpc.Cycles.BlackJack.Controllers.OnResultReply;

namespace Xdd.Network.XddCards.Network.Mappers
{
    internal static class PlayerClientMapper
    {
        public static ICard Map(this CardMessage card)
            => new Card { rank = card.Rank.Map(), suit = card.Suit.Map() };

        public static PlayerStatus? Map(this StatusReply.Types.PlayerStatus status)
        {
            switch (status)
            {
                case StatusReply.Types.PlayerStatus.Empty:
                    return null;
                case StatusReply.Types.PlayerStatus.Win:
                    return PlayerStatus.Win;
                case StatusReply.Types.PlayerStatus.Lose:
                    return PlayerStatus.Lose;
                case StatusReply.Types.PlayerStatus.Push:
                    return PlayerStatus.Push;
                case StatusReply.Types.PlayerStatus.Bust:
                    return PlayerStatus.Bust;
            }
            throw new ArgumentException();
        }

        public static Ranks Map(this CardMessage.Types.Ranks rank) =>
           rank switch
           {
               CardMessage.Types.Ranks.Ace => Ranks.Ace,
               CardMessage.Types.Ranks.Two => Ranks.Two,
               CardMessage.Types.Ranks.Three => Ranks.Three,
               CardMessage.Types.Ranks.Four => Ranks.Four,
               CardMessage.Types.Ranks.Five => Ranks.Five,
               CardMessage.Types.Ranks.Six => Ranks.Six,
               CardMessage.Types.Ranks.Seven => Ranks.Seven,
               CardMessage.Types.Ranks.Eight => Ranks.Eight,
               CardMessage.Types.Ranks.Nine => Ranks.Nine,
               CardMessage.Types.Ranks.Ten => Ranks.Ten,
               CardMessage.Types.Ranks.Jack => Ranks.Jack,
               CardMessage.Types.Ranks.Queen => Ranks.Queen,
               CardMessage.Types.Ranks.King => Ranks.King,
               _ => throw new ArgumentException(),
           };

        public static Suits Map(this CardMessage.Types.Suits suit)
            => suit switch
            {
                CardMessage.Types.Suits.Clubs => Suits.Clubs,
                CardMessage.Types.Suits.Diamonds => Suits.Diamonds,
                CardMessage.Types.Suits.Hearts => Suits.Hearts,
                CardMessage.Types.Suits.Spades => Suits.Spades,
                _ => throw new ArgumentException(),
            };

        public static GameResult Map(this OnResultReply.Types.GameResult result) =>
            result switch
            {
                OnResultReply.Types.GameResult.Win => GameResult.Win,
                OnResultReply.Types.GameResult.Lose => GameResult.Lose,
                OnResultReply.Types.GameResult.Push => GameResult.Push,
                _ => throw new ArgumentException(),
            };

        public static GameResult Map(this OnResultReplyControllers.Types.GameResult result) =>
            result switch
            {
                OnResultReplyControllers.Types.GameResult.Win => GameResult.Win,
                OnResultReplyControllers.Types.GameResult.Lose => GameResult.Lose,
                OnResultReplyControllers.Types.GameResult.Push => GameResult.Push,
                _ => throw new ArgumentException(),
            };
    }
}
