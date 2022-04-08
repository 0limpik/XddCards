using System;
using System.Collections.Generic;
using Assets.Source.Model.Cash;
using Assets.Source.Model.Cycles.BlackJack.Controllers;

namespace Assets.Source.Model.Cycles.BlackJack
{
    public class User
    {
        public event Action OnBet;

        public Wallet wallet;
        public decimal amount { get; internal set; }
        public List<Hand> hands = new List<Hand>();

        internal HandController handController;
        internal BetController betController;
        internal GameController gameController;

        public User(Wallet wallet)
        {
            this.wallet = wallet;
        }

        public void Take()
        {
            handController.Take(this);
        }

        public void Release()
        {
            handController.Release(this);
        }
        public bool CanBet(decimal amount)
        {
            return betController.CanBet(this, amount);
        }

        public void Bet(decimal amount)
        {
            betController.Bet(this, amount);
            OnBet?.Invoke();
        }
    }
}
