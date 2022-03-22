using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Source.Model.Enums;
using Assets.Source.Model.Games.BlackJack.Users;
using UnityEngine;

namespace Assets.Source.Model.Games.BlackJack
{
    public class BlackJack : IBlackJack
    {
        public event Action<GameResult> OnGameResult;
        public event Action<Card> OnDillerUpHiddenCard;

        public IUser player => _player;
        public IUser diller => _diller;

        private Player _player;
        private Diller _diller;
        public Card dillerHiddenCard;

        public Deck deck = new Deck();

        public Func<bool> take;

        private bool gameEnd;

        public BlackJack()
        {
            _diller = new Diller(GetScores);
            _player = new Player(GetScores, _diller);

            OnGameResult += (result) => Debug.Log($"P: {string.Join(" / ", _player.GetScores())} {result} D: {string.Join(" / ", _diller.GetScores())}");
        }

        public void Start()
        {
            gameEnd = false;
            _player.Reset();
            _diller.Reset();

            _diller.AddCard(GetCard());
            dillerHiddenCard = GetCard();

            _player.AddCard(GetCard());
            _player.AddCard(GetCard());

            if (_player.IsBlackJack())
            {
                EndGame();
            }
        }

        public void Hit()
        {
            if (gameEnd) throw new Exception("Game is End");
            _player.AddCard(GetCard());

            if (_player.IsBust())
            {
                UpDillerHiddenCard();
                OnGameResult?.Invoke(GameResult.Lose);
                return;
            }

            if (_player.IsBlackJack())
            {
                EndGame();
                return;
            }
        }

        public void Stand()
        {
            if (gameEnd) throw new Exception("Game is End");
            EndGame();
        }

        private void EndGame()
        {
            gameEnd = true;

            UpDillerHiddenCard();

            while (_diller.GetScores().All(x => x < 17))
            {
                _diller.AddCard(GetCard());
            }

            if (_diller.IsBust())
            {
                OnGameResult?.Invoke(GameResult.Win);
                return;
            }

            if (_diller.IsBlackJack())
            {
                OnGameResult?.Invoke(GameResult.Lose);
                return;
            }

            if (_player.IsMore())
            {
                OnGameResult?.Invoke(GameResult.Win);
                return;
            }

            if (_player.IsEquals())
            {
                OnGameResult?.Invoke(GameResult.Push);
                return;
            }

            OnGameResult?.Invoke(GameResult.Lose);
        }

        private void UpDillerHiddenCard()
        {
            _diller.AddCardHidden(dillerHiddenCard);
            OnDillerUpHiddenCard?.Invoke(dillerHiddenCard);
        }

        private Card GetCard()
        {
            if (!deck.TryPeek(out Card card))
            {
                deck.Reload();
            }
            return card;
        }

        public static int[] GetScores(IEnumerable<ICard> cards)
        {
            var values = cards
                .Where(x => x != null)
                .Select(x => GetCardValue(x))
                .ToList();

            var scores = GetScore(values);

            return scores;
        }

        private static int[] GetScore(List<int[]> values)
        {
            if (values.Count < 1)
                return new int[0];

            var value = values[0];

            if (values.Count == 1)
                return value.Distinct().ToArray();

            values.Remove(value);

            var next = GetScore(values);

            int k = 0;

            var scores = new int[value.Length * next.Length];

            for (int i = 0; i < value.Length; i++)
            {
                for (int j = 0; j < next.Length; j++)
                {
                    scores[k] = value[i] + next[j];
                    k++;
                }
            }

            return scores.Distinct().ToArray();
        }

        private static int[] GetCardValue(ICard card)
        {
            if (card == null)
                return new int[0];

            switch (card.rank)
            {
                case Ranks.Two:
                case Ranks.Three:
                case Ranks.Four:
                case Ranks.Five:
                case Ranks.Six:
                case Ranks.Seven:
                case Ranks.Eight:
                case Ranks.Nine:
                    return new int[] { (int)card.rank };
                case Ranks.Ten:
                case Ranks.Jack:
                case Ranks.Queen:
                case Ranks.King:
                    return new int[] { 10 };
                case Ranks.Ace:
                    return new int[] { 1, 11 };
            }

            throw new ArgumentException();
        }
    }

    public enum GameResult
    {
        Win = 1,
        Lose,
        Push,
    }

    public static class GameResultExtensions
    {
        public static string ToDebug(this GameResult result)
        {
            var color = "black"; ;
            switch (result)
            {
                case GameResult.Win:
                    color = "green";
                    break;
                case GameResult.Lose:
                    break;
                case GameResult.Push:
                    color = "yeloow";
                    break;
            }

            return $"<color={color}>{result}</color>";
        }

    }
}
