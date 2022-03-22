using System;
using System.Linq;
using Assets.Source.Model.Enums;
using Assets.Source.Model.Games;
using Assets.Source.Model.Games.BlackJack;
using Assets.Source.Model.Games.BlackJack.Users;
using Assets.Tests.Extensions;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Tests.BlackJackTest
{
    public class GameResultsTest
    {
        #region StartTests

        [TestCase(
            new Ranks[] { Ranks.Ten, Ranks.Ten, Ranks.Ten, Ranks.Ace },
            TestName = "Player has BlackJack",
            ExpectedResult = GameResult.Win)]

        [TestCase(
            new Ranks[] { Ranks.Ten, Ranks.Ace, Ranks.Ten, Ranks.Ace },
            TestName = "Player and Diller BlackJack",
            ExpectedResult = GameResult.Lose)]
        public GameResult? Start(Ranks[] ranks)
            => TestGame(ranks);

        #endregion
        #region StartAndStand

        [TestCase(
            new Ranks[] { Ranks.Ten, Ranks.Seven, Ranks.Ten, Ranks.Ten },
            TestName = "Player has More",
            ExpectedResult = GameResult.Win)]

        [TestCase(new Ranks[] { Ranks.Ten, Ranks.Ten, Ranks.Ten, Ranks.Seven },
            TestName = "Diller has More",
            ExpectedResult = GameResult.Lose)]

        [TestCase(new Ranks[] { Ranks.Ten, Ranks.Ace, Ranks.Ten, Ranks.Ten },
            TestName = "Diller has BlackJack",
            ExpectedResult = GameResult.Lose)]

        [TestCase(new Ranks[] { Ranks.Ten, Ranks.Seven, Ranks.Ten, Ranks.Seven },
            TestName = "Diller and Player Equals",
            ExpectedResult = GameResult.Push)]

        public GameResult? Start_Stand(Ranks[] ranks)
            => TestGame(ranks, (game) =>
            {
                Debug.Log("Player Stand");
                game.Stand();
            });

        [TestCase(new Ranks[] { Ranks.Ten, Ranks.Ten, Ranks.Ace, Ranks.Nine, Ranks.Ten },
            TestName = "Player has Ace and Bust",
            ExpectedResult = GameResult.Lose)]
        public GameResult? Start_Hit_Hit(Ranks[] ranks)
             => TestGame(ranks, (game) =>
             {
                 Debug.Log("Player Hit");
                 game.Hit();
                 Debug.Log("Player Hit");
                 game.Hit();
             });

        #endregion

        [TearDown]
        public void After()
        {
            Debug.Log("---");
            Debug.Log($"Expect: {TestExtensions.GetCurrentTestCase().ExpectedResult}");
        }

        private GameResult? TestGame(Ranks[] ranks, Action<IBlackJack> afterStart = null)
        {
            GameResult? gameResult = null;
            var game = new BlackJack();
            game.player.OnCardAdd += (card) => Debug.Log($"Player recieve {card}");
            game.diller.OnCardAdd += (card) => Debug.Log($"Diller recieve {card}");
            game.OnDillerUpHiddenCard += (card) => Debug.Log($"Diller up {card}");
            game.OnGameResult += (result) =>
            {
                Debug.Log($"Game End");
                Debug.Log($"{GetScores(game.player)} {result} {GetScores(game.diller)}");
                gameResult = result;
            };
            game.deck.cards = ranks.Select(x => new Card { rank = x, suit = Suits.Clubs }).ToList();
            Debug.Log("Game Start");
            game.Start();
            afterStart?.Invoke(game);
            return gameResult;
        }

        private static string GetScores(IUser player)
            => GameScoreTests.ScoresToString(player.GetScores());
    }
}
