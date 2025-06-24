using Noo.Nui;
using UnityEngine;
using UnityEngine.UIElements;

namespace Noo.DevToolkit
{
    internal class NuiTextDrawer : NuiDrawer
    {
        private readonly string text;
        private readonly bool centered;
        private readonly Color? color;

        public NuiTextDrawer(string text, bool centered = false, Color? color = null)
        {
            this.text = text;
            this.centered = centered;
            this.color = color;
        }

        protected override void OnCreate()
        {
            var el = NuiPool.Rent<NuiText>().WithClass("dtk-drawer__text").AppendTo(Root);
            if (centered) el.WithClass("text-center");
            if (color.HasValue) el.style.color = color.Value;
            el.text = text;
        }

        protected override void OnDestroy()
        {
            var textElement = Root.FirstChild<NuiText>();
            textElement.WithoutClass("dtk-drawer__text", "text-center");
            textElement.style.color = StyleKeyword.Null;
            NuiPool.Return(textElement);
        }
    }
}
