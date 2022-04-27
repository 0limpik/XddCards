using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using UnityEngine;
using Xdd.Network.Clients.Lobby;
using Xdd.Scripts.Base;

namespace Xdd.Network.Clients
{
    internal class GrpcClient<T> where T : ClientBase<T>
    {
        protected T Client = GetNewClient();
        protected T NewClient => GetNewClient();

        public void CreateListener<TResponse>(
            Func<T, AsyncServerStreamingCall<TResponse>> init,
            Action<TResponse> recive)
        {
            Task.Run(async () =>
            {
                try
                {
                    using (var call = init?.Invoke(NewClient) ?? throw new ArgumentException())
                    {
                        while (await call.ResponseStream.MoveNext(new CancellationToken()))
                        {
                            var response = call.ResponseStream.Current;
                            ;
                            UnityMainThreadDispatcher.Instance()
                                .Enqueue(() => recive?.Invoke(response));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(this.GetType().FullName);
                    Debug.LogException(ex);
                }
            })
            .Forget();
        }

        private static GrpcChannel Channel = NewChannel;

        private static GrpcChannel NewChannel
        {
            get
            {
                var channel = GrpcChannel.ForAddress("http://localhost:5000", new GrpcChannelOptions
                {
                    HttpHandler = new GrpcWebHandler(new HttpClientHandler()),
                });

                return channel;
            }
        }

        public static T GetClient()
        {
            return (T)Activator.CreateInstance(typeof(T), Channel.Intercept(new ExampleInterceptor()));
        }

        public static T GetNewClient()
        {
            return (T)Activator.CreateInstance(typeof(T), NewChannel.Intercept(new ExampleInterceptor()));
        }
    }
    public class ExampleInterceptor : Interceptor
    {
        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            return continuation(GetAuthContext(context));
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            return continuation(GetAuthContext(context));
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            return continuation(request, GetAuthContext(context));
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            return continuation(request, GetAuthContext(context));
        }

        private ClientInterceptorContext<TRequest, TResponse> GetAuthContext<TRequest, TResponse>(
            ClientInterceptorContext<TRequest, TResponse> context) where TRequest : class where TResponse : class
        {
            return new ClientInterceptorContext<TRequest, TResponse>(context.Method,
                context.Host, context.Options.WithHeaders(AuthClient.AuthHeader));
        }
    }
}
