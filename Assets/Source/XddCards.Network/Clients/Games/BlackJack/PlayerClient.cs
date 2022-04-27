using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Enums;
using Xdd.Model.Games;
using Xdd.Model.Games.BlackJack;
using Xdd.Model.Games.BlackJack.Users;
using Xdd.Scripts.Cards;
using XddCards.Grpc.Games;
using XddCards.Grpc.Games.BlackJack;

namespace Xdd.Network.Clients.Games.BlackJack
{
    internal class PlayerClient : GrpcClient<PlayerGrpc.PlayerGrpcClient>, IHand
    {
        public event Action<ICard> OnCardAdd;
        public event Action<GameResult> OnResult;

        public int Id { get; private set; }

        public bool CanTurn { get; private set; } = true;

        public PlayerStatus? Status { get; set; }

        public bool IsPlaying => throw new NotImplementedException();

        public IEnumerable<int> Scores => throw new NotImplementedException();

        public PlayerClient(int id)
        {
            this.Id = id;
        }

        public async ValueTask<bool> Hit()
        {
            var response = await Client.HitAsync(new Player { Id = Id });
            CanTurn = response.CanTurn;

            return CanTurn;
        }

        public ValueTask Stand()
        {
            Client.Stand(new Player { Id = Id });
            CanTurn = false;

            return new ValueTask();
        }

        public ValueTask DoubleUp()
        {
            CanTurn = Client.Hit(new Player { Id = Id }).CanTurn;

            if (CanTurn)
                Client.Stand(new Player { Id = Id });

            return new ValueTask();
        }

        public void OnCardAddInvoke(ICard card)
        {
            OnCardAdd?.Invoke(card);
        }

        public void OnResultInvoke(GameResult result)
        {
            OnResult?.Invoke(result);
            CanTurn = true;
        }
    }
}
