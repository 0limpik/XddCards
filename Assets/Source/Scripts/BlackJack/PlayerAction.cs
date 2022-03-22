using System.Collections;
using System.Collections.Generic;
using Assets.Source.Model.Games;
using Assets.Source.Model.Games.BlackJack;
using UnityEngine;

[RequireComponent(typeof(GameUIScript))]
public class PlayerAction : MonoBehaviour
{
    [SerializeField] private GameScript gameScript;
    [SerializeField] private HandScript hand;
    [SerializeField] private CardCollection cardCollection;
    public GameUIScript uiScript;

    void Awake()
    {
        uiScript = this.GetComponent<GameUIScript>();
    }

    void Start()
    {

    }

    public void NotifyGameResult(GameResult result)
    {
        Color? color = null;
        if (result == GameResult.Win)
            color = Color.green;
        if (result == GameResult.Lose)
            color = Color.red;
        if (result == GameResult.Push)
            color = Color.yellow;

        uiScript.DisplayMessage(result.ToString(), color);
    }

    public void Hit()
    {
        //Debug.Log("Player Hit");
        gameScript.game.Hit();
    }

    public void Stand()
    {
        //Debug.Log("Player Stand");
        gameScript.game.Stand();
    }
}
