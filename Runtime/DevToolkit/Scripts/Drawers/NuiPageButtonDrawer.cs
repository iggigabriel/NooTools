using Noo.Nui;

namespace Noo.DevToolkit
{
    public class NuiPageButtonDrawer : NuiButtonDrawer
    {
        readonly string pagePath;

        public string buttonClass;

        public override bool IsValid => DevToolkit.Commands.PageHasValidItems(pagePath);

        public NuiPageButtonDrawer(string path, string displayName) : base(displayName, MatIcon.None, MatIcon.ChevronRight, () => DevToolkit.Commands.ShowPage(path))
        {
            pagePath = path;
            Order = 1000000;
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
