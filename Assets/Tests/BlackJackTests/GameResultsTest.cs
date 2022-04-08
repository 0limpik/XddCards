using System;
using System.Linq;
using Assets.Tests.Extensions;
using NUnit.Framework;
using UnityEngine;
using Xdd.Model.Enums;
using Xdd.Model.Games;
using Xdd.Model.Games.BlackJack;
using Xdd.Model.Games.BlackJack.Users;

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
                game.Stand(game.players[0]);
            });

        [TestCase(new Ranks[] { Ranks.Ten, Ranks.Ten, Ranks.Ace, Ranks.Nine, Ranks.Ten },
            TestName = "Player has Ace and Bust",
            ExpectedResult = GameResult.Lose)]
        public GameResult? Start_Hit_Hit(Ranks[] ranks)
             => TestGame(ranks, (game) =>
             {
                 Debug.Log("Player Hit");
                 game.Hit(game.players[0]);
                 Debug.Log("Player Hit");
                 game.Hit(game.players[0]);
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
            var game = new Game();
            game.Init(1);
            var gameEnd = false;
            game.players[0].OnCardAdd += (card) => Debug.Log($"Player recieve {card}");
            game.players[0].OnResult += (result) =>
            {
                gameResult = result;
                Debug.Log($"Player {result}");
            };
            game.dealer.OnCardAdd += (card) => Debug.Log($"Diller recieve {card}");
            game.OnDillerUpHiddenCard += (card) => Debug.Log($"Diller up {card}");
            game.OnGameEnd += () =>
            {
                gameEnd = true;
                Debug.Log($"Game End");
                Debug.Log($"{GetScores(game.players[0])} {gameResult} {GetScores(game.dealer)}");
            };
            game.deck.cards = ranks.Select(x => new Card { rank = x, suit = Suits.Clubs }).ToList();
            Debug.Log("Game Start");
            game.Start();
            afterStart?.Invoke(game);

            if (!gameEnd)
                throw new Exception("Game Is Not End");

            return gameResult;
        }

        private static string GetScores(IPlayer player)
            => GameScoreTests.ScoresToString(player.GetScores());
    }
}
