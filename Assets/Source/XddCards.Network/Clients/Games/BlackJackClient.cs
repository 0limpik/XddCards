using System.Linq;
using System.Threading.Tasks;
using Xdd.Network.Clients.Cycles.BlackJack;
using Xdd.Network.Clients.Lobby;
using XddCards.Grpc.Games.BlackJack;

namespace Xdd.Network.Clients.Games
{
    internal class BlackJackClient : GrpcClient<BlackJackGrpc.BlackJackGrpcClient>
    {
        public async Task<UserClient> Create()
        {
            var game = await Client.CreateAsync(new CreateRequest());

            var userClient = await UserClient.New(game.User.Id, game.User.Nickname, true);

            return userClient;
        }

        public async Task<UserClient> Connect(int id)
        {
            var game = await Client.ConnectAsync(new ConnectRequest { Game = new Game { Id = id } });

            var userClient = await UserClient.New(game.User.Id, game.User.Nickname, true);

            return userClient;
        }

        public async Task<int[]> GetAll()
        {
            var games = await Client.GetAllAsync(new AllRequest());

            return games.Games.Select(x => x.Id).ToArray();
        }
    }
}
