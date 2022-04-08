using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Source.Model.Cash;
using Assets.Source.Model.Cycles.BlackJack.Controllers;
using UnityEngine;

namespace Assets.Source.Model.Cycles.BlackJack
{
    public class BJCycle
    {
        public event Action<State> OnStateChange;

        private User[] players;
        private List<Hand> hands;

        public User[] Players => players.ToArray();
        public Hand[] Hands => hands.ToArray();

        public HandController handController { get; private set; }
        public BetController betController { get; private set; }
        public GameController gameController { get; private set; }

        private IEnumerable<State> States
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
            players = new User[wallets.Length];
            hands = new(handCount);

            for (int i = 0; i < wallets.Length; i++)
            {
                players[i] = new User(wallets[i]);
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
            State prevState = gameController;
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
            State prevState = gameController;
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
            handController = new HandController(players, hands);
            betController = new BetController(players);
            gameController = new GameController(players);

            foreach (var player in players)
            {
                player.handController = handController;
                player.betController = betController;
                player.gameController = gameController;

                foreach (var hand in player.hands)
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
