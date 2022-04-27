using System.Threading.Tasks;
using XddCards.Grpc.Games;

namespace Xdd.Network.Clients.Lobby
{
    internal class GamesClient : GrpcClient<GamesGrpc.GamesGrpcClient>
    {
        public async Task<int> CreateGame()
        {
            var game = await Client.CreateBlackJackAsync(new BlackJackRequest());

            return game.Id;
        }

        public async Task<int> CreateBJCycle()
        {
            var game = await Client.CreateBJCycleAsync(new BJCycleRequest());

            return game.Id;
        }
    }
}
