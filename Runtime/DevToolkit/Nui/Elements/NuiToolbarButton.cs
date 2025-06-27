using UnityEngine;

namespace Noo.Nui
{
    public class NuiToolbarButton : NuiButton
    {
        public NuiToolbarButton() : this(string.Empty)
        {
        }

        public NuiToolbarButton(MatIcon icon) : this(string.Empty, icon)
        {
        }

        public NuiToolbarButton(string text, MatIcon iconLeft = MatIcon.None, MatIcon iconRight = MatIcon.None) : base(text, iconLeft, iconRight)
        {
            AddToClassList("nui-toolbar-btn");
        }
    }
}
