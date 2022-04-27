using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Cycles.BlackJack.Controllers;
using Xdd.Scripts.Base;
using Xdd.Scripts.UI.BlackJack;

namespace Xdd.Scripts.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class GameUIScript : MonoBehaviour, ICycleRequired
    {
        private UIDocument document;

        public float gameResultDelayPerUser;

        private Label centerMessage;

        private IBetController controller => cycle.BetController;
        private IBJCycle cycle;

        void Awake()
        {
            document = this.GetComponent<UIDocument>();

            var tree = document.rootVisualElement;

            centerMessage = tree.Q<Label>("center-message") ?? throw new ArgumentException();
        }

        public void InitCycle(IBJCycle cycle, IUser user)
        {
            this.cycle = cycle;

            this.GetComponents<ICycleRequired>()
                .Where(x => x != this as ICycleRequired)
                .ToList()
                .ForEach(component => component.InitCycle(cycle, user));
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