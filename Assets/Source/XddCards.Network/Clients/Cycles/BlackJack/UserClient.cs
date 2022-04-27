using System.Collections.Generic;
using System.Threading.Tasks;
using Xdd.Model.Cycles.BlackJack;
using XddCards.Grpc.Cycles.BlackJack;
using Hand = XddCards.Grpc.Cycles.BlackJack.Hand;

namespace Xdd.Network.Clients.Cycles.BlackJack
{
    internal class UserClient : GrpcClient<UserGrpc.UserGrpcClient>, IUser
    {
        public int Id { get; private set; }
        public string NickName { get; private set; }

        public decimal Cash { get; private set; }

        public bool IsSelf { get; private set; }

        public IHand[] Hands => _Hands.ToArray();
        private List<HandClient> _Hands = new();

        public decimal Amount { get; private set; }

        public static Task<UserClient> New(int id, string nickname, bool isSelf = false)
            => new UserClient(id, nickname, isSelf).Init();

        private UserClient(int id, string nickname, bool isSelf = false)
        {
            this.Id = id;
            this.NickName = nickname;

            this.IsSelf = isSelf;
        }

        private async Task<UserClient> Init()
        {
            var response = await Client.GetCashAsync(new CashRequest());
            Cash = decimal.Parse(response.Cash);

            return this;
        }

        public async ValueTask Take(IHand hand)
        {
            var model = hand as HandClient;

            await Client.TakeHandAsync(new TakeHandRequest { Hand = new Hand { Id = model.Id } });

            _Hands.Add(model);
        }

        public async ValueTask Release(IHand hand)
        {
            var model = hand as HandClient;

            await Client.ReleaseHandAsync(new ReleaseHandRequest { Hand = new Hand { Id = model.Id } });

            _Hands.Remove(model);
        }

        public async ValueTask Bet(decimal amount)
        {
            await Client.BetAsync(new BetRequest { Amount = amount.ToString() });
            Amount = amount;
            Cash -= amount;
        }

        public bool CanBet(decimal amount)
            => Cash - amount > 0;
    }
}
