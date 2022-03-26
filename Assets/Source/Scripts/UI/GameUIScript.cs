using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Source.Scripts.Cash;
using Assets.Source.Scripts.UI;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteAlways]
[RequireComponent(typeof(UIDocument))]
public class GameUIScript : MonoBehaviour
{
    private UIDocument document => _document ??= this.GetComponent<UIDocument>();
    private UIDocument _document;

    [SerializeField] private VisualTreeAsset playerDocument;

    Camera Camera => _Camera ??= Camera.main;
    Camera _Camera;

    public float gameResultDelayPerUser;

    private Label centerMessage;
    private Label cashLabel;
    private Label betLabel;

    void Awake()
    {
        centerMessage = document.rootVisualElement.Q<Label>("center-message");
        cashLabel = document.rootVisualElement.Q<Label>("cash-count");
        betLabel = document.rootVisualElement.Q<Label>("bet-count");
    }

    public async void DisplayMessage(string message, Color? color = null)
    {
        centerMessage.text = message;
        centerMessage.style.color = color ?? Color.white;
        await Task.Delay(3000);
        centerMessage.text = null;
    }

    private List<HandUI> handUIs = new List<HandUI>();

    public void RegisterHand(BJHandScript hand)
    {
        var tree = playerDocument.CloneTree();

        var handUI = new HandUI(hand, tree);

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

    public async Task ShowResults()
    {
        handUIs.ForEach(x => x.Result = true);
        await TaskEx.Delay(handUIs.Count * gameResultDelayPerUser);
        handUIs.ForEach(x => x.Result = false);
    }

    public void RegisterWallet(PlayerWallet wallet)
    {
        wallet.OnChange += () =>
        {
            cashLabel.text = wallet.Cash.ToString();
            betLabel.text = wallet.AllBets.ToString();
        };
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

    public static float GetCanvasScaleFactor(PanelSettings panelSettings)
    {
        return ((float)Screen.width / panelSettings.referenceResolution.x) * (1 - panelSettings.match) + ((float)Screen.height / panelSettings.referenceResolution.y) * (panelSettings.match);
    }
}
