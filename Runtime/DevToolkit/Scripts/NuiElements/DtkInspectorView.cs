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

        void TransitionStartEvent(TransitionStartEvent e)
        {
            style.display = DisplayStyle.Flex;
        }

        void TransitionEndEvent(TransitionEndEvent e)
        {
            if (resolvedStyle.opacity > 0f)
            {
                style.display = DisplayStyle.Flex;
                list.focusable = true;
            }
            else
            {
                list.ScrollToStart();
                list.focusable = false;
                list.SetItems(null);
                style.display = DisplayStyle.None;
            }
        }

        public void ConfigureEvents(bool enable)
        {
            if (enable)
            {
                RegisterCallback<TransitionStartEvent>(TransitionStartEvent);
                RegisterCallback<TransitionEndEvent>(TransitionEndEvent);
            }
            else
            {
                UnregisterCallback<TransitionStartEvent>(TransitionStartEvent);
                UnregisterCallback<TransitionEndEvent>(TransitionEndEvent);
            }
        }

        public void SetDrawers(IReadOnlyList<NuiDrawer> drawers)
        {
            list.ScrollToStart();

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
        }

        public void ClearStyle()
        {
            style.transitionDuration = StyleKeyword.Null;
            style.translate = StyleKeyword.Null;
        }
    }
}
