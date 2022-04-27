using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Games;
using Xdd.Network.XddCards.Network.Mappers;
using XddCards.Grpc.Games.BlackJack;

namespace Xdd.Network.Clients.Games.BlackJack
{
    internal class GameClient : GrpcClient<GameGrpc.GameGrpcClient>
    {
        public bool isGame => Client.IsGame(new IsGameRequest()).Value;

        public IHand[] playersHand => _players.Where(x => x != _dealer).ToArray();
        private List<PlayerClient> _players = new List<PlayerClient>();

        public IHand dealerHand => _dealer;
        public PlayerClient _dealer;

        public event Action OnGameEnd;
        public event Action<ICard> OnDillerUpHiddenCard;

        public GameClient()
        {
            CreateListener(
                client => client.OnGameEnd(new GameEndRequest()),
                response => OnGameEnd?.Invoke());

            CreateListener(
                client => client.OnDillerUpHiddenCard(new OnDillerUpHiddenCardRequest()),
                response => OnDillerUpHiddenCard?.Invoke(response.Card.Map()));

            CreateListener(
                client => client.OnCardAdd(new OnCardAddRequest()),
                response => _players.Single(x => x.Id == response.Player.Id).OnCardAddInvoke(response.Card.Map()));

            CreateListener(
                client => client.OnResult(new OnResultRequest()),
                response =>
                {
                    var player = _players.Single(x => x.Id == response.Player.Id);

                    player.Status = response.Status.Status.Map();
                    player.OnResultInvoke(response.Result.Map());
                });
        }

        public async Task Init(int playerCount)
        {
            await Client.InitAsync(new InitRequest { PlayerCount = playerCount });

            _players.Clear();

            _dealer = new PlayerClient(Client.Dealer(new DealerRequest()).Player.Id);

            _players.Add(_dealer);

            var players = await Client.PlayersAsync(new PlayersRequest());

            foreach (var player in players.Players)
            {
                var pl = new PlayerClient(player.Id);

                _players.Add(pl);
            }
        }
    }
}
