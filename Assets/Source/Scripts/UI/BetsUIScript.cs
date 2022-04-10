using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Cycles.BlackJack.Controllers;
using Xdd.Scripts.Bets;
using Xdd.Scripts.UI.BlackJack;
using Xdd.UI.Elements;

namespace Xdd.Scripts.UI
{
    [RequireComponent(typeof(UIDocument))]
    internal class BetsUIScript : MonoBehaviour, ICycleRequired
    {
        public event Action<decimal> OnBetChange;

        private UIDocument document;

        [SerializeField] private PossibleBets betsPreset;

        private ValueLabelElement amountLabel;
        private ValueLabelElement countLabel;

        private ScrollView betsTake;

        private ObservableCollection<PossibleBet> bets = new();

        private decimal BetsSum => bets.Select(x => x.Amount).Sum();

        private BetController controller => cycle.betController;
        private User user;
        BJCycle cycle;

        private List<ChipElement> chips = new();

        void Awake()
        {
            document = this.GetComponent<UIDocument>();

            var tree = document.rootVisualElement.Q("bet");

            amountLabel = tree.Q<ValueLabelElement>("bet-amount") ?? throw new ArgumentException();
            countLabel = tree.Q<ValueLabelElement>("bet-count") ?? throw new ArgumentException();
            betsTake = tree.Q<ScrollView>("bet-take") ?? throw new ArgumentException();

            var container = tree.Q("bet-available") ?? throw new ArgumentException();

            bets.CollectionChanged += (o, e) =>
            {
                UpdateChips();
            };

            // clear chips in builder
            container.Clear();
            betsTake.Clear();

            foreach (var bet in betsPreset.possibleBets)
            {
                var chip = CreateChip(bet, AddBet);

                chips.Add(chip);

                container.Add(chip);
            }
        }
        public void InitCycle(BJCycle cycle, User user)
        {
            this.cycle = cycle;
            this.user = user;
            controller.OnChangeExecute += OnChangeExecute;
        }

        private void OnChangeExecute(bool execute)
        {
            if (execute)
                countLabel.Value = controller.HandCount.ToString();
            else
                ClearBet();
            UpdateChips();
        }

        private void AddBet(ChipElement chip)
        {
            user.Bet(BetsSum + chip.Bet.Amount);

            bets.Add(chip.Bet);

            var chipElement = CreateChip(chip.Bet, RemoveBet);

            betsTake.Add(chipElement);

            betsTake.scrollOffset = new Vector2(float.MaxValue, 0);

            OnBetChange?.Invoke(BetsSum);
        }

        private void RemoveBet(ChipElement chip)
        {
            user.Bet(BetsSum - chip.Bet.Amount);
            bets.Remove(chip.Bet);
            betsTake.Remove(chip);

            OnBetChange?.Invoke(BetsSum);
        }

        private void ClearBet()
        {
            bets.Clear();
            betsTake.Clear();
            UpdateChips();
        }

        private ChipElement CreateChip(PossibleBet bet, Action<ChipElement> onInteract)
        {
            ChipElement chipElement = null;
            chipElement = new ChipElement(bet, () => onInteract(chipElement), betsPreset.betsPostfix);
            return chipElement;
        }

        private void UpdateChips()
        {
            amountLabel.Value = controller.Amount.ToString();
            foreach (var chip in chips)
            {
                chip.SetEnabled(user.CanBet(BetsSum + chip.Bet.Amount));
            }
        }
    }
}
