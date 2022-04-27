using System;
using Grpc.Core;
using Xdd.Model.Cycles.BlackJack;

namespace Xdd.Network.Clients.Cycles.BlackJack.Controllers
{
    internal class ControllerClient<T> : GrpcClient<T>, IState where T : ClientBase<T>
    {
        public event Action<bool> OnChangeExecute;

        public void OnChangeExecuteInvoke(bool execute)
        {
            OnChangeExecute?.Invoke(execute);
        }
    }
}
