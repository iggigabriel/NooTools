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

            RegisterCallback<TransitionStartEvent>((e) =>
            {
                style.display = DisplayStyle.Flex;
            });

            RegisterCallback<TransitionEndEvent>((e) =>
            {
                if (resolvedStyle.opacity > 0f)
                {
                    style.display = DisplayStyle.Flex;
                    list.focusable = true;
                }
                else
                {
                    list.focusable = false;
                    list.SetItems(null);
                    style.display = DisplayStyle.None;
                }
            });

            list = new NuiListView<NuiDrawer>
            (
                (x, i) => { x.Create(); return x.Root; },
                (x, el) => x.Destroy()
            );

            list.WithClass("dtk-inspector-view__list").AppendTo(this);

            validDrawers = new();
        }

        public void SetDrawers(IReadOnlyList<NuiDrawer> drawers)
        {
            validDrawers.Clear();

            for (int i = 0; i < drawers.Count; i++)
            {
                var drawer = drawers[i];

                if (drawer.IsValid) validDrawers.Add(drawer);
            }

            list.SetItems(validDrawers);
        }
    }
}
