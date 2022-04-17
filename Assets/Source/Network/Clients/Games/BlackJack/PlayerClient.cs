using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Source.Network.Clients.Lobby;
using Grpc.Core;
using UnityEngine;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Enums;
using Xdd.Model.Games;
using Xdd.Model.Games.BlackJack;
using Xdd.Model.Games.BlackJack.Users;
using Xdd.Scripts.Base;
using Xdd.Scripts.Cards;
using XddCards.Grpc.Games;
using XddCards.Grpc.Games.BlackJack;

namespace Assets.Source.Network.Clients.Games.BlackJack
{
    internal class PlayerClient : IPlayer, IHand
    {
        public static PlayerGrpc.PlayerGrpcClient client = Client.GetClient<PlayerGrpc.PlayerGrpcClient>();

        public int Id { get; private set; }

        public bool CanTurn => client.CanTurn(new Player { Id = Id }, AuthClient.AuthHeader).Value;

        public PlayerStatus? Status => client.GetStatus(new Player { Id = Id }, AuthClient.AuthHeader).Status.Map();

        public event Action<ICard> OnCardAdd;
        public event Action<GameResult> OnResult;

        public PlayerClient(int id)
        {
            Debug.Log($"Init {id}");
            this.Id = id;
            //Task.Run(async () =>
            //{
            //    var call = Client.GetNewClient<PlayerGrpc.PlayerGrpcClient>().OnCardAdd(new OnCardAddRequest { Player = new PlayerReply { Id = Id } }, AuthClient.AuthHeader);
            //    while (await call.ResponseStream.MoveNext().ConfigureAwait(true))
            //    {
            //        var card = call.ResponseStream.Current.Card.Map();

            //        Debug.Log($"{Id} Card: {card}");
            //        UnityMainThreadDispatcher.Instance().Enqueue(
            //            () => OnCardAdd?.Invoke(card));
            //    }
            //}).Forget();
            //Task.Run(async () =>
            //{
            //    var call = Client.GetNewClient<PlayerGrpc.PlayerGrpcClient>().OnResult(new OnResultRequest { Player = new PlayerReply { Id = Id } }, AuthClient.AuthHeader);
            //    while (await call.ResponseStream.MoveNext().ConfigureAwait(true))
            //    {
            //        Debug.Log($"{Id} Result");
            //        UnityMainThreadDispatcher.Instance().Enqueue(
            //            () => OnResult?.Invoke(call.ResponseStream.Current.Result.Map()));
            //    }
            //}).Forget();
        }

        public IEnumerable<int> GetScores()
        {
            return client.GetScores(new Player { Id = Id }, AuthClient.AuthHeader).Scores;
        }

        public PlayerStatus? GetStatus()
        {
            return client.GetStatus(new Player { Id = Id }, AuthClient.AuthHeader).Status.Map();
        }

        public void Hit()
        {
            client.Hit(new Player { Id = Id }, AuthClient.AuthHeader);
        }

        public void Stand()
        {
            client.Stand(new Player { Id = Id }, AuthClient.AuthHeader);
        }

        public void DoubleUp()
        {
            client.Hit(new Player { Id = Id }, AuthClient.AuthHeader);
            client.Stand(new Player { Id = Id }, AuthClient.AuthHeader);
        }
    }

    public static class PlayerClientExtension
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
        public static Ranks Map(this XddCards.Grpc.Games.CardMessage.Types.Ranks rank) =>
           rank switch
           {
               XddCards.Grpc.Games.CardMessage.Types.Ranks.Ace => Ranks.Ace,
               XddCards.Grpc.Games.CardMessage.Types.Ranks.Two => Ranks.Two,
               XddCards.Grpc.Games.CardMessage.Types.Ranks.Three => Ranks.Three,
               XddCards.Grpc.Games.CardMessage.Types.Ranks.Four => Ranks.Four,
               XddCards.Grpc.Games.CardMessage.Types.Ranks.Five => Ranks.Five,
               XddCards.Grpc.Games.CardMessage.Types.Ranks.Six => Ranks.Six,
               XddCards.Grpc.Games.CardMessage.Types.Ranks.Seven => Ranks.Seven,
               XddCards.Grpc.Games.CardMessage.Types.Ranks.Eight => Ranks.Eight,
               XddCards.Grpc.Games.CardMessage.Types.Ranks.Nine => Ranks.Nine,
               XddCards.Grpc.Games.CardMessage.Types.Ranks.Ten => Ranks.Ten,
               XddCards.Grpc.Games.CardMessage.Types.Ranks.Jack => Ranks.Jack,
               XddCards.Grpc.Games.CardMessage.Types.Ranks.Queen => Ranks.Queen,
               XddCards.Grpc.Games.CardMessage.Types.Ranks.King => Ranks.King,
               _ => throw new ArgumentException(),
           };

        public static Suits Map(this XddCards.Grpc.Games.CardMessage.Types.Suits suit)
            => suit switch
            {
                XddCards.Grpc.Games.CardMessage.Types.Suits.Clubs => Suits.Clubs,
                XddCards.Grpc.Games.CardMessage.Types.Suits.Diamonds => Suits.Diamonds,
                XddCards.Grpc.Games.CardMessage.Types.Suits.Hearts => Suits.Hearts,
                XddCards.Grpc.Games.CardMessage.Types.Suits.Spades => Suits.Spades,
                _ => throw new ArgumentException(),
            };

        public static GameResult Map(this XddCards.Grpc.Games.BlackJack.OnResultReply.Types.GameResult result) =>
            result switch
            {
                XddCards.Grpc.Games.BlackJack.OnResultReply.Types.GameResult.Win => GameResult.Win,
                XddCards.Grpc.Games.BlackJack.OnResultReply.Types.GameResult.Lose => GameResult.Lose,
                XddCards.Grpc.Games.BlackJack.OnResultReply.Types.GameResult.Push => GameResult.Push,
                _ => throw new ArgumentException(),
            };
    }
}
