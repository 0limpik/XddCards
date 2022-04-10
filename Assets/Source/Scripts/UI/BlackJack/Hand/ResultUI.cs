using System;
using System.Linq;
using UnityEngine.UIElements;
using Xdd.Model.Games.BlackJack.Users;

namespace Xdd.Scripts.UI.BlackJack.Hand
{
    internal class ResultUI
    {
        private VisualElement win;
        private VisualElement lose;
        private VisualElement push;
        private VisualElement bust;
        private VisualElement[] results;

        private Label text;

        private VisualElement container;

        public ResultUI(VisualElement container)
        {
            this.container = container;

            this.win = container.Q("win") ?? throw new NullReferenceException(nameof(win));
            this.lose = container.Q("lose") ?? throw new NullReferenceException(nameof(lose));
            this.push = container.Q("push") ?? throw new NullReferenceException(nameof(push));
            this.bust = container.Q("bust") ?? throw new NullReferenceException(nameof(bust));

            results = new[] { win, lose, push, bust };

            this.text = container.Q<Label>("text");
        }

        public void SetVisibility(bool visibility)
        {
            container.style.visibility = visibility ? Visibility.Visible : Visibility.Hidden;
        }

        public void SetStatsus(PlayerStatus status)
        {
            text.text = status.ToString();

            switch (status)
            {
                case PlayerStatus.Win:
                    SelectResult(win);
                    return;
                case PlayerStatus.Lose:
                    SelectResult(lose);
                    return;
                case PlayerStatus.Push:
                    SelectResult(push);
                    return;
                case PlayerStatus.Bust:
                    SelectResult(bust);
                    return;
            }

            throw new ArgumentException(status.ToString(), nameof(status));
        }

        private void SelectResult(VisualElement element)
        {
            element.style.display = DisplayStyle.Flex;

            foreach (var item in results.Where(x => x != element))
            {
                item.style.display = DisplayStyle.None;
            }
        }
    }
}
