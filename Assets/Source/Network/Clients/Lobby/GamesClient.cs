using System.Threading.Tasks;
using UnityEngine;
using XddCards.Grpc.Games;

namespace Assets.Source.Network.Clients.Lobby
{
    internal class GamesClient
    {
        public int id;

        static GamesGrpc.GamesGrpcClient client = Client.GetClient<GamesGrpc.GamesGrpcClient>();

        public async Task CreateGame()
        {
            var game = await client.CreateBlackJackAsync(new BlackJackRequest(), AuthClient.AuthHeader);

            id = game.Id;
        }
    }
}
