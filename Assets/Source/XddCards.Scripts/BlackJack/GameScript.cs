using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Cycles.BlackJack.Controllers;
using Xdd.Model.Games;
using Xdd.Scripts.Base;
using Xdd.Scripts.Cards;
using Xdd.Scripts.Hands;
using Xdd.Scripts.UI;

namespace Xdd.Scripts.BlackJack
{
    public class GameScript : MonoBehaviour
    {
        public event Action OnDealtEnd;
        public event Action<float> OnGameResult;
        public event Action OnDealerUpCard;

        public float afterResultDelay;

        private IGameController controller => cycle.GameController;
        private IUser user;
        private IBJCycle cycle;

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

        public async void InitCycle(IBJCycle cycle, IUser user)
        {
            this.cycle = cycle;
            this.user = user;

            cycle.BetController.OnChangeExecute += BetController_OnChangeExecute;

            controller.DealerHand.OnCardAdd += (card) => OnCardAdd(dealerHand, card);
            controller.OnDillerUpHiddenCard += (card) => OnDillerUpHiddenCard(card);
            dealerHand.GetComponent<BJHandScript>().SetHand(controller.DealerHand);
            controller.OnGameEnd += async () => await OnGameEnd();
            controller.OnChangeExecute += OnChangeExecute;

            await TaskEx.Delay(1f);

            var hands = bysyHands.AllHands.ToArray();

            for (int i = 0; i < cycle.HandController.Hands.Length; i++)
            {
                var handScript = hands[i];
                var hand = cycle.HandController.Hands[i];

                var storage = handScript.GetComponent<HandStorageScript>();
                var bjHand = handScript.GetComponent<BJHandScript>();

                hand.OnCardAdd += (card) =>
                {
                    OnCardAdd(storage, card);
                    bjHand.InvokeOnCardsChange();
                };
                bjHand.SetHand(hand);
            }

        }

        private void BetController_OnChangeExecute(bool execute)
        {
            if (!execute)
            {
                var hands = bysyHands.AllHands.ToArray();

                foreach (var hand in user.Hands)
                {
                    foreach (var item in hands.Select(x => x.GetComponent<BJHandScript>()))
                    {
                        if (item.Hand == hand)
                        {
                            item.Bet(user.Amount);
                            break;
                        }
                    }
                }
            }
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

            //controller.Start();

            dealerHiddenCard = CreateCard(null);
            dealerHiddenCard.transform.rotation = Quaternion.AngleAxis(180f, Vector3.forward);
            dealerHand.AddCard(manager, dealerHiddenCard).OnEnd +=
                (x) => OnDealtEnd?.Invoke();
        }

        private void OnCardAdd(HandStorageScript hand, ICard card)
        {
            var cardMono = CreateCard(card);
            hand.AddCard(manager, cardMono);
        }

        private CardMono CreateCard(ICard card)
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

            foreach (var hand in bysyHands.AllHands)
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