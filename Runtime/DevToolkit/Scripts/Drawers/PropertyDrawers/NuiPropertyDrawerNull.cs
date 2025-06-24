using Noo.Nui;

namespace Noo.DevToolkit
{
    public class NuiPropertyDrawerNull : NuiPropertyDrawer<object>
    {
        public override bool IsValid => false;

        NuiTooltipIcon icon;

        protected override void OnPropertyCreate()
        {
            Root.WithClass("dtk--inline");

            icon = NuiPool.Rent<NuiTooltipIcon>()
                .WithClass("ml-2")
                .WithTooltip(MatIcon.Report, $"Drawer not implemented for type: {DevToolkitUtility.GetFormattedTypeName(Property.PropertyType)}")
                .AppendTo(LabelContainer);
        }

        protected override void OnPropertyDestroy()
        {
            Root.WithoutClass("dtk--inline");

            if (icon != null)
            {
                NuiPool.Return(icon.WithoutClass("ml-2"));
                icon = null;
            }
        }
    }
}
