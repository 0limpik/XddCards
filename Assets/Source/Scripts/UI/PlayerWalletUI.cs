using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Cycles.BlackJack.Controllers;
using Xdd.Scripts.BlackJack;
using Xdd.Scripts.UI.BlackJack;
using Xdd.UI.Elements;

namespace Xdd.Scripts.UI
{
    [RequireComponent(typeof(UIDocument))]
    internal class PlayerWalletUI : MonoBehaviour, ICycleRequired
    {
        private UIDocument document;

        private ValueLabelElement cashLabel;
        private ValueLabelElement betLabel;

        public decimal BetSum { get; private set; }
        private int BetsDoubleCount;
        private decimal TotalBetSum => BetSum * (controller.HandCount + BetsDoubleCount);

        [SerializeField] private BetsUIScript betsUI;
        [SerializeField] private GameScript gameScript;

        private decimal Cash
        {
            set => cashLabel.Value = value.ToString();
        }

        private decimal TotalBet
        {
            set => betLabel.Value = value.ToString();
        }
        private decimal _StartCash;

        private BJCycle cycle;
        private BetController controller => cycle.betController;
        private User Player => cycle.Users.First();

        void Awake()
        {
            document = this.GetComponent<UIDocument>();

            var tree = document.rootVisualElement;

            cashLabel = tree.Q<ValueLabelElement>("cash-container") ?? throw new ArgumentException();
            betLabel = tree.Q<ValueLabelElement>("bet-container") ?? throw new ArgumentException();

            betsUI.OnBetChange += OnBetChange;
        }

        private void OnBetChange(decimal bet)
        {
            BetSum = bet;
            TotalBet = TotalBetSum;
        }

        public void InitCycle(BJCycle cycle, User user)
        {
            this.cycle = cycle;
            cycle.gameController.OnChangeExecute += Game_OnChangeExecute;
            gameScript.OnGameResult += (_) => Cash = Player.Cash;
            Cash = Player.Cash;
            TotalBet = 0;
        }

        private void Game_OnChangeExecute(bool execute)
        {
            Cash = Player.Cash;
            if (execute)
            {
                _StartCash = Player.Cash;
            }
            else
            {
                BetsDoubleCount = 0;
                TotalBet = 0;
                _StartCash = 0;
            }
        }

        public void DoubleBet()
        {
            BetsDoubleCount++;
            TotalBet = TotalBetSum;
            Cash = _StartCash - BetSum * BetsDoubleCount;
        }
    }
}
