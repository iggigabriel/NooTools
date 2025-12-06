using UnityEngine;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    [AddComponentMenu("")]
    public class NuiTooltipSystem : NuiSystem
    {
        EventCallback<GeometryChangedEvent> setTooltipPostion;
        EventCallback<PointerEnterEvent> onPointerEnter;
        EventCallback<PointerLeaveEvent> onPointerExit;

        TextElement tooltipElement;
        VisualElement target;

        const float hoverToShow = 0.5f;
        const float hoverToHide = 0.1f;

        float timeToShow;
        float timeToHide;

        bool tooltipPositionDirty = true;

        public bool IsShowing => tooltipElement.parent != null;

        protected override void Awake()
        {
            base.Awake();

            tooltipElement = new TextElement().WithClass("nui-tooltip");
            tooltipElement.pickingMode = PickingMode.Ignore;
            tooltipElement.usageHints = UsageHints.DynamicTransform;

            setTooltipPostion = SetTooltipPosition;
            onPointerEnter = OnPointerEnter;
            onPointerExit = OnPointerExit;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            Panel?.visualTree.RegisterCallback(onPointerEnter, TrickleDown.TrickleDown);
            Panel?.visualTree.RegisterCallback(onPointerExit, TrickleDown.TrickleDown);

            tooltipElement.RegisterCallback(setTooltipPostion);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Panel?.visualTree.UnregisterCallback(onPointerEnter, TrickleDown.TrickleDown);
            Panel?.visualTree.UnregisterCallback(onPointerExit, TrickleDown.TrickleDown);

            tooltipElement.UnregisterCallback(setTooltipPostion);
        }

        bool TryGetValidTooltipTarget(IEventHandler target, out VisualElement element)
        {
            if (target == null || target is not VisualElement el)
            {
                element = null;
                return false;
            }

            element = el;

            while (element != null && string.IsNullOrEmpty(element.tooltip))
            {
                element = element.parent;
            }

            return element != null;
        }

        private void OnPointerEnter(PointerEnterEvent e)
        {
            if (!TryGetValidTooltipTarget(e.target, out var element)) return;

            if (IsShowing)
            {
                Hide();
                target = element;
                Show();

                NuiTask.EveryUpdate -= QueueTooltipHide;
                NuiTask.EveryUpdate -= QueueTooltipShow;
            }
            else
            {
                target = element;
                QueueShow();
            }
        }

        private void OnPointerExit(PointerLeaveEvent e)
        {
            if (e.target == target) QueueHide();
        }

        private void QueueShow()
        {
            timeToShow = Time.unscaledTime + hoverToShow;
            NuiTask.EveryUpdate -= QueueTooltipHide;
            NuiTask.EveryUpdate -= QueueTooltipShow;
            NuiTask.EveryUpdate += QueueTooltipShow;
        }

        private void QueueHide()
        {
            timeToHide = Time.unscaledTime + hoverToHide;
            NuiTask.EveryUpdate -= QueueTooltipShow;
            NuiTask.EveryUpdate -= QueueTooltipHide;
            NuiTask.EveryUpdate += QueueTooltipHide;
        }

        private void QueueTooltipShow()
        {
            if (Time.unscaledTime > timeToShow)
            {
                NuiTask.EveryUpdate -= QueueTooltipShow;
                Show();
            }
        }

        private void QueueTooltipHide()
        {
            if (Time.unscaledTime > timeToHide)
            {
                NuiTask.EveryUpdate -= QueueTooltipShow;
                Hide();
            }
        }

        private void Show()
        {
            if (target == null || Panel == null) return;

            tooltipPositionDirty = true;

            tooltipElement.text = target.tooltip;
            Panel.visualTree.Add(tooltipElement);
        }

        private void Hide()
        {
            if (target == null || !IsShowing) return;

            tooltipElement.RemoveFromHierarchy();
            NuiTask.EveryUpdate -= QueueTooltipShow;
            target = null;
        }

        private void SetTooltipPosition(GeometryChangedEvent e)
        {
            if (!tooltipPositionDirty) return;
            if (target == null || tooltipElement.parent == null || Panel == null) return;

            var tooltipSize = tooltipElement.contentRect.size + new Vector2(10f, 10f);
            var targetRect = target.worldBound;
            var panelSize = Panel.visualTree.contentRect.size;

            if (float.IsNaN(tooltipSize.x) || float.IsNaN(targetRect.width)) return;

            var targetCenter = targetRect.center;
            var tooltipPos = targetCenter + new Vector2(-tooltipSize.x / 2f, targetRect.height / 2f);

            tooltipPos.x = Mathf.Clamp(tooltipPos.x, 0f, panelSize.x - tooltipSize.x - 8f);

            if (tooltipPos.y > panelSize.y - tooltipSize.y)
            {
                tooltipPos.y = targetCenter.y - targetRect.height / 2f - tooltipSize.y;
            }

            tooltipPos = Vector2Int.RoundToInt(tooltipPos);

            tooltipElement.style.translate = tooltipPos;
            tooltipPositionDirty = false;
        }
    }
}
