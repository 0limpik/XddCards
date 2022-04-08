using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Xdd.Scripts.Bets;

namespace Xdd.UI.Elements
{
    internal class ChipElement : Button
    {
        private static readonly string d_SpritePath = "Assets/Models/Chips/chip_0_25.asset";
        private static readonly string d_Value = "00000";
        private static readonly string d_Postfix = "$";

        public new class UxmlFactory : UxmlFactory<ChipElement, UxmlTraits> { }

        public new class UxmlTraits : Button.UxmlTraits
        {
            UxmlStringAttributeDescription m_SpritePath = new() { name = "sprite-path", defaultValue = d_SpritePath };
            UxmlStringAttributeDescription m_Value = new() { name = "value", defaultValue = d_Value };
            UxmlStringAttributeDescription m_Postfix = new() { name = "postfix", defaultValue = d_Postfix };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                var chipElement = visualElement as ChipElement;

                chipElement.ImagePath = m_SpritePath.GetValueFromBag(bag, context);
                chipElement.Value = m_Value.GetValueFromBag(bag, context);
                chipElement.Postfix = m_Postfix.GetValueFromBag(bag, context);
            }
        }

        public ChipElement()
            : base()
        {
            Generate();
            InitDefault();
        }

        public ChipElement(PossibleBet bet, Action click = null, string postfix = null)
           : base(click)
        {
            Generate();
            Bet = bet;
            Sprite = bet.Sprite;
            Value = bet.Amount.ToString();
            Postfix = postfix;
        }

        private void Generate()
        {
            Clear();
            imageContainer = AddElement(new VisualElement { name = "image-container" });
            image = AddElement(new VisualElement { name = "image" }, imageContainer);

            valueContainer = AddElement(new VisualElement { name = "value-container" });
            valueContainer.style.flexDirection = FlexDirection.Row;
            valueLabel = AddElement(new Label { name = "value" }, valueContainer);
            postfixLabel = AddElement(new Label { name = "postfix" }, valueContainer);

            T AddElement<T>(T element, VisualElement container = null, string className = null) where T : VisualElement
            {
                container ??= this;
                element.AddToClassList(className ?? element.name);
                container.Add(element);
                return element;
            }
        }

        private void InitDefault()
        {
            ImagePath = d_SpritePath;
            Value = d_Value;
            Postfix = d_Postfix;
        }

        private VisualElement imageContainer;
        private VisualElement image;

        private VisualElement valueContainer;
        private Label valueLabel;
        private Label postfixLabel;

        public string Value
        {
            get => valueLabel.text;
            set => valueLabel.text = value;
        }

        public string Postfix
        {
            get => postfixLabel.text;
            set => postfixLabel.text = value;
        }

        public string ImagePath
        {
            get => _ImagePath;
            set
            {
                _ImagePath = value;

                _Sprite = AssetDatabase.LoadAssetAtPath<Sprite>(value);
                image.style.backgroundImage = new StyleBackground(_Sprite);
            }
        }
        private string _ImagePath;

        public Sprite Sprite
        {
            get => _Sprite;
            set
            {
                _ImagePath = null;

                _Sprite = value;
                image.style.backgroundImage = new StyleBackground(_Sprite);
            }
        }
        private Sprite _Sprite;

        public PossibleBet Bet { get; private set; }
    }
}
