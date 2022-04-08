using System.Collections.Generic;
using System.Linq;
using Assets.Source.Model.Cycles.BlackJack;
using Assets.Source.Model.Cycles.BlackJack.Controllers;
using Assets.Source.Scripts.BlackJack;
using Assets.Source.Scripts.UI.BlackJack;
using Assets.Source.Scripts.UI.Elements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Source.Scripts.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(UIDocument))]
    public class GameStatesUI : MonoBehaviour, ICycleRequired
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

        public void InitCycle(BJCycle cycle, User user)
        {
            cycle.OnStateChange -= OnStateChange;
            cycle.OnStateChange += OnStateChange;
        }

        private void OnStateChange(State state)
        {
            if (state is GameController)
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
