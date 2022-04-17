using System;
using System.Net.Http;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;

namespace Assets.Source.Network.Clients
{
    internal static class Client
    {
        static GrpcChannel channel = newChannel;

        static GrpcChannel newChannel => GrpcChannel.ForAddress("http://localhost:5000", new GrpcChannelOptions
        {
            HttpHandler = new GrpcWebHandler(new HttpClientHandler()),
        });

        public static T GetClient<T>() where T : ClientBase<T>
        {
            return (T)Activator.CreateInstance(typeof(T), channel);
        }
        
        public static T GetNewClient<T>() where T : ClientBase<T>
        {
            return (T)Activator.CreateInstance(typeof(T), newChannel);
        }
    }
}
