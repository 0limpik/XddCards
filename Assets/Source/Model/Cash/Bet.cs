using System;
using UnityEngine;

namespace Xdd.Model.Cash
{
    [Serializable]
    public class Bet
    {
        [field: SerializeField] public decimal Amount { get; set; }

        public Bet(decimal bet)
        {
            Amount = bet;
        }
    }
}
