using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xdd.Model.Cash;
using Xdd.Model.Cycles.BlackJack.Controllers;

namespace Xdd.Model.Cycles.BlackJack
{
    public class BJCycle
    {
        public event Action<AState> OnStateChange;

        private User[] users;
        private List<Hand> hands;

        public User[] Users => users.ToArray();
        public Hand[] Hands => hands.ToArray();

        public HandController handController { get; private set; }
        public BetController betController { get; private set; }
        public GameController gameController { get; private set; }

        private IEnumerable<AState> States
        {
            get
            {
                yield return handController;
                yield return betController;
                yield return gameController;
            }
        }

        public void Init(Wallet[] wallets, int handCount)
        {
            users = new User[wallets.Length];
            hands = new(handCount);

            for (int i = 0; i < wallets.Length; i++)
            {
                users[i] = new User(wallets[i]);
            }

            foreach (var i in Enumerable.Range(0, handCount))
            {
                hands.Add(new Hand());
            }

            Reset();
        }

        public void Start()
        {
            handController.IsExecute = true;
            OnStateChange?.Invoke(handController);
        }

        public bool CanSwitchState(out string message)
        {
            message = null;
            AState prevState = gameController;
            foreach (var state in States)
            {
                if (prevState.IsExecute)
                {
                    if (!prevState.CanExit(out message))
                        return false;
                    if (!state.CanEnter(out message))
                        return false;

                    return true;
                }
                prevState = state;
            }
            throw new Exception("active state not found");
        }

        public void SwitchState()
        {
            AState prevState = gameController;
            foreach (var state in States)
            {
                if (prevState.IsExecute)
                {
                    prevState.IsExecute = false;
                    state.IsExecute = true;
                    OnStateChange?.Invoke(state);
                    return;
                }
                prevState = state;
            }
            throw new Exception("active state not found");
        }

        public void Reset()
        {
            handController = new HandController(users, hands);
            betController = new BetController(users);
            gameController = new GameController(users);

            foreach (var user in users)
            {
                user.handController = handController;
                user.betController = betController;
                user.gameController = gameController;

                foreach (var hand in user.hands)
                {
                    hand.gameController = gameController;
                }
            }

            foreach (var hand in hands)
            {
                hand.gameController = gameController;
            }

            foreach (var state in States)
            {
                state.OnIncorectState += (state) =>
                    Debug.LogAssertion($"Incorect {state} when active {States.FirstOrDefault(x => x.IsExecute)?.ToString() ?? "null"}");
            }
        }
    }
}
