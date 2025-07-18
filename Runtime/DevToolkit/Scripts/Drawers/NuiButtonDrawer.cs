using Noo.Nui;
using System;

namespace Noo.DevToolkit
{
    public class NuiButtonDrawer : NuiDrawer
    {
        public readonly string buttonText;
        public readonly Action onClick;
        public readonly MatIcon iconRight = MatIcon.None;
        public readonly MatIcon iconLeft = MatIcon.None;

        public NuiButtonDrawer(string buttonText, MatIcon iconLeft = MatIcon.None, MatIcon iconRight = MatIcon.None, Action onClick = null)
        {
            this.buttonText = buttonText;
            this.onClick = onClick;
            this.iconRight = iconRight;
            this.iconLeft = iconLeft;
        }

        protected override void OnCreate()
        {
            var btn = NuiPool.Rent<NuiButton>().WithClass("dtk-drawer__inspector-button", "nui-btn-light-black").AppendTo(Root);
            btn.ButtonText = buttonText;
            btn.IconRight = iconRight;
            btn.IconLeft = iconLeft;
            btn.clicked += onClick;
        }

        protected override void OnDestroy()
        {
            var btn = Root.FirstChild<NuiButton>().WithoutClass("dtk-drawer__inspector-button", "nui-btn-light-black");
            btn.clicked -= onClick;
            NuiPool.Return(btn);
        }
    }
}
