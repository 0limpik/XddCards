using System;
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
        private decimal TotalBetSum => BetSum * (user.Hands.Length + BetsDoubleCount);

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

        private IBetController controller => cycle.BetController;
        private IUser user;
        private IBJCycle cycle;

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

        public void InitCycle(IBJCycle cycle, IUser user)
        {
            this.cycle = cycle;
            this.user = user;

            cycle.GameController.OnChangeExecute += Game_OnChangeExecute;
            gameScript.OnGameResult += (_) => Cash = this.user.Cash;
            Cash = user.Cash;
            TotalBet = 0;
        }

        private void Game_OnChangeExecute(bool execute)
        {
            Cash = user.Cash;
            if (execute)
            {
                _StartCash = user.Cash;
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
