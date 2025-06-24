using Noo.Nui;

namespace Noo.DevToolkit
{
    public class NuiPageButtonDrawer : NuiButtonDrawer
    {
        public string buttonClass;

        public NuiPageButtonDrawer(string path, string displayName) : base(displayName, MatIcon.ChevronRight, () => DevToolkit.Commands.ShowPage(path))
        {
            Order = -1000000;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            var btn = Root.FirstChild<NuiButton>();
            btn.WithClass("dtk-drawer__page-button", buttonClass);
        }

        protected override void OnDestroy()
        {
            var btn = Root.FirstChild<NuiButton>();
            btn.WithoutClass("dtk-drawer__page-button", buttonClass);
            base.OnDestroy();
        }
    }
}
