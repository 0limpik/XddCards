using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Source.Model.Games;
using Assets.Source.Model.Games.BlackJack;
using Assets.Source.Scripts.BlackJack;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public IBlackJack game;
    [SerializeField] BJHandScript[] playerHands;
    [SerializeField] BJHandScript dillerHand;
    [SerializeField] CardCollection cardCollection;

    [SerializeField] PlayerAction playerAction;
    [SerializeField] DeckScript trashDeck;

    [SerializeField] CardMono cardInstanse;

    private CardMono dillerHiddenCard;

    public CardsSequenceScript sequence;

    private List<CardMono> allCards = new List<CardMono>();

    private List<GameResult> gameResults = new List<GameResult>();

    [SerializeField] private CardsManager cardsManager = new CardsManager();

    private Task cardTranslate = Task.CompletedTask;

    void Awake()
    {
        var playerCount = playerHands.Length;

        game = new Game();

        game.Init(6);

        for (int i = 0; i < playerCount; i++)
        {
            var player = game.players[i];
            var playerHand = playerHands[i];
            player.OnCardAdd += (card) => OnCardAdd(card, playerHand);
            player.OnResult += (result) => gameResults.Add(result);
            ///player.OnEnd += () => playerHand.SetAction(false);
            //playerHand.user = player;
        }

        game.dealer.OnCardAdd += (card) => OnCardAdd(card, dillerHand);
        game.OnDillerUpHiddenCard += OnDillerUpHiddenCard;
        //dillerHand.user = game.dealer;

        game.OnGameEnd += OnGameEnd;
    }

    void Start()
    {
        foreach (var playerHand in playerHands)
        {
            //playerAction.uiScript.RegisterPlayer(playerHand, game);
        }

        //playerAction.uiScript.RegisterDiller(dillerHand);

        StartGame();
    }

    private async void OnCardAdd(Card card, ICardStorage storage)
    {
        var cardMono = CreateCard(card);

        await Wait();

        //cardTranslate = cardsManager.MoveCard(new CardTranslateItem { card = cardMono, storage = storage });
    }

    private async void OnDillerUpHiddenCard(Card card)
    {
        dillerHiddenCard ??= CreateCard(card);
        dillerHiddenCard.card = cardCollection.GetCardObject(card);
        dillerHiddenCard.transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);

        await Wait();

       // cardTranslate = cardsManager.FlipCard(dillerHiddenCard);
        //dillerHand.UpdateScores();
    }

    private async void OnGameEnd()
    {
        await Wait();

        //playerAction.NotifyGameResult(result);
        //playerAction.uiScript.ShowResults(game);
        await TaskEx.Delay(2.5f);

        playerAction.NotifyGameResult(gameResults.ToArray());
        gameResults.Clear();

        foreach (var playerHand in playerHands)
        {
            playerHand.ClearCards();
        }

        dillerHand.ClearCards();

        //await cardsManager.MoveCard(allCards.Select(x => new CardTranslateItem { card = x, storage = trashDeck }).ToArray());

        allCards.Clear();

        await TaskEx.Delay(1.5f);

        StartGame();
    }

    private CardMono CreateCard(Card card)
    {
        var cardObject = cardCollection.GetCardObject(card);

        var position = this.transform.position + Vector3.up * 0.01f * allCards.Count();

        var cardMono = GameObject.Instantiate(this.cardInstanse, position, this.transform.rotation, this.transform);
        cardMono.card = cardObject;

        allCards.Add(cardMono);

        return cardMono;
    }

    async void StartGame()
    {
        await Wait();

        game.Start();

        dillerHiddenCard = CreateCard(null);

        await Wait();

        //cardTranslate = cardsManager.MoveCard(new CardTranslateItem { card = dillerHiddenCard, storage = dillerHand });
    }
    public async Task Wait()
    {
        do
        {
            await Task.Yield();
        } while (!cardTranslate.IsCompleted);
    }
}

public static class TaskEx
{
    public static async Task Delay(float delay)
    {
        var time = Time.timeAsDouble + delay;

        while (time > Time.timeAsDouble)
        {
            await Task.Yield();
        }
    }

    public static async Task Next(this Task task)
    {
        do
        {
            Debug.Log("Wait");
            await Task.Yield();
        } while (!task.IsCompleted);
        Debug.Log("Release");
    }
}
