using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Source.Network.Clients.Lobby;
using Grpc.Core;
using UnityEngine;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Games;
using Xdd.Model.Games.BlackJack.Users;
using Xdd.Scripts.Base;
using XddCards.Grpc.Games.BlackJack;

namespace Assets.Source.Network.Clients.Games.BlackJack
{
    internal class GameClient
    {
        public static GameGrpc.GameGrpcClient client = Client.GetClient<GameGrpc.GameGrpcClient>();

        public bool isGame => client.IsGame(new IsGameRequest()).Value;

        public IPlayer[] players => _players.ToArray();
        public IHand[] playersHand => _players.ToArray();
        private List<PlayerClient> _players = new List<PlayerClient>();

        public IPlayer dealer => _dealer;
        public IHand dealerHand => _dealer;
        public PlayerClient _dealer;

        public event Action OnGameEnd;
        public event Action<ICard> OnDillerUpHiddenCard;

        public GameClient()
        {
            Task.Run(async () =>
            {
                var call = Client.GetNewClient<GameGrpc.GameGrpcClient>().OnGameEnd(new GameEndRequest(), AuthClient.AuthHeader);
                while (await call.ResponseStream.MoveNext())
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(
                        () => OnGameEnd?.Invoke());
                }
            }).Forget();

            Task.Run(async () =>
            {
                var call = Client.GetNewClient<GameGrpc.GameGrpcClient>().OnDillerUpHiddenCard(new OnDillerUpHiddenCardRequest(), AuthClient.AuthHeader);
                while (await call.ResponseStream.MoveNext())
                {
                    var card = call.ResponseStream.Current.Card.Map();

                    Debug.Log($"Diller Up {card}");
                    UnityMainThreadDispatcher.Instance().Enqueue(
                        () => OnDillerUpHiddenCard?.Invoke(card));
                }
            }).Forget();
        }

        public async Task Init(int playerCount)
        {
            await client.InitAsync(new InitRequest { PlayerCount = playerCount }, AuthClient.AuthHeader);

            _players.Clear();

            var players = await client.PlayersAsync(new PlayersRequest(), AuthClient.AuthHeader);

            foreach (var player in players.Players)
            {
                var pl = new PlayerClient(player.Id);

                _players.Add(pl);
            }
            _dealer = new PlayerClient(client.Dealer(new DealerRequest(), AuthClient.AuthHeader).Player.Id);
        }

        public async Task Start()
        {
            await client.StartAsync(new StartRequest(), AuthClient.AuthHeader);
        }
    }
}
