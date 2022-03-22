using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Source.Model.Games.BlackJack;
using Assets.Source.Model.Games.BlackJack.Users;
using UnityEngine;

public class HandScript : MonoBehaviour, ICardStorage
{
    public PlayerUIObject playerUIObject;
    public IUser user;

    public IReadOnlyList<CardObject> cards => cardsMono.Select(x => x.card).ToList();

    public CardMono cardInstanse;
    public GameUIScript uiScript;

    [SerializeField, HideInInspector] private List<CardMono> cardsMono = new List<CardMono>();

    private Vector3 cardOffset = new Vector3(1f, 0.01f, 1f);
    public float moveTime;

    void Awake()
    {
        playerUIObject = ScriptableObject.CreateInstance<PlayerUIObject>();

        //uiScript.AddScore(this);
    }

    public void AddCard(CardMono card)
    {
        cardsMono.Add(card);
        card.transform.parent = this.transform;
        UpdateScores();
    }

    public Vector3 GetCardPosition()
    {
        return this.transform.position + (cardOffset * (cardsMono.Count + 1));
    }

    public void RemoveCard(CardMono card)
    {
        if (!cardsMono.Remove(card))
            throw new Exception();

        UpdateScores();
    }

    public void ClearCards()
    {
        cardsMono.Clear();
        UpdateScores();
    }

    public void UpdateScores()
    {
        playerUIObject.Scores = GetScore();
        //uiScript.SetScore(this, string.Join(" / ", GetScore()));
    }

    public int[] GetScore()
    {
        return BlackJack.GetScores(cards);
    }
}
