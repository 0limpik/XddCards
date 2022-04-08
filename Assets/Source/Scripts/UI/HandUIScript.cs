using System.Collections.Generic;
using Assets.Source.Scripts.Base;
using Assets.Source.Scripts.BlackJack;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Source.Scripts.UI
{
    [RequireComponent(typeof(UIDocument))]
    internal class HandUIScript : MonoBehaviour
    {
        private UIDocument document;

        [SerializeField] private VisualTreeAsset handUIDocument;
        [SerializeField] private PlayerWalletUI playerWalletUI;
        [SerializeField] private GameScript gameScript;

        private Camera Camera => _Camera ??= Camera.main;
        private Camera _Camera;

        void Awake()
        {
            document = this.GetComponent<UIDocument>();
            gameScript.OnGameResult += (delay) => ShowResults(delay);
        }

        private List<HandUI> handUIs = new List<HandUI>();

        public void RegisterHand(BJHandScript hand)
        {
            var tree = handUIDocument.CloneTree();

            var handUI = new HandUI(hand, tree);

            hand.OnDoubleUp += () =>
            {
                playerWalletUI.DoubleBet();
                handUI.BetAmount = playerWalletUI.BetSum * 2;
            };

            handUIs.Add(handUI);

            document.rootVisualElement.Add(tree);

            var container = tree.Q("container");

            var screenPos = WorldToScreenSpace(hand.transform.position);

            tree.RegisterCallback<GeometryChangedEvent>((x) =>
            {
                var width = container.resolvedStyle.width;
                var height = container.resolvedStyle.height;

                tree.style.left = screenPos.x - width / 2;
                tree.style.top = screenPos.y - height / 2;
            });
        }

        private async void ShowResults(float delay)
        {
            handUIs.ForEach(x => x.Result = true);
            await TaskEx.Delay(delay);
            handUIs.ForEach(x => x.Result = x.Bet = false);
        }

        private Vector2 WorldToScreenSpace(Vector3 position)
        {
            var screenPointPosition = Camera.WorldToScreenPoint(position);
            var viewPortPoint = Camera.ScreenToViewportPoint(screenPointPosition);
            var referenceResolution = new Vector2(Screen.width, Screen.height);

            var screenPosition = new Vector2(
                viewPortPoint.x * referenceResolution.x,
                referenceResolution.y - viewPortPoint.y * referenceResolution.y);

            var scale = GetCanvasScaleFactor(document.panelSettings);

            return screenPosition / scale;
        }

        private static float GetCanvasScaleFactor(PanelSettings panelSettings)
        {
            return ((float)Screen.width / panelSettings.referenceResolution.x) * (1 - panelSettings.match) + ((float)Screen.height / panelSettings.referenceResolution.y) * (panelSettings.match);
        }
    }
}
