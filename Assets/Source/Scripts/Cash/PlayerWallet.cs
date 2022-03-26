using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Scripts.Cash
{
    public class PlayerWallet
    {
        public event Action OnChange;

        public decimal Cash { get; private set; }

        public List<Bet> Bets { get; private set; } = new List<Bet>();

        public decimal All => Cash + AllBets;
        public decimal AllBets => Bets.Select(x => x.Cash).Sum();

        public PlayerWallet(decimal cache)
        {
            this.Cash = cache;
        }

        public Bet Reserve(decimal bet)
        {
            if (Cash - bet < 0)
                throw new ArgumentException();

            Cash -= bet;

            var betC = new Bet(bet);
            Bets.Add(betC);

            OnChange?.Invoke();

            return betC;
        }

        public void Take(Bet bet)
        {
            if(!Bets.Remove(bet))
                throw new ArgumentException();

            OnChange?.Invoke();
        }

        public void Give(Bet bet)
        {
            Cash += bet.Cash * 2;

            if (!Bets.Remove(bet))
                throw new ArgumentException();

            OnChange?.Invoke();
        }

        public void Cancel(Bet bet)
        {
            Cash += bet.Cash;

            if (!Bets.Remove(bet))
                throw new ArgumentException();

            OnChange?.Invoke();
        }
    }

    public class Bet
    {
        public decimal Cash { get; private set; }

        public Bet(decimal bet)
        {
            Cash = bet;
        }
    }
}
