using System.Collections.Generic;
using System.Linq;
using Assets.Source.Model.Enums;
using Assets.Source.Model.Games;
using Assets.Source.Model.Games.BlackJack;
using Assets.Tests.Extensions;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Tests.BlackJackTest
{
    [TestFixture]
    public class GameScoreTests
    {
        [TestCase(
            new Ranks[] { Ranks.Two, Ranks.Ace, Ranks.Three },
            TestName = "2_Ace_3",
            ExpectedResult = new int[] { 6, 16 })]

        [TestCase(
            new Ranks[] { Ranks.Ace, Ranks.Ace, Ranks.Ace, Ranks.Ace },
            TestName = "4xAce",
            ExpectedResult = new int[] { 4, 3 + 11, 2 + 22, 1 + 33, 44 })]

        [TestCase(
            new Ranks[] { Ranks.Ten, Ranks.Jack, Ranks.Queen, Ranks.King },
            TestName = "10_3xImage",
            ExpectedResult = new int[] { 40 })]

        public int[] GetScores(Ranks[] ranks)
        {
            var scores = GameScores.GetBlackJackScores(ranks.Select(x => new Card { rank = x, suit = Suits.Clubs }));

            Debug.Log($"Recive: {ScoresToString(scores)}");

            return scores;
        }

        [TearDown]
        public void After()
        {
            Debug.Log("---");
            Debug.Log($"Expect: {ScoresToString(TestExtensions.GetCurrentTestCase().ExpectedResult as int[])}");
        }

        public static string ScoresToString(IEnumerable<int> scores)
            => string.Join(" / ", scores);
    }
}
