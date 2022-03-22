using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Source.Model.Games;
using Assets.Source.Model.Games.BlackJack;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    [HideInInspector] public IBlackJack game;
    [SerializeField] HandScript playerHand;
    [SerializeField] HandScript dillerHand;
    [SerializeField] CardCollection cardCollection;

    [SerializeField] PlayerAction playerAction;
    [SerializeField] DeckScript trashDeck;

    [SerializeField] CardMono cardInstanse;

    private CardMono dillerHiddenCard;

    public CardsSequenceScript sequence;

    public float moveTime;
    public float flipTime;

    private List<CardMono> allCards = new List<CardMono>();

    public Task cardTranslate = Task.CompletedTask;

    void Awake()
    {
        game = new BlackJack();
        game.player.OnCardAdd += (card) => OnCardAdd(card, playerHand);
        game.diller.OnCardAdd += (card) => OnCardAdd(card, dillerHand);
        game.OnDillerUpHiddenCard += OnDillerUpHiddenCard;
        game.OnGameResult += OnGameResult;

        playerHand.user = game.player;
        dillerHand.user = game.diller;
    }

    void Start()
    {
        playerAction.uiScript.RegisterPlayer(playerHand, game);
        playerAction.uiScript.RegisterDiller(dillerHand);
        StartGame();
    }

    private async void OnCardAdd(Card card, ICardStorage storage)
    {
        var cardMono = CreateCard(card);

        await Wait();

        cardTranslate = MoveCard(new CardTranslateItem { card = cardMono, storage = storage });
    }

    private async void OnDillerUpHiddenCard(Card card)
    {
        dillerHiddenCard ??= CreateCard(card);
        dillerHiddenCard.card = cardCollection.GetCardObject(card);
        dillerHiddenCard.transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);

        await Wait();

        cardTranslate = FlipCard(dillerHiddenCard);
        dillerHand.UpdateScores();
    }

    private async void OnGameResult(GameResult result)
    {
        await Wait();

        playerAction.NotifyGameResult(result);
        playerAction.uiScript.ShowResults(game);
        await TaskEx.Delay(1.5f);

        playerHand.ClearCards();
        dillerHand.ClearCards();

        await MoveCard(allCards.Select(x => new CardTranslateItem { card = x, storage = trashDeck }).ToArray());

        allCards.Clear();

        await TaskEx.Delay(2.5f);

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

    async Task Wait()
    {
        do
        {
            await Task.Yield();
        } while (!cardTranslate.IsCompleted);
    }

    async void StartGame()
    {
        await Wait();

        game.Start();

        dillerHiddenCard = CreateCard(null);

        await Wait();

        cardTranslate = MoveCard(new CardTranslateItem { card = dillerHiddenCard, storage = dillerHand });
    }

    private Task MoveCard(CardTranslateItem card)
        => MoveCard(new CardTranslateItem[] { card });

    private async Task MoveCard(CardTranslateItem[] cards)
    {
        var moveTime = this.moveTime;

        var positions = new List<(CardTranslateItem card, Vector3 destPos, Vector3 startPos)>();

        foreach (var item in cards)
        {
            positions.Add((item, item.storage.GetCardPosition(), item.card.transform.position));
        }

        if (moveTime != 0)
        {
            while ((moveTime -= Time.deltaTime) > 0)
            {
                var relation = moveTime / this.moveTime;
                Move(relation);
                await Task.Yield();
            }
        }
        Move(0);

        foreach (var item in cards)
        {
            item.storage.AddCard(item.card);
        }

        void Move(float relation)
        {
            foreach (var item in positions)
            {
                item.card.card.transform.position = Vector3.Lerp(item.destPos, item.startPos, relation);
            }
        }
    }

    private async Task FlipCard(CardMono card)
    {
        var flipTime = this.flipTime;
        var startRotation = card.transform.rotation;
        var startPosition = card.transform.position;
        var flippedRotation = startRotation * Quaternion.AngleAxis(180, Vector3.forward);

        if (flipTime != 0)
        {
            while ((flipTime -= Time.deltaTime) > 0)
            {
                var relation = flipTime / this.flipTime;
                Rotate(relation);
                await Task.Yield();
            }
        }

        Rotate(0);

        void Rotate(float relation)
        {
            card.transform.rotation = Quaternion.Lerp(flippedRotation, startRotation, relation);
            card.transform.position = startPosition + new Vector3(0, CardObject.cardWidth * this.transform.localScale.z * Mathf.Sin(relation * Mathf.PI), 0);
        }
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
