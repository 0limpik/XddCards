using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Games;
using Xdd.Scripts.Hands;
using Xdd.Scripts.UI.BlackJack.Hand;

namespace Xdd.Scripts.BlackJack
{
    [RequireComponent(typeof(HandStorageScript))]
    public class BJHandScript : MonoBehaviour
    {
        public event Action OnBet;
        public event Action OnDoubleUp;
        public event Action OnCardsChange;
        public event Action OnHandChange;

        [field: SerializeField]
        public bool isDealer { get; private set; }

        public decimal Amount { get; private set; }

        public Hand Hand
        {
            get => _Hand;
            set
            {
                _Hand = value;
                OnHandChange?.Invoke();
            }
        }
        public Hand _Hand;

        [SerializeField]
        private HandUIScript handUIScript;
        private HandStorageScript storage;

        public List<ICard> cards => storage.cards.Select(x => x.card).Cast<ICard>().ToList();

        void Awake()
        {
            storage = this.GetComponent<HandStorageScript>();
            storage.OnCardsChange += () => OnCardsChange?.Invoke();
            handUIScript.RegisterHand(this);
        }

        public void Bet(decimal amount)
        {
            this.Amount = amount;
            OnBet?.Invoke();
        }

        public void Hit()
            => Hand.Hit();

        public void Stand()
            => Hand.Stand();

        public void DoubleUp()
        {
            Hand.DoubleUp();
            OnDoubleUp?.Invoke();
        }

        public void InvokeOnCardsChange()
        {
            OnCardsChange?.Invoke();
        }
    }

    public enum BJTurn
    {
        Hit,
        Stand,
        DoubleUp
    }
}