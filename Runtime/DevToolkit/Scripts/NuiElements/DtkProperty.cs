using Noo.Nui;
using UnityEngine.UIElements;

namespace Noo.DevToolkit
{
    public partial class DtkProperty : VisualElement
    {
        public override VisualElement contentContainer => PropertyContainer ?? this;

        protected VisualElement LabelContainer { get; private set; }
        protected VisualElement PropertyContainer { get; private set; }
        protected NuiText LabelElement { get; private set; }
        protected VisualElement Indicator { get; private set; }

        public DtkProperty() : base()
        {
            AddToClassList("dtk-property");

            Indicator = NuiPool.Rent<VisualElement>().WithClass("dtk-property-drawer__indicator").AppendTo(this);
            Indicator.pickingMode = PickingMode.Ignore;

            LabelContainer = NuiPool.Rent<VisualElement>().WithClass("dtk-property-drawer__label-container").AppendTo(this);
            LabelElement = NuiPool.Rent<NuiText>().WithClass("dtk-property-drawer__label").AppendTo(LabelContainer);

            PropertyContainer = NuiPool.Rent<VisualElement>().WithClass("dtk-property-drawer__property-container").AppendToHierarchy(this);
        }
    }
}
