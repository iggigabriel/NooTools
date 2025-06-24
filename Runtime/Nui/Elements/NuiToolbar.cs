using UnityEngine;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    public partial class NuiToolbar : NuiRow
    {
        public readonly NuiRow left;
        public readonly NuiRow middle;
        public readonly NuiRow right;

        public NuiToolbar() : base()
        {
            AddToClassList("nui-toolbar");

            left = new NuiRow().WithClass("nui-toolbar__left", "flex-noshrink");
            middle = new NuiRow().WithClass("nui-toolbar__middle", "flex-grow");
            right = new NuiRow(true).WithClass("nui-toolbar__right", "flex-noshrink");

            Add(left);
            Add(middle);
            Add(right);
            Add(new VisualElement().WithClass("nui-toolbar-bottom-border"));
        }
    }
}
