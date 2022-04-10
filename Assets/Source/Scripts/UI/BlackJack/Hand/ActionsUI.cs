using System;
using UnityEngine.UIElements;

namespace Xdd.Scripts.UI.BlackJack.Hand
{
    internal class ActionsUI
    {
        public event Action OnHit
        {
            add => hit.clicked += value;
            remove => hit.clicked -= value;
        }
        public event Action OnStand
        {
            add => stand.clicked += value;
            remove => stand.clicked -= value;
        }
        public event Action OnDoubleUp
        {
            add => doubleUp.clicked += value;
            remove => doubleUp.clicked -= value;
        }

        private Button hit;
        private Button stand;
        private Button doubleUp;

        private VisualElement container;

        private bool isDealer;

        public ActionsUI(VisualElement container, bool isDealer)
        {
            this.container = container;

            this.hit = container.Q<Button>("hit") ?? throw new NullReferenceException(nameof(hit));
            this.stand = container.Q<Button>("stand") ?? throw new NullReferenceException(nameof(stand));
            this.doubleUp = container.Q<Button>("double-up") ?? throw new NullReferenceException(nameof(doubleUp));

            if (isDealer)
            {
                SetDisplay(false);
            }

            this.isDealer = isDealer;
        }

        public void SetVisibility(bool visibility)
        {
            if (!isDealer)
            {
                container.style.visibility = visibility ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public void SetDisplay(bool display)
        {
            if (!isDealer)
            {
                container.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
    }
}
