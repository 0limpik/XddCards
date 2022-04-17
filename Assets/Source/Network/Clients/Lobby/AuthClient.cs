using Grpc.Core;
using UnityEngine;
using XddCards.Grpc.Auth;

namespace Assets.Source.Network.Clients.Lobby
{
    internal static class AuthClient
    {
        static AuthGrpc.AuthGrpcClient client = Client.GetClient<AuthGrpc.AuthGrpcClient>();

        static string token;

        public static Metadata AuthHeader;

        public static void Init(string nickname)
        {
            var response = client.Auth(new AuthRequest { Nickname = nickname });
            token = response.Token;

            AuthHeader = new Metadata();
            AuthHeader.Add("Authorization", $"Bearer {token}");
        }
    }
}
