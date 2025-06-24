using Noo.Nui;

namespace Noo.DevToolkit
{
    internal class NuiDebugDrawer : NuiDrawer
    {
        private readonly string text;

        public NuiDebugDrawer(string text)
        {
            this.text = text;
        }

        protected override void OnCreate()
        {
            var el = NuiPool.Rent<NuiText>().WithClass("dtk-drawer__debug").AppendTo(Root);
            el.text = text;
        }

        protected override void OnDestroy()
        {
            NuiPool.Return(Root.FirstChild<NuiText>().WithoutClass("dtk-drawer__debug"));
        }
    }
}
