using System.Linq;
using UnityEngine.UIElements;
using Xdd.Model.Games;
using Xdd.Model.Games.BlackJack;
using Xdd.Model.Games.BlackJack.Users;
using Xdd.Scripts.BlackJack;
using Xdd.UI.Elements;

namespace Xdd.Scripts.UI.BlackJack.Hand
{
    internal class HandUI
    {
        private bool Result { set => result.SetVisibility(value); }

        public bool Actions
        {
            set
            {
                if (!handScript.Hand.IsPlaying)
                    return;

                actions.SetVisibility(value);
            }
        }

        public bool Score { set => score.style.visibility = value ? Visibility.Visible : Visibility.Hidden; }

        public bool Bet
        {
            set
            {
                if (handScript.isDealer)
                    return;

                bet.style.visibility = GetVisibility(value);
            }
        }

        private bool All { set => Result = Actions = Score = Bet = value; }

        public decimal BetAmount { set => bet.Value = $"{value}"; }

        private ResultUI result;
        private ActionsUI actions;
        private Label score;
        private ValueLabelElement bet;
        public VisualElement tree;
        public VisualElement container;

        public BJHandScript handScript;

        public HandUI(BJHandScript script, VisualElement handUI)
        {
            this.handScript = script;
            this.tree = handUI;

            this.container = handUI.Q("container");
            this.result = new(handUI.Q("result"));
            this.actions = new(handUI.Q("actions"), handScript.isDealer);
            this.score = handUI.Q<Label>("score");

            this.bet = handUI.Q<ValueLabelElement>("bet");

            if (handScript.isDealer)
            {
                bet.style.display = DisplayStyle.None;
            }
            else
            {
                actions.OnHit += async () => await handScript.Hit();
                actions.OnStand += async () =>
                {
                    await handScript.Stand();
                    Actions = handScript.Hand.CanTurn;
                };
                actions.OnDoubleUp += async () =>
                {
                    await handScript.DoubleUp();
                    Actions = handScript.Hand.CanTurn;
                    BetAmount = handScript.Amount * 2;
                };
            }

            handScript.Hand.OnCardAdd += User_OnCardAdd;
            handScript.OnCardsChange += OnCardsChange;
            handScript.OnBet += () =>
            {
                Bet = true;
                BetAmount = handScript.Amount;
            };

            All = false;
        }

        public void SetResult(bool visibility)
        {
            if (!handScript.Hand.IsPlaying)
                return;

            var status = handScript.Hand.Status;

            if (status != null)
            {
                result.SetStatsus((PlayerStatus)status);
                Result = visibility;
            }

            if (!visibility)
            {
                Bet = false;
            }
        }

        private void User_OnCardAdd(ICard obj)
        {
            Actions = false;
        }

        private void OnCardsChange()
        {
            if (handScript.Hand == null)
                return;

            Score = handScript.cards.Count > 0;

            Actions = handScript.Hand.CanTurn;

            var scores = GameScores.GetBlackJackScores(handScript.cards);

            if (scores.Count() > 0)
            {
                var less = scores.Where(x => x <= 21);

                if (less.Count() > 0)
                {
                    score.text = string.Join(" / ", less);
                }
                else
                {
                    score.text = scores.Min().ToString();
                }
            }
            else
            {
                score.text = "0";
            }
        }

        private Visibility GetVisibility(bool value)
            => value ? Visibility.Visible : Visibility.Hidden;
    }
}
