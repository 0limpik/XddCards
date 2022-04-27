using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Xdd.Model.Cash;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Scripts.Base;
using Xdd.Scripts.Hands;
using Xdd.Scripts.UI;
using Xdd.Scripts.UI.BlackJack;

namespace Xdd.Scripts.BlackJack
{
    public class GameManager : MonoBehaviour, ICycleRequired
    {
        public event Action<float> OnHand;
        public event Action<float> OnBet;
        public event Action OnWait;

        public float handTakeDelay;
        public float betPlaceDelay;

        [SerializeField] private GameUIScript uiScript;
        [SerializeField] private BusyHandsScript bysyHands;
        [SerializeField] private GameScript gameScript;

        public IBJCycleController cycle;
        private IUser user;

        private bool isGame;

        void Awake()
        {
            //cycle = BJCycleFabric.Create();
        }

        public void InitCycle(IBJCycle cycle, IUser user)
        {
            this.user = user;

            //cycle.Init(6);
            bysyHands.OnInteraction += (x) => OnInteraction(x).Forget();

            bysyHands.InitCycle(cycle, user);
            uiScript.InitCycle(cycle, user);
            gameScript.InitCycle(cycle, user);

            //cycle.Start();
            bysyHands.Lock(false);
            OnWait?.Invoke();
        }

        private async Task OnInteraction(HandScript hand)
        {
            return;

            if (!isGame)
                while (hand.IsBusy || bysyHands.Hands.Count() > 0)
                {
                    isGame = true;

                    cycle.Start();

                    OnHand?.Invoke(handTakeDelay);
                    await TaskEx.Delay(handTakeDelay);

                    if (!TrySwitchState())
                        continue;

                    OnBet?.Invoke(betPlaceDelay);
                    await TaskEx.Delay(betPlaceDelay);

                    if (!TrySwitchState())
                        continue;

                    await gameScript.WaitEndGame();

                    if (!TrySwitchState())
                        continue;

                    isGame = false;
                }
        }

        private bool TrySwitchState()
        {
            if (cycle.CanSwitchState(out string message))
            {
                cycle.SwitchState();
                return true;
            }

            uiScript.DisplayMessage(message, Color.red);

            cycle.Reset();

            bysyHands.Lock(false);
            OnWait?.Invoke();

            isGame = false;

            return false;
        }
    }
}