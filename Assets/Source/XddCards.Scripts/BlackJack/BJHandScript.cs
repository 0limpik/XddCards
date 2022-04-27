using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [field: SerializeField]
        public bool isDealer { get; private set; }

        public decimal Amount { get; private set; }

        public IHand Hand { get; private set; }

        [SerializeField]
        private HandUIScript handUIScript;
        private HandStorageScript storage;

        public List<ICard> cards => storage.cards.Select(x => x.card).Cast<ICard>().ToList();

        void Awake()
        {
            storage = this.GetComponent<HandStorageScript>();
            storage.OnCardsChange += () => OnCardsChange?.Invoke();
        }

        public void SetHand(IHand hand)
        {
            Hand = hand;
            handUIScript.RegisterHand(this);
        }

        public void Bet(decimal amount)
        {
            this.Amount = amount;
            OnBet?.Invoke();
        }

        public ValueTask<bool> Hit()
            => Hand.Hit();

        public ValueTask Stand()
            => Hand.Stand();

        public async ValueTask DoubleUp()
        {
            await Hand.DoubleUp();
            OnDoubleUp?.Invoke();
        }

        public void InvokeOnCardsChange()
        {
            OnCardsChange?.Invoke();
        }
    }
}