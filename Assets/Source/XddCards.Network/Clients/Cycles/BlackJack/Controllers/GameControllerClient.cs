using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Cycles.BlackJack.Controllers;
using Xdd.Model.Games;
using Xdd.Network.Clients.Games.BlackJack;
using Xdd.Network.XddCards.Network.Mappers;
using XddCards.Grpc.Cycles.BlackJack.Controllers;

namespace Xdd.Network.Clients.Cycles.BlackJack.Controllers
{
    internal class GameControllerClient : ControllerClient<GameControllerGrpc.GameControllerGrpcClient>, IGameController
    {
        public IHand DealerHand => _DealerHand;
        public HandClient _DealerHand;

        public IHand[] PlayerHands => _PlayerHands.ToArray();
        public List<HandClient> _PlayerHands;

        private IEnumerable<HandClient> AllHands => new List<HandClient>(_PlayerHands) { _DealerHand };

        public event Action<ICard> OnDillerUpHiddenCard;
        public event Action OnGameEnd;

        public static Task<GameControllerClient> New(List<HandClient> hands)
            => new GameControllerClient(hands).Init();

        private GameControllerClient(List<HandClient> hands)
        {
            OnChangeExecute += ChangeExecute;

            this._PlayerHands = hands;

            CreateListener(
                  client => client.OnGameEnd(new GameEndRequest()),
                  response => OnGameEnd?.Invoke());

            CreateListener(
                client => client.OnDillerUpHiddenCard(new OnDillerUpHiddenCardRequest()),
                response => OnDillerUpHiddenCard?.Invoke(response.Card.Map()));

            CreateListener(
                client => client.OnCardAdd(new OnCardAddRequest()),
                response =>
                {
                    var card = response.Card.Map();
                    var hand = AllHands.Single(x => x.Id == response.Hand.Id);

                    hand.CanTurn = response.CanTurn;
                    hand.OnCardAddInvoke(card);
                });

            CreateListener(
                client => client.OnResult(new OnResultRequest()),
                response =>
                {
                    var hand = AllHands.Single(x => x.Id == response.Hand.Id);

                    hand.Status = response.Status.Status.Map();
                    hand.Scores = response.Scores.Scores;
                    hand.OnResultInvoke(response.Result.Map());
                });
        }

        private async Task<GameControllerClient> Init()
        {
            var response = await Client.DealerAsync(new DealerRequest());

            _DealerHand = new HandClient(response.Hand.Id);

            return this;
        }

        private void ChangeExecute(bool execute)
        {
            if (execute)
                foreach (var hand in _PlayerHands)
                {
                    hand.CanTurn = true;
                }
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}
