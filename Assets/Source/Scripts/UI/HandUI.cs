using System;
using System.Linq;
using UnityEngine.UIElements;
using Xdd.Model.Games;
using Xdd.Model.Games.BlackJack;
using Xdd.Scripts.BlackJack;
using Xdd.UI.Elements;

namespace Xdd.Scripts.UI
{
    internal class HandUI
    {
        public bool Result
        {
            set
            {
                if (value && SetResult())
                    result.style.visibility = Visibility.Visible;
                else
                    result.style.visibility = Visibility.Hidden;

            }
        }
        public bool Actions
        {
            set
            {
                if (handScript?.isDealer == true)
                    return;
                actions.style.visibility = value ? Visibility.Visible : Visibility.Hidden;
            }
        }
        public bool Score
        {
            set => score.style.visibility = value ? Visibility.Visible : Visibility.Hidden;
        }
        public bool Bet
        {
            set
            {
                if (handScript?.isDealer == true)
                    return;

                bet.style.visibility = value ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public decimal BetAmount
        {
            set => bet.Value = $"{value}";
        }

        public bool All { set => Result = Actions = Score = Bet = value; }

        private VisualElement result;
        private Label resultLabel;
        private VisualElement[] results;
        private VisualElement actions;
        private VisualElement score;
        private Label scoreLabel;
        private ValueLabelElement bet;
        private VisualElement handUI;

        private BJHandScript handScript;

        public HandUI(BJHandScript script, VisualElement handUI)
        {
            this.handScript = script;
            this.handUI = handUI;

            this.result = handUI.Q("result");
            this.resultLabel = result.Q<Label>("text");
            this.actions = handUI.Q("actions");
            this.score = handUI.Q("score");
            this.scoreLabel = score.Q<Label>("text");
            this.bet = handUI.Q<ValueLabelElement>("bet");
            results = result.Q("images").hierarchy.Children().ToArray();

            if (handScript.isDealer)
            {
                actions.parent.Remove(actions);
                bet.parent.Remove(bet);
            }
            else
            {
                actions.Q<Button>("hit").clicked += handScript.Hit;
                actions.Q<Button>("stand").clicked += handScript.Stand;
                actions.Q<Button>("double-up").clicked += handScript.DoubleUp;
            }

            handScript.OnHandChange += (hand) =>
            {
                if (hand != null)
                {
                    hand.player.OnCardAdd -= User_OnCardAdd;
                }

                if (handScript.Hand?.player != null)
                {
                    handScript.Hand.player.OnCardAdd += User_OnCardAdd;
                }
            };

            handScript.OnCardsChange += OnCardsChange;
            handScript.OnTurn += (turn) =>
            {
                if (turn == BJTurn.Stand || turn == BJTurn.DoubleUp)
                    Actions = handScript.Hand.player.CanTurn;
            };
            handScript.OnBet += () =>
            {
                Bet = true;
                BetAmount = handScript.Amount;
            };
            handScript.OnDoubleUp += () => BetAmount = handScript.Amount * 2;

            All = false;
        }

        private void User_OnCardAdd(Card obj)
        {
            Actions = false;
        }

        private void OnCardsChange()
        {
            Score = handScript.cards.Count > 0;

            if (handScript.Hand?.player == null)
                return;

            Actions = handScript.Hand.player.CanTurn;
            var scores = GameScores.GetBlackJackScores(handScript.cards);

            if (scores.Count() > 0)
            {
                var less = scores.Where(x => x <= 21);

                if (less.Count() > 0)
                {
                    scoreLabel.text = string.Join(" / ", less);
                }
                else
                {
                    scoreLabel.text = scores.Min().ToString();
                }
            }
            else
            {
                scoreLabel.text = "0";
            }
        }

        private bool SetResult()
        {
            if (handScript.Hand?.player == null)
                return false;

            var status = handScript.Hand.player.GetStatus().ToString();

            if (string.IsNullOrEmpty(status))
                return false;

            var image = results.First(x => x.name.Equals(status, StringComparison.OrdinalIgnoreCase));

            image.style.display = DisplayStyle.Flex;

            results
                .Where(x => x != image)
                .ToList()
                .ForEach(x => x.style.display = DisplayStyle.None);

            resultLabel.text = status;

            return true;
        }
    }
}
