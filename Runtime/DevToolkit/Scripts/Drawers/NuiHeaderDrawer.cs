using Noo.Nui;

namespace Noo.DevToolkit
{
    public class NuiHeaderDrawer : NuiDrawer
    {
        private readonly string title;
        private readonly bool centered;

        public NuiHeaderDrawer(string title, bool centered = false)
        {
            this.title = title;
            this.centered = centered;
        }

        protected override void OnCreate()
        {
            var el = NuiPool.Rent<NuiText>().WithClass("dtk-drawer__header").AppendTo(Root);
            if (centered) el.WithClass("text-center");
            el.text = title;
        }

        protected override void OnDestroy()
        {
            NuiPool.Return(Root.FirstChild<NuiText>().WithoutClass("dtk-drawer__header", "text-center"));
        }

        protected override bool OnFilter(string query)
        {
            return false;
        }
    }
}
