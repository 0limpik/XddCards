using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Source.Model.Cycles.BlackJack;
using Assets.Source.Model.Games;
using Assets.Source.Scripts.Hands;
using Assets.Source.Scripts.UI;
using UnityEngine;

namespace Assets.Source.Scripts.BlackJack
{
    [RequireComponent(typeof(HandStorageScript))]
    public class BJHandScript : MonoBehaviour
    {
        public event Action OnBet;
        public event Action OnDoubleUp;

        [field: SerializeField] public bool isDealer { get; private set; }

        public event Action OnCardsChange;
        public event Action<BJTurn> OnTurn;
        public event Action<Hand> OnHandChange;

        public decimal Amount { get; private set; }

        public Hand Hand
        {
            get => _Hand;
            set
            {
                var hand = _Hand;
                _Hand = value;
                OnHandChange?.Invoke(hand);
            }
        }
        public Hand _Hand;

        [SerializeField] private HandUIScript handUIScript;
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
        {
            Hand.Hit();
            OnTurn?.Invoke(BJTurn.Hit);
        }

        public void Stand()
        {
            Hand.Stand();
            OnTurn?.Invoke(BJTurn.Stand);
        }

        public void DoubleUp()
        {
            Hand.DoubleUp();
            OnTurn?.Invoke(BJTurn.DoubleUp);
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