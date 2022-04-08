using System.Threading.Tasks;
using Assets.Source.Scripts.BlackJack;
using UnityEngine;
using UnityEngine.UIElements;
using Xdd.UI.Elements;

namespace Assets.Source.Scripts.UI.Elements
{
    internal class GameStateElement
    {
        public ValueLabelElement Element { get; private set; }

        public GameState Preset { get; private set; }

        private string postfix;

        public GameStateElement Init(GameState preset, string postfix)
        {
            this.Preset = preset;
            this.postfix = postfix;

            var state = new ValueLabelElement()
            {
                ValueName = preset.DisplayName,
                Value = null,
            };

            Element = state;

            return this;
        }

        public async void SetEnable(float? delay)
        {
            Element.SetEnabled(true);

            if (delay != null)
            {
                Element.ValuePostfix = postfix;
                var start = Time.time + (float)delay;
                while (start > Time.time)
                {
                    Element.Value = (start - Time.time).ToString("0");
                    await Task.Yield();
                }
                Element.Value = null;
                Element.ValuePostfix = null;

                Element.SetEnabled(false);
            }
        }

        public void SetDisable()
        {
            Element.SetEnabled(false);
        }

        public static implicit operator VisualElement(GameStateElement element) => element.Element;
    }
}
