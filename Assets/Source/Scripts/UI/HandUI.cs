using System;
using System.Linq;
using Assets.Source.Model.Games;
using Assets.Source.Model.Games.BlackJack;
using UnityEngine.UIElements;

namespace Assets.Source.Scripts.UI
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
                if (hand?.isDealer == true)
                    return;
                actions.style.visibility = value ? Visibility.Visible : Visibility.Hidden;
            }
        }
        public bool Score
        {
            set => score.style.visibility = value ? Visibility.Visible : Visibility.Hidden;
        }

        private VisualElement result;
        private Label resultLabel;
        private VisualElement[] results;
        private VisualElement actions;
        private VisualElement score;
        private Label scoreLabel;
        private VisualElement handUI;

        private BJHandScript hand;

        public HandUI(BJHandScript hand, VisualElement handUI)
        {
            this.hand = hand;
            this.handUI = handUI;

            this.result = handUI.Q("result-container");
            this.resultLabel = result.Q<Label>("result-text");
            this.actions = handUI.Q("actions-container");
            this.score = handUI.Q("score-container");
            this.scoreLabel = score.Q<Label>("score-text");
            results = result.Q("result-images").hierarchy.Children().ToArray();

            if (hand.isDealer)
            {
                handUI.Q("game-container").Remove(handUI.Q("actions-container"));
            }
            else
            {
                actions.Q<Button>("hit").clicked += hand.Hit;
                actions.Q<Button>("stand").clicked += hand.Stand;
            }

            hand.OnUserChange += (user) =>
            {
                Score = false;
                if (user != null)
                {
                    user.OnCardAdd -= User_OnCardAdd;
                }

                if (hand.User != null)
                {
                    hand.User.OnCardAdd += User_OnCardAdd;
                    Actions = hand.User.CanTurn;
                }
            };

            hand.OnCardsChange += OnCardChange;
            hand.OnTurn += (turn) =>
            {
                if (turn == BlackJackTurn.Stand)
                    Actions = hand.User.CanTurn;
            };

            Result = false;
            Actions = false;
            Score = false;
        }

        private void User_OnCardAdd(Card obj)
        {
            Actions = false;
        }

        private void OnCardChange()
        {
            if (hand.User == null)
                return;

            Actions = hand.User.CanTurn;
            Score = true;
            var scores = GameScores.GetBlackJackScores(hand.cards);

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
            if (hand.User == null)
                return false;

            var status = hand.User.GetStatus().ToString();

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
