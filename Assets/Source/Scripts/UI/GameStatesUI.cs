using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Cycles.BlackJack.Controllers;
using Xdd.Scripts.BlackJack;
using Xdd.Scripts.UI.BlackJack;
using Xdd.Scripts.UI.Elements;

namespace Xdd.Scripts.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(UIDocument))]
    internal class GameStatesUI : MonoBehaviour, ICycleRequired
    {
        private UIDocument document;

        [SerializeField] private GameStates preset;
        [SerializeField] private GameScript gameScript;
        [SerializeField] private GameManager gameManager;

        private VisualElement gameStates;
        private List<GameStateElement> states = new();
        void Awake()
        {
            document = this.GetComponent<UIDocument>();
            var tree = document.rootVisualElement;

            gameStates = tree.Q("game-states");
            gameStates.Clear();

            foreach (var preset in preset.States)
            {
                var state = new GameStateElement().Init(preset, this.preset.TimePostfix);
                gameStates.Add(state);
                states.Add(state);
                state.SetDisable();
            }
            gameScript.OnDealtEnd += () => EnableState("players");
            gameScript.OnDealerUpCard += () => EnableState("dealer");
            gameScript.OnGameResult += (delay) => EnableState("end", delay);

            gameManager.OnHand += (delay) => EnableState("hand", delay);
            gameManager.OnBet += (delay) => EnableState("bet", delay);
            gameManager.OnWait += () => EnableState("wait");
        }

        public void InitCycle(IBJCycle cycle, User user)
        {
            cycle.OnStateChange -= OnStateChange;
            cycle.OnStateChange += OnStateChange;
        }

        private void OnStateChange(BJCycleStates state)
        {
            if (state == BJCycleStates.Game)
            {
                EnableState("dealt");
            }
        }

        private void EnableState(string stateName, float? time = null)
        {
            var state = states.First(x => x.Preset.StateName == stateName);

            foreach (var item in states)
            {
                if (state == item)
                    item.SetEnable(time);
                else
                    item.SetDisable();
            }
        }
    }
}
