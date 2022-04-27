using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Games;
using Xdd.Model.Games.BlackJack;
using Xdd.Model.Games.BlackJack.Users;
using XddCards.Grpc.Cycles.BlackJack;
using Hand = XddCards.Grpc.Cycles.BlackJack.Hand;

namespace Xdd.Network.Clients.Cycles.BlackJack
{
    internal class HandClient : GrpcClient<HandGrpc.HandGrpcClient>, IHand
    {
        public int Id { get; private set; }

        public UserClient owner;

        public bool IsPlaying => owner != null && owner.IsSelf;

        public bool CanTurn { get; set; }

        public PlayerStatus? Status { get; set; }

        public IEnumerable<int> Scores { get; set; }

        public event Action<ICard> OnCardAdd;
        public event Action<GameResult> OnResult;

        public HandClient(int id)
        {
            this.Id = id;
        }

        public async ValueTask DoubleUp()
        {
            await Client.DoubleUpAsync(new Hand { Id = this.Id });
            CanTurn = false;
        }

        public async ValueTask<bool> Hit()
        {
            var resonse = await Client.HitAsync(new Hand { Id = this.Id });
            CanTurn = resonse.CanTurn;
            return CanTurn;
        }

        public async ValueTask Stand()
        {
            await Client.StandAsync(new Hand { Id = this.Id });
            CanTurn = false;
        }

        public void OnCardAddInvoke(ICard card)
        {
            OnCardAdd?.Invoke(card);
        }

        public void OnResultInvoke(GameResult result)
        {
            OnResult?.Invoke(result);
            CanTurn = false;
        }
    }
}
