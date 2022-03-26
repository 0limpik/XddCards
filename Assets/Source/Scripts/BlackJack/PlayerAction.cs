using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Source.Model.Games;
using Assets.Source.Model.Games.BlackJack;
using UnityEngine;

[RequireComponent(typeof(GameUIScript))]
public class PlayerAction : MonoBehaviour
{
    [SerializeField] private BJHandScript hand;
    [SerializeField] private CardCollection cardCollection;
    public GameUIScript uiScript;

    void Awake()
    {
        uiScript = this.GetComponent<GameUIScript>();
    }

    void Start()
    {

    }

    private int wins;
    private int pushs;
    private int loses;

    public void NotifyGameResult(GameResult[] result)
    {
        wins += result.Where(x => x == GameResult.Win).Count();
        pushs += result.Where(x => x == GameResult.Push).Count();
        loses += result.Where(x => x == GameResult.Lose).Count();

        uiScript.DisplayMessage($"W: {wins} P: {pushs} L: {loses}", Color.white);
    }
}
