﻿using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Xdd.Model.Cash;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Scripts.Base;
using Xdd.Scripts.Hands;
using Xdd.Scripts.UI;

namespace Xdd.Scripts.BlackJack
{
    internal class GameManager : MonoBehaviour
    {
        public event Action<float> OnHand;
        public event Action<float> OnBet;
        public event Action OnWait;

        public float handTakeDelay;
        public float betPlaceDelay;

        [SerializeField] private GameUIScript uiScript;
        [SerializeField] private BusyHandsScript bysyHands;
        [SerializeField] private GameScript gameScript;

        private BJCycle cycle;
        private Wallet wallet;

        private bool isGame;

        void Awake()
        {
            cycle = new BJCycle();
            wallet = new Wallet(10000);
            cycle.Init(new Wallet[] { wallet }, 6);

            bysyHands.OnInteraction += (x) => OnInteraction(x).Forget();
        }

        void Start()
        {
            Init();
            cycle.Start();
            bysyHands.Lock(false);
            OnWait?.Invoke();
        }

        private async Task OnInteraction(HandScript hand)
        {
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

        private void Init()
        {
            var player = cycle.Users.First();

            bysyHands.InitCycle(cycle, player);
            uiScript.InitCycle(cycle, player);
            gameScript.InitCycle(cycle, player);
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

            Init();

            bysyHands.Lock(false);
            OnWait?.Invoke();

            isGame = false;

            return false;
        }
    }
}