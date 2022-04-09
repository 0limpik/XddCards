using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Cycles.BlackJack.Controllers;
using Xdd.Scripts.Base;
using Xdd.Scripts.UI.BlackJack;

namespace Xdd.Scripts.UI
{
    [RequireComponent(typeof(UIDocument))]
    internal class GameUIScript : MonoBehaviour, ICycleRequired
    {
        private UIDocument document;

        public float gameResultDelayPerUser;

        private Label centerMessage;
        private VisualElement betsContainer;

        private BetController controller => cycle.betController;
        private BJCycle cycle;

        void Awake()
        {
            document = this.GetComponent<UIDocument>();

            var tree = document.rootVisualElement;

            centerMessage = tree.Q<Label>("center-message") ?? throw new ArgumentException();

            betsContainer = tree.Q("bet") ?? throw new ArgumentException();
            betsContainer.style.display = DisplayStyle.None;
        }

        public void InitCycle(BJCycle cycle, User user)
        {
            this.cycle = cycle;

            controller.OnChangeExecute += OnChangeExecute;

            this.GetComponents<ICycleRequired>()
                .Where(x => x != this as ICycleRequired)
                .ToList()
                .ForEach(component => component.InitCycle(cycle, user));

            betsContainer.style.display = DisplayStyle.None;
        }

        private void OnChangeExecute(bool execute)
        {
            betsContainer.style.display = execute ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public async void DisplayMessage(string message, Color? color = null)
        {
            centerMessage.text = message ?? "null";
            centerMessage.style.color = color ?? Color.white;
            await TaskEx.Delay(3f);
            centerMessage.text = null;
        }
    }
}