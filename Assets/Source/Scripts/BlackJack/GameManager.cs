using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Source.Model.Games;
using Assets.Source.Model.Games.BlackJack;
using Assets.Source.Model.Games.BlackJack.Users;
using Assets.Source.Scripts.Cash;
using UnityEngine;

namespace Assets.Source.Scripts.BlackJack
{
    [RequireComponent(typeof(BusyHandsScript))]
    internal class GameManager : MonoBehaviour
    {
        private IBlackJack game;

        [SerializeField] GameUIScript uiScript;

        [SerializeField] CardCollection cardCollection;

        [SerializeField] BusyHandsScript bysyHands;

        [SerializeField] private BJHandScript dealerHand;
        private CardMono dealerHiddenCard;

        [SerializeField] DeckScript trashDeck;

        [SerializeField] private CardsManager cardsManager = new CardsManager();

        private List<CardMono> allCards = new List<CardMono>();

        private bool needReInit;

        PlayerWallet wallet;


        void Awake()
        {
            wallet = new PlayerWallet(10000m);
            prev = wallet.Cash;

            bysyHands = this.GetComponent<BusyHandsScript>();
            bysyHands.OnInteraction += async () =>
            {
                needReInit = true;
                await TaskEx.Delay(1.5f);

                if (!game.isGame && bysyHands.Hands.Count > 0)
                    _ = StartGame();
            };

            game = new Game();
            game.dealer.OnCardAdd += async (card) => await OnCardAdd(dealerHand, card);
            game.OnDillerUpHiddenCard += async (card) => await OnDillerUpHiddenCard(card);
            game.OnGameEnd += async () => await OnGameEnd();
        }

        private async Task OnDillerUpHiddenCard(Card card)
        {
            await cardsManager.cardTranslate;
            dealerHiddenCard.card = cardCollection.GetCardObject(card);
            await cardsManager.FlipCard(dealerHiddenCard);
        }

        void Start()
        {
            dealerHand.BindPlayer(game.dealer, game);
            uiScript.RegisterWallet(wallet);
        }

        private async Task StartGame()
        {
            bysyHands.Hands.ForEach(x => x.Lock = true);

            var handCount = bysyHands.Hands.Count;

            if (needReInit)
            {
                game.Init(handCount);

                bysyHands.AllHands.ForEach(x => x.GetComponent<BJHandScript>().UnBindPlayer());

                for (int i = 0; i < handCount; i++)
                {
                    var hand = bysyHands.Hands[i];
                    var player = game.players[i];

                    var bjHand = hand.GetComponent<BJHandScript>();

                    bjHand.BindPlayer(player, game);
                    bjHand.User.OnCardAdd += async (card) => await OnCardAdd(bjHand, card);

                    var index = i;

                    player.OnResult += async (x) => await OnResult(x, player);
                }

                needReInit = false;
            }

            foreach (var player in game.players)
            {
                bets.Add((player, wallet.Reserve(250)));
            }

            game.Start();

            await cardsManager.Wait();
            dealerHiddenCard = CreateCard(null);
            dealerHiddenCard.transform.rotation = Quaternion.AngleAxis(180f, Vector3.forward);

            await cardsManager.MoveCard(dealerHiddenCard, (card) => dealerHand.GetCardPosition());

            dealerHand.AddCard(dealerHiddenCard);
        }

        List<(IUser player, Bet bet)> bets = new();
        private decimal prev;

        private async Task OnResult(GameResult result, IUser player)
        {
            await cardsManager.Wait();

            Debug.Log($"{result}");

            var bet = bets.First(x => x.player == player);

            if (result == GameResult.Win)
            {
                wallet.Give(bet.bet);
            }
            if (result == GameResult.Lose)
            {
                wallet.Take(bet.bet);
            }
            if (result == GameResult.Push)
            {
                wallet.Cancel(bet.bet);
            }
            bets.Remove(bet);
        }

        private async Task OnCardAdd(BJHandScript hand, Card card)
        {
            var cardMono = CreateCard(card);
            await cardsManager.MoveCard(cardMono, (card) => hand.GetCardPosition());
            hand.AddCard(cardMono);
        }

        private CardMono CreateCard(Card card)
        {
            var position = this.transform.position + Vector3.up * 0.01f * allCards.Count;
            var cardMono = cardCollection.CreateCardMono(card, position, this.transform.rotation, this.transform);
            allCards.Add(cardMono);
            return cardMono;
        }

        private async Task OnGameEnd()
        {
            await cardsManager.Wait();

            Debug.Log($"{prev} - {wallet.Cash}");
            prev = wallet.Cash;

            await uiScript.ShowResults();

            foreach (var hand in bysyHands.Hands)
            {
                var bjHand = hand.GetComponent<BJHandScript>();
                bjHand.ClearCards();
            }

            dealerHand.ClearCards();

            await cardsManager.MoveCards(allCards.ToArray(), (card) =>
            {
                trashDeck.AddCard(card);
                allCards.Remove(card);
                return trashDeck.GetCardPosition();
            });

            bysyHands.Hands.ForEach(x => x.Lock = false);

            await TaskEx.Delay(4f);

            if (bysyHands.Hands.Count > 0)
                await StartGame();
        }
    }
}
