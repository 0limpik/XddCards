using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Xdd.UI.Elements
{
    public class ValueLabelElement : VisualElement
    {
        private static readonly string d_ValueName = "Name Abc123";
        private static readonly string d_Value = "00000";
        private static readonly string d_ValuePrefix = string.Empty;
        private static readonly string d_ValuePostfix = string.Empty;
        private static readonly bool d_ValueVertical = false;

        public new class UxmlFactory : UxmlFactory<ValueLabelElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_ValueName = new() { name = "value-name", defaultValue = d_ValueName };
            UxmlStringAttributeDescription m_Value = new() { name = "value", defaultValue = d_Value };
            UxmlStringAttributeDescription m_ValuePrefix = new() { name = "value-prefix", defaultValue = d_ValuePrefix };
            UxmlStringAttributeDescription m_ValuePostfix = new() { name = "value-postfix", defaultValue = d_ValuePostfix };
            UxmlBoolAttributeDescription m_ValueVertical = new() { name = "value-vertical", defaultValue = d_ValueVertical };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                var valueLabel = visualElement as ValueLabelElement;

                valueLabel.ValueName = m_ValueName.GetValueFromBag(bag, context);
                valueLabel.Value = m_Value.GetValueFromBag(bag, context);
                valueLabel.ValuePrefix = m_ValuePrefix.GetValueFromBag(bag, context);
                valueLabel.ValuePostfix = m_ValuePostfix.GetValueFromBag(bag, context);
                valueLabel.ValueVertical = m_ValueVertical.GetValueFromBag(bag, context);
            }
        }

        public ValueLabelElement()
        {
            //Generate element
            Clear();
            nameLabel = AddElement(new Label { name = "name" });
            divider = AddElement(new VisualElement { name = "divider" });
            container = AddElement(new VisualElement { name = "container" });
            prefixLabel = AddElement(new Label { name = "prefix" }, container);
            valueLabel = AddElement(new Label("00000") { name = "value" }, container);
            postfixLabel = AddElement(new Label { name = "postfix" }, container);

            T AddElement<T>(T element, VisualElement container = null, string className = null) where T : VisualElement
            {
                container ??= this;
                element.AddToClassList(className ?? element.name);
                container.Add(element);
                return element;
            }
            InitDefault();
        }
        private void InitDefault()
        {
            ValueName = d_ValueName;
            Value = d_Value;
            ValuePrefix = d_ValuePrefix;
            ValuePostfix = d_ValuePostfix;
            ValueVertical = d_ValueVertical;
        }

        private Label valueLabel;
        private VisualElement container;

        private VisualElement divider;

        private Label nameLabel;
        private Label prefixLabel;
        private Label postfixLabel;
        public string Value
        {
            get => valueLabel.text;
            set { valueLabel.text = value; CheckDivider(); }
        }

        public string ValueName
        {
            get => nameLabel.text;
            set => nameLabel.text = value;
        }
        public string ValuePrefix
        {
            get => prefixLabel.text;
            set { prefixLabel.text = value; CheckDivider(); }
        }
        public string ValuePostfix
        {
            get => postfixLabel.text;
            set { postfixLabel.text = value; CheckDivider(); }
        }

        public bool ValueVertical
        {
            get => _ValueVertical;
            set
            {
                _ValueVertical = value;

                if (value)
                {
                    this.style.flexDirection = FlexDirection.Column;
                    container.style.marginRight = 0;
                }
                else
                {
                    this.style.flexDirection = FlexDirection.Row;
                }
            }
        }
        private bool _ValueVertical;

        private void CheckDivider()
        {
            if (string.IsNullOrEmpty(ValuePrefix) && string.IsNullOrEmpty(Value) && string.IsNullOrEmpty(ValuePostfix))
                divider.style.display = DisplayStyle.None;
            else
                divider.style.display = DisplayStyle.Flex;
        }
    }
}
