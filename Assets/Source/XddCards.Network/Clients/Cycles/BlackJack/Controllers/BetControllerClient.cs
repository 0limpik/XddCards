using Xdd.Model.Cycles.BlackJack.Controllers;
using XddCards.Grpc.Cycles.BlackJack.Controllers;

namespace Xdd.Network.Clients.Cycles.BlackJack.Controllers
{
    internal class BetControllerClient : ControllerClient<BetControllerGrpc.BetControllerGrpcClient>, IBetController
    {
    }
}
