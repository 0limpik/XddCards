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

        private VisualElement container;

        private ValueLabelElement amountLabel;
        private ValueLabelElement countLabel;

        private ScrollView betsTake;

        private ObservableCollection<PossibleBet> bets = new();

        private decimal BetsSum => bets.Select(x => x.Amount).Sum();

        private IBetController controller => cycle.BetController;
        private IUser user;
        IBJCycle cycle;

        private List<ChipElement> chips = new();

        void Awake()
        {
            document = this.GetComponent<UIDocument>();

            container = document.rootVisualElement.Q("bet") ?? throw new ArgumentException();

            amountLabel = container.Q<ValueLabelElement>("bet-amount") ?? throw new ArgumentException();
            countLabel = container.Q<ValueLabelElement>("bet-count") ?? throw new ArgumentException();
            betsTake = container.Q<ScrollView>("bet-take") ?? throw new ArgumentException();

            var avalibles = container.Q("bet-available") ?? throw new ArgumentException();

            bets.CollectionChanged += (o, e) =>
            {
                UpdateChips();
            };

            // clear chips in builder
            avalibles.Clear();
            betsTake.Clear();

            foreach (var bet in betsPreset.possibleBets)
            {
                var chip = CreateChip(bet, AddBet);

                chips.Add(chip);

                avalibles.Add(chip);
            }

            container.style.display = DisplayStyle.None;
        }
        public void InitCycle(IBJCycle cycle, IUser user)
        {
            this.cycle = cycle;
            this.user = user;
            controller.OnChangeExecute += OnChangeExecute;
        }

        private void OnChangeExecute(bool execute)
        {
            if (execute)
            {
                countLabel.Value = user.Hands.Length.ToString();
                if (user.Hands.Length > 0)
                    container.style.display = DisplayStyle.Flex;
            }
            else
            {
                container.style.display = DisplayStyle.None;
                ClearBet();
            }
            UpdateChips();
        }

        private async void AddBet(ChipElement chip)
        {
            await user.Bet(BetsSum + chip.Bet.Amount);
            bets.Add(chip.Bet);
            var chipElement = CreateChip(chip.Bet, RemoveBet);
            betsTake.Add(chipElement);
            betsTake.scrollOffset = new Vector2(float.MaxValue, 0);

            OnBetChange?.Invoke(BetsSum);
        }

        private async void RemoveBet(ChipElement chip)
        {
            await user.Bet(BetsSum - chip.Bet.Amount);
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
            amountLabel.Value = user.Amount.ToString();
            foreach (var chip in chips)
            {
                chip.SetEnabled(user.CanBet(BetsSum + chip.Bet.Amount));
            }
        }
    }
}
