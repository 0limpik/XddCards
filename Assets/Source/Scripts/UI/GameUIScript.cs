using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Source.Model.Games.BlackJack;
using Assets.Source.Model.Games.BlackJack.Users;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class PlayerUIObject : ScriptableObject
{
    public int[] Scores
    {
        get => _Score;
        set
        {
            _Score = value;

            if (_Score.Count() > 0)
            {
                var less = _Score.Where(x => x <= 21);

                if (less.Count() > 0)
                {
                    displayScore = string.Join(" / ", less);
                }
                else
                {
                    displayScore = _Score.Min().ToString();
                }
            }
            else
            {
                displayScore = "0";
            }

            displayScoreAction.Invoke(_Score);
        }
    }
    private int[] _Score;

    public string displayScore;

    public Action<int[]> displayScoreAction;
}


[RequireComponent(typeof(UIDocument))]
public class GameUIScript : MonoBehaviour
{
    private UIDocument document;
    [SerializeField] private VisualTreeAsset buttonsStyle;
    [SerializeField] private VisualTreeAsset playerDocument;
    Camera _camera;

    public ButtonData[] buttonsData;

    private Label centerMessage;

    void Awake()
    {
        document = this.GetComponent<UIDocument>();
        _camera = Camera.main;

        var _container = document.rootVisualElement.Q<VisualElement>("action-buttons");
        centerMessage = document.rootVisualElement.Q<Label>("center-message");

        foreach (var data in buttonsData)
        {
            var newElement = buttonsStyle.CloneTree();
            var button = newElement.Q<Button>("action-button-default");
            button.text = data.text;
            button.clicked += () => data.events.Invoke();
            _container.Add(button);
        }
    }

    public async void DisplayMessage(string message, Color? color = null)
    {
        centerMessage.text = message;
        centerMessage.style.color = color ?? Color.white;
        await Task.Delay(2500);
        centerMessage.text = null;
    }

    private List<(HandScript, Label)> handList = new List<(HandScript, Label)>();
    public void AddScore(HandScript hand)
    {
        var newElement = buttonsStyle.CloneTree();
        var label = newElement.Q<Label>("score-label");

        var screenPos = WorldToScreenSpace(hand.transform.position);

        label.style.left = screenPos.x;
        label.style.bottom = screenPos.y - 125;
        document.rootVisualElement.Add(label);
        handList.Add((hand, label));
    }

    public void SetScore(HandScript hand, string score)
    {
        var item = handList.First(x => x.Item1 == hand);

        item.Item2.text = score;
    }

    public void RegisterPlayer(HandScript hand, IBlackJack game)
    {
        RegisterUI(hand, game, true);
    }

    public void RegisterDiller(HandScript hand)
    {
        RegisterUI(hand, null, false);
    }

    private void RegisterUI(HandScript hand, IBlackJack game, bool addButton)
    {
        var tree = playerDocument.CloneTree();
        tree.Bind(new SerializedObject(hand.playerUIObject));

        var scoreText = tree.Q<Label>("score-text");

        hand.playerUIObject.displayScoreAction += (scores) =>
        {
            if(scores.Count() == 0)
            {
                scoreText.style.color = Color.yellow;
                return;
            }
            if (scores.All(x => x > 21))
            {
                scoreText.style.color = Color.red;
                return;
            }
            var less = scores.Where(x => x <= 21);
            if (less.Any(x => x >= 17))
            {
                scoreText.style.color = Color.green;
                return;
            }
            if (less.Any(x => x <= 17))
            {
                scoreText.style.color = Color.yellow;
                return;
            }
        };

        if (addButton)
        {
            tree.Q<Button>("hit").clicked += () => game.Hit();
            tree.Q<Button>("stand").clicked += () => game.Stand();
        }
        else
        {
            tree.Q("game-container").Remove(tree.Q("buttons-container"));
        }

        var screenPos = WorldToScreenSpace(hand.transform.position);

        tree.RegisterCallback<GeometryChangedEvent>(async (x) =>
        {
            await Task.Yield();

            tree.style.left = screenPos.x - tree.Q("container").resolvedStyle.width / 2;
            tree.style.bottom = screenPos.y + 125;
        });


        var results = tree.Q("result-images");

        var all = new AllResults
        (
            tree.Q("game-result"),
            hand.playerUIObject,
            hand.user
        );
        allResults.Add(all);

        document.rootVisualElement.Add(tree);
    }

    private List<AllResults> allResults = new List<AllResults>();

    public async void ShowResults(IBlackJack game)
    {
        foreach (var item in allResults)
        {
            item.EnableNeed();
        }

        await TaskEx.Delay(3f);

        foreach (var item in allResults)
        {
            item.DisableAll();
        }
    }

    private class AllResults
    {
        private VisualElement win;
        private VisualElement lose;
        private VisualElement push;
        private VisualElement bust;
        private VisualElement container;
        private PlayerUIObject playerUIObject;
        private Label resultText;

        private VisualElement[] all;

        private IUser user;

        public AllResults(VisualElement container, PlayerUIObject playerUIObject, IUser user)
        {
            this.win = container.Q("win");
            this.lose = container.Q("lose");
            this.push = container.Q("push");
            this.bust = container.Q("bust");
            this.resultText = container.Q<Label>("result-text");

            this.container = container;
            this.playerUIObject = playerUIObject;

            all = new VisualElement[] { win, lose, push, bust };

            this.user = user;
        }

        public void EnableNeed()
        {
            switch (user.GetStatus())
            {
                case PlayerStatus.Bust:
                    SetVisible(bust);
                    resultText.text = "Bust";
                    resultText.style.color = Color.red;
                    break;
                case PlayerStatus.Win:
                    SetVisible(win);
                    resultText.text = "Win";
                    resultText.style.color = Color.green;
                    break;
                case PlayerStatus.Lose:
                    SetVisible(lose);
                    resultText.text = "Lose";
                    resultText.style.color = Color.red;
                    break;
                case PlayerStatus.Push:
                    SetVisible(push);
                    resultText.text = "Push";
                    resultText.style.color = Color.yellow;
                    break;
            }
        }

        private void SetVisible(VisualElement element)
        {
            container.visible = true;
            all.Where(x => x != element).ToList().ForEach(x => x.style.display = DisplayStyle.None);
            element.style.display = DisplayStyle.Flex;
        }

        public void DisableAll()
        {
            container.visible = false;
            all.ToList().ForEach(x => x.style.display = DisplayStyle.None);
        }

    }

    private Vector2 WorldToScreenSpace(Vector3 position)
    {
        var screenPointPosition = _camera.WorldToScreenPoint(position);
        var viewPortPoint = _camera.ScreenToViewportPoint(screenPointPosition);
        var referenceResolution = new Vector2(Screen.width, Screen.height);

        var screenPosition = new Vector2(
            viewPortPoint.x * referenceResolution.x,
            viewPortPoint.y * referenceResolution.y);

        var scale = GetCanvasScaleFactor(document.panelSettings);

        return screenPosition / scale;
    }

    public static float GetCanvasScaleFactor(PanelSettings panelSettings)
    {
        return ((float)Screen.width / panelSettings.referenceResolution.x) * (1 - panelSettings.match) + ((float)Screen.height / panelSettings.referenceResolution.y) * (panelSettings.match);
    }
}

[Serializable]
public class ButtonData
{
    public string text;
    public UnityEvent events;
}
