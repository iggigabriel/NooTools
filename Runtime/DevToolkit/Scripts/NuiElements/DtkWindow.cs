using Noo.Nui;

namespace Noo.DevToolkit
{
    public partial class DtkWindow : NuiWindow
    {
        public readonly NuiButton appsButton;

        public DtkWindow() : base()
        {
            AddToClassList("dtk-window");

            appsButton = new NuiButton().WithClass("p-2", "nui-btn-circle").PrependToToHierarchy(topbar);
            appsButton.IconLeft = MatIcon.Apps;
        }
    }
}
