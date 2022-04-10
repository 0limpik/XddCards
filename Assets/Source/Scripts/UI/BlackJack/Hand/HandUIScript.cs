using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Xdd.Scripts.Base;
using Xdd.Scripts.BlackJack;

namespace Xdd.Scripts.UI.BlackJack.Hand
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

        private List<HandUI> handUIs = new List<HandUI>();

        private Vector2 resolution;

        void Awake()
        {
            document = this.GetComponent<UIDocument>();
            gameScript.OnGameResult += ShowResults;
        }

        void FixedUpdate()
        {
            if (Screen.width != resolution.x || Screen.height != resolution.y)
            {
                resolution = new Vector2(Screen.width, Screen.height);

                foreach (var handUI in handUIs)
                {
                    SetHandUIPosition(handUI);
                }
            }
        }

        public async void RegisterHand(BJHandScript hand)
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

            await TaskEx.Delay(0.1f);

            SetHandUIPosition(handUI);
        }

        private void SetHandUIPosition(HandUI handUI)
        {
            var screenPos = WorldToScreenSpace(handUI.handScript.transform.position);

            var width = handUI.container.resolvedStyle.width;
            var height = handUI.container.resolvedStyle.height;

            handUI.tree.style.left = screenPos.x - width / 2;
            handUI.tree.style.top = screenPos.y - height / 2;
        }

        private async void ShowResults(float delay)
        {
            handUIs.ForEach(x => x.SetResult(true));
            await TaskEx.Delay(delay);
            handUIs.ForEach(x => x.SetResult(false));
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
