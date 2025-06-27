using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    public partial class NuiWindow : VisualElement
    {
        public readonly VisualElement background;
        public readonly VisualElement container;

        public readonly NuiRow topbar;
        public readonly NuiRow body;
        public readonly NuiRow statusbar;

        public readonly NuiButton closeButton;
        public readonly TextElement windowTitle;

        readonly DragManipulator dragManipulator;

        public override VisualElement contentContainer => body;

        public string Title
        {
            get => windowTitle.text;
            set => windowTitle.text = value;
        }

        public bool ShowStatusbar
        {
            get => !statusbar.ClassListContains("hide");
            set => statusbar.EnableInClassList("hide", !value);
        }

        public bool IsDraggable
        {
            get => dragManipulator.target == topbar;
            set
            {
                dragManipulator.target = value ? topbar : null;

                if (value)
                {

                }
                else
                {
                    style.top = StyleKeyword.Null;
                    style.left = StyleKeyword.Null;
                }
            }
        }

        public NuiWindow()
        {
            usageHints = UsageHints.DynamicTransform;
            dragManipulator = new DragManipulator(this, OnDrag);

            AddToClassList("nui-window");

            background = new VisualElement().WithClass("nui-window__background").AppendToHierarchy(this);
            background.pickingMode = PickingMode.Ignore;

            container = new VisualElement().WithClass("nui-window__container").AppendToHierarchy(this);

            topbar = new NuiRow().WithClass("nui-window__topbar", "flex-noshrink");

            windowTitle = new NuiText().WithClass("nui-window__title", "p-2", "flex-grow").AppendToHierarchy(topbar);

            closeButton = new NuiButton().WithClass("nui-btn-red-flat", "p-2", "nui-btn-circle").AppendToHierarchy(topbar);
            closeButton.IconLeft = MatIcon.Close;

            statusbar = new NuiRow().WithClass("nui-window__statusbar", "flex-noshrink");

            body = new NuiRow().WithClass("nui-window__body");

            container.Add(topbar);
            container.Add(body);
            container.Add(statusbar);
        }

        private void OnDrag()
        {
            style.top = Mathf.RoundToInt(resolvedStyle.top);
            style.left = Mathf.RoundToInt(resolvedStyle.left);
        }
    }
}
