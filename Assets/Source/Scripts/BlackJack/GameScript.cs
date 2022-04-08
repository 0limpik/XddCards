using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Cycles.BlackJack.Controllers;
using Xdd.Model.Games;
using Xdd.Scripts.Base;
using Xdd.Scripts.Cards;
using Xdd.Scripts.Hands;
using Xdd.Scripts.UI;

[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]
namespace Xdd.Scripts.BlackJack
{
    internal class GameScript : MonoBehaviour
    {
        public event Action OnDealtEnd;
        public event Action<float> OnGameResult;
        public event Action OnDealerUpCard;

        public float afterResultDelay;

        private GameController controller => cycle.gameController;
        private User user;
        private BJCycle cycle;

        [SerializeField]
        private CardCollection cardCollection;

        [SerializeField]
        private GameUIScript uiScript;

        [SerializeField]
        private BusyHandsScript bysyHands;

        [SerializeField]
        private HandStorageScript dealerHand;
        private CardMono dealerHiddenCard;

        [SerializeField]
        private DeckScript trashDeck;

        [SerializeField]
        public TaskQueue manager = new TaskQueue();

        private List<CardMono> allCards = new List<CardMono>();

        private bool game;

        public void InitCycle(BJCycle cycle, User user)
        {
            this.cycle = cycle;
            this.user = user;

            controller.dealerHand.player.OnCardAdd += (card) => OnCardAdd(dealerHand, card);
            controller.OnDillerUpHiddenCard += (card) => OnDillerUpHiddenCard(card);
            dealerHand.GetComponent<BJHandScript>().Hand = controller.dealerHand;
            controller.OnGameEnd += async () => await OnGameEnd();
            controller.OnChangeExecute += OnChangeExecute;
        }

        private void OnChangeExecute(bool execute)
        {
            if (execute)
            {
                StartGame();
            }
        }

        private void StartGame()
        {
            game = true;

            var handCount = bysyHands.Hands.Length;

            bysyHands.AllHands.ToList().ForEach(x => x.GetComponent<BJHandScript>().Hand = null);

            for (int i = 0; i < handCount; i++)
            {
                var hand = bysyHands.Hands[i];
                var player = user.Hands[i];

                var storage = hand.GetComponent<HandStorageScript>();
                var bjHand = hand.GetComponent<BJHandScript>();
                player.player.OnCardAdd += (card) =>
                {
                    OnCardAdd(storage, card);
                    bjHand.InvokeOnCardsChange();
                };
                bjHand.Hand = player;
                bjHand.Bet(player.Amount);
            }

            controller.Start();

            dealerHiddenCard = CreateCard(null);
            dealerHiddenCard.transform.rotation = Quaternion.AngleAxis(180f, Vector3.forward);
            dealerHand.AddCard(manager, dealerHiddenCard).OnEnd +=
                (x) => OnDealtEnd?.Invoke();
        }

        private void OnCardAdd(HandStorageScript hand, Card card)
        {
            var cardMono = CreateCard(card);
            hand.AddCard(manager, cardMono);
        }

        private CardMono CreateCard(Card card)
        {
            var transform = this.transform;
            var position = transform.position;
            var cardMono = cardCollection.CreateCardMono(card, position, transform.rotation, transform);
            allCards.Add(cardMono);
            return cardMono;
        }

        private void OnDillerUpHiddenCard(ICard card)
        {
            if (dealerHiddenCard == null)
                dealerHiddenCard = CreateCard(null);

            OnDealerUpCard?.Invoke();
            dealerHiddenCard.card = cardCollection.GetCardObject(card);
            dealerHiddenCard.Flip(manager);
            manager.AddTask((x) => TaskEx.Delay(0.5f), name: "delay");
        }

        private async Task OnGameEnd()
        {
            await manager.WaitQueleEnd();
            await Task.Yield();

            OnGameResult?.Invoke(afterResultDelay);
            await TaskEx.Delay(afterResultDelay);

            foreach (var hand in bysyHands.Hands)
            {
                hand.GetComponent<HandStorageScript>().ClearCards();
            }

            dealerHand.ClearCards();

            trashDeck.AddCard(manager, allCards.ToArray());
            allCards.Clear();

            game = false;
        }

        public async Task WaitEndGame()
        {
            do
            {
                await Task.Yield();
            } while (game);
        }
    }
}

