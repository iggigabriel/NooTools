using Noo.Nui;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Noo.DevToolkit
{
    public class DtkInspectorView : VisualElement
    {
        readonly NuiListView<NuiDrawer> list;
        readonly List<NuiDrawer> validDrawers;

        public DtkInspectorView() : base()
        {
            AddToClassList("dtk-inspector-view");

            usageHints = UsageHints.DynamicTransform | UsageHints.DynamicColor;

            list = new NuiListView<NuiDrawer>
            (
                (x, i) => { x.Create(); return x.Root; },
                (x, el) => x.Destroy()
            );

            list.WithClass("dtk-inspector-view__list").AppendTo(this);

            validDrawers = new();
        }

        public void Show()
        {
            style.display = DisplayStyle.Flex;
            list.focusable = true;
            list.ScrollToStart();
        }

        public void Hide()
        {
            list.ScrollToStart();
            SetDrawers(null);
            list.focusable = false;
            style.display = DisplayStyle.None;
        }

        public void SetDrawers(IReadOnlyList<NuiDrawer> drawers)
        {
            validDrawers.Clear();

            if (drawers != null)
            {
                for (int i = 0; i < drawers.Count; i++)
                {
                    var drawer = drawers[i];

                    if (drawer.IsValid) validDrawers.Add(drawer);
                }
            }

            list.SetItems(validDrawers);

            list.ScrollToStart();
        }
    }
}
