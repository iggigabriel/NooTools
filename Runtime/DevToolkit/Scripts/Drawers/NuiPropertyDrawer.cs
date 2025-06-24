using Noo.Nui;
using UnityEngine.UIElements;

namespace Noo.DevToolkit
{
    public abstract class NuiPropertyDrawer : NuiCompositeDrawer
    {
        public string Label { get; set; }
        public string Info { get; set; }

        public NuiProperty Property { get; internal set; }

        protected VisualElement LabelContainer { get; private set; }
        protected VisualElement PropertyContainer { get; private set; }
        protected NuiText LabelElement { get; private set; }
        protected NuiTooltipIcon MissingTargetIcon { get; private set; }
        protected UpdateManipulator UpdateManipulator { get; private set; }

        protected VisualElement Indicator { get; private set; }
        protected FocusListenerManipulator IndicatorManipulator { get; private set; }

        public override VisualElement ContentContainer => PropertyContainer;

        public override bool IsValid => Property.HasValidTarget(out var _);

        private bool propertyCreated = false;

        public virtual void OnInit()
        {
            UpdateManipulator = new UpdateManipulator(OnPropertyUpdate);
            IndicatorManipulator = new FocusListenerManipulator("nui--focused");
        }

        protected override void OnCreate()
        {
            Indicator = NuiPool.Rent<VisualElement>().WithClass("dtk-property-drawer__indicator").AppendTo(Root);
            Indicator.pickingMode = PickingMode.Ignore;

            UpdateManipulator.updateIfFocused = Property.IsReadOnly;

            Root.WithClass("dtk-property-drawer");
            Root.focusable = true;
            Root.delegatesFocus = true;
            Root.EnableInClassList("dtk--read-only", Property.IsReadOnly);
            Root.AddManipulator(IndicatorManipulator);

            if (!string.IsNullOrEmpty(Label))
            {
                LabelContainer = NuiPool.Rent<VisualElement>().WithClass("dtk-property-drawer__label-container").AppendTo(Root);
                LabelContainer.pickingMode = Property.IsReadOnly ? PickingMode.Ignore : PickingMode.Position;

                LabelElement = NuiPool.Rent<NuiText>().WithClass("dtk-property-drawer__label").AppendTo(LabelContainer);
                LabelElement.text = Label;
            }

            PropertyContainer = NuiPool.Rent<VisualElement>().WithClass("dtk-property-drawer__property-container").AppendTo(Root);

            base.OnCreate();

            propertyCreated = true;
            OnPropertyCreate();

            if (!Property.HasValidTarget(out var _))
            {
                LabelContainer.SetEnabled(false);
                PropertyContainer.SetEnabled(false);

                MissingTargetIcon = NuiPool.Rent<NuiTooltipIcon>()
                .WithClass("ml-2")
                .WithTooltip(MatIcon.Warning, $"Missing valid target for non-static member.")
                .AppendTo(LabelContainer);
            }
        }

        protected override void OnDestroy()
        {
            if (propertyCreated)
            {
                OnPropertyDestroy();
                propertyCreated = false;
            }

            if (MissingTargetIcon != null)
            {
                NuiPool.Return(MissingTargetIcon.WithoutClass("ml-2"));
                MissingTargetIcon = null;
            }

            if (Indicator != null)
            {
                Indicator.pickingMode = PickingMode.Position;
                NuiPool.Return(Indicator.WithoutClass("dtk-property-drawer__indicator"));
                Indicator = null;
            }

            if (PropertyContainer != null)
            {
                PropertyContainer.SetEnabled(true);
                NuiPool.Return(PropertyContainer.WithoutClass("dtk-property-drawer__property-container"));
                PropertyContainer = null;
            }

            if (LabelElement != null)
            {
                LabelContainer.pickingMode = PickingMode.Position;
                LabelContainer.SetEnabled(true);

                NuiPool.Return(LabelElement.WithoutClass("dtk-property-drawer__label"));
                NuiPool.Return(LabelContainer.WithoutClass("dtk-property-drawer__label-container"));
                LabelElement = null;
                LabelContainer = null;
            }

            Root.tooltip = default;
            Root.RemoveManipulator(IndicatorManipulator);
            Root.WithoutClass("dtk-property-drawer");
            Root.focusable = false;
            Root.delegatesFocus = false;

            base.OnDestroy();
        }

        protected abstract void OnPropertyCreate();
        protected abstract void OnPropertyDestroy();
        protected virtual void OnPropertyUpdate() { }
        protected virtual bool OnPropertyFilter(string query)
        {
            if (Property == null) return false;
            if (Property.Value == null) return false;
            return Property.Value.ToString().ToLowerInvariant().Contains(query);
        }

        protected sealed override bool OnFilter(string query)
        {
            // this is bad because all drawers are drawn when there is no search query
            if (!Property.HasValidTarget(out var _)) return false;

            return
                Label.Contains(query, System.StringComparison.InvariantCultureIgnoreCase) ||
                OnPropertyFilter(query) ||
                base.OnFilter(query);
        }
    }

    public abstract class NuiPropertyDrawer<T> : NuiPropertyDrawer
    {
        public T Value
        {
            get => Property.Value is T tValue ? tValue : default;
            protected set => Property.Value = value;
        }
    }
}
