using System.Threading.Tasks;
using Grpc.Core;
using XddCards.Grpc.Auth;

namespace Xdd.Network.Clients.Lobby
{
    internal static class AuthClient
    {
        internal static Metadata AuthHeader;
        internal static string Token { get; private set; }

        public static async Task Init(string nickname)
        {
            var response = await GrpcClient<AuthGrpc.AuthGrpcClient>.GetNewClient().AuthAsync(new AuthRequest { Nickname = nickname });
            Token = response.Token;
            AuthHeader = new Metadata();
            AuthHeader.Add("Authorization", $"Bearer {Token}");
        }
    }
}
