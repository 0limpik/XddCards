using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Source.Model.Games;
using Assets.Source.Model.Games.BlackJack;
using Assets.Source.Model.Games.BlackJack.Users;
using UnityEngine;

[ExecuteAlways]
public class BJHandScript : MonoBehaviour, ICardStorage
{
    [field: SerializeField] public bool isDealer { get; private set; }

    public event Action OnCardsChange;
    public event Action<BlackJackTurn> OnTurn;
    public event Action<IUser> OnUserChange;

    public IUser User
    {
        get => _User;
        private set
        {
            var prev = _User;
            _User = value;
            OnUserChange?.Invoke(prev);
        }
    }
    private IUser _User;
    private IBlackJack game;

    public CardMono cardInstanse;
    public GameUIScript uiScript;

    public IEnumerable<ICard> cards => cardsMono.Select(x => x.card);
    [SerializeField, HideInInspector] private List<CardMono> cardsMono = new List<CardMono>();

    private Vector3 cardOffset = new Vector3(1.3f, 0.01f, 1.75f);

    void Awake()
    {
        uiScript.RegisterHand(this);
    }

    public void AddCard(CardMono cardMono)
    {
        cardsMono.Add(cardMono);

        cardMono.OnCardChange += () => OnCardsChange?.Invoke();

        cardMono.transform.parent = this.transform;
        OnCardsChange?.Invoke();
    }

    public Vector3 GetCardPosition()
    {
        return this.transform.position + (cardOffset * cardsMono.Count);
    }

    public void ClearCards()
    {
        cardsMono.Clear();
        OnCardsChange?.Invoke();
    }

    public void BindPlayer(IUser user, IBlackJack game)
    {
        this.User = user;
        this.game = game;
    }
    public void UnBindPlayer()
    {
        this.User = null;
        this.game = null;
    }

    public void Hit()
    {
        var turn = BlackJackTurn.Hit;
        game.Turn(User, turn);
        OnTurn?.Invoke(turn);
    }

    public void Stand()
    {
        var turn = BlackJackTurn.Stand;
        game.Turn(User, turn);
        OnTurn?.Invoke(turn);
    }
}
