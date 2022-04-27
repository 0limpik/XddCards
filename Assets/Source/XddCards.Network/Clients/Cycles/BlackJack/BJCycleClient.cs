using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xdd.Model.Cash;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Cycles.BlackJack.Controllers;
using Xdd.Network.Clients.Cycles.BlackJack.Controllers;
using XddCards.Grpc.Cycles.BlackJack;
using State = XddCards.Grpc.Cycles.BlackJack.OnStateChangeResponse.Types.State;

namespace Xdd.Network.Clients.Cycles.BlackJack
{
    internal class BJCycleClient : GrpcClient<BJCycleGrpc.BJCycleGrpcClient>, IBJCycleController
    {
        private List<UserClient> users = new();
        private UserClient user;

        public IHandController HandController => _HandController;
        public HandControllerClient _HandController;

        public IBetController BetController => _BetController;
        public BetControllerClient _BetController;

        public IGameController GameController => _GameController;
        public GameControllerClient _GameController;

        public event Action<string> OnMessage;

        public static Task<BJCycleClient> New(UserClient _user)
            => new BJCycleClient(_user).Init();

        private BJCycleClient(UserClient _user)
        {
            user = _user;
            users.Add(user);

            CreateListener(
                   client => client.OnStateChange(new OnStateChangeRequest()),
                   response =>
                   {
                       switch (response.State)
                       {
                           case State.Hand:
                               _HandController.OnChangeExecuteInvoke(response.Execute);
                               break;
                           case State.Bet:
                               _BetController.OnChangeExecuteInvoke(response.Execute);
                               break;
                           case State.Game:
                               _GameController.OnChangeExecuteInvoke(response.Execute);
                               break;
                           default:
                               throw new ArgumentException();
                       }
                   });

            CreateListener(
                    client => client.OnSwitchMessage(new OnSwitchMessageRequest()),
                    response =>
                    {
                        OnMessage.Invoke(response.Message);
                        _HandController.OnChangeExecuteInvoke(false);
                        _BetController.OnChangeExecuteInvoke(false);
                        _GameController.OnChangeExecuteInvoke(false);
                    });
        }

        private async Task<BJCycleClient> Init()
        {
            _HandController = new HandControllerClient(users);
            _BetController = new BetControllerClient();
            _GameController = await GameControllerClient.New(_HandController._Hands);
            return this;
        }


        public IUser AddUser(Wallet wallet)
        {
            throw new NotImplementedException();
        }

        public bool CanSwitchState(out string message)
        {
            throw new NotImplementedException();
        }

        public void Init(int handCount)
        {
            throw new NotImplementedException();
        }

        public void RemoveUser(IUser user)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void SwitchState()
        {
            throw new NotImplementedException();
        }
    }
}
