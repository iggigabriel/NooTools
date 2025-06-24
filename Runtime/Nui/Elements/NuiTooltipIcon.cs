using UnityEngine;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    public class NuiTooltipIcon : NuiIconMat
    {
        public NuiTooltipIcon() : base()
        {
            AddToClassList("nui-tooltip-icon");
            pickingMode = PickingMode.Position;
        }

        public override void OnReturnToPool()
        {
            base.OnReturnToPool();
            tooltip = null;
            pickingMode = PickingMode.Position;
        }

        public NuiTooltipIcon WithTooltip(MatIcon icon, string tooltip)
        {
            Icon = icon;
            this.tooltip = tooltip;
            return this;
        }
    }
}
