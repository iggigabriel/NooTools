using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    public interface IDropdownElement
    {
        void OnDropdownAfterShow();
        void OnDropdownBeforeHide();
    }

    [AddComponentMenu("")]
    public class NuiDropdownSystem : NuiSystem
    {
        const float PANEL_PADDING = 4f;
        const float TARGET_PADDING = -1f;

        private VisualElement activeDropdown;
        private Action activeHideCallback;
        private Rect activeTargetRect;
        private bool activeInitialized;

        protected override void OnDisable()
        {
            base.OnDisable();
            HideDropdown(activeDropdown);
        }

        public void ShowAsDropdown(VisualElement element, Rect rect, Action hideCallback)
        {
            if (element.hierarchy.parent != null) throw new NotSupportedException("Cannot display dropdown, Element is already in hierachy.");

            if (activeDropdown != null) HideDropdown(element);

            activeDropdown = element;
            activeTargetRect = rect;
            activeHideCallback = hideCallback;

            activeTargetRect.position += new Vector2(TARGET_PADDING, TARGET_PADDING);
            activeTargetRect.size -= new Vector2(TARGET_PADDING * 2f, TARGET_PADDING * 2f);
            activeTargetRect.width = Mathf.Max(0, activeTargetRect.width);
            activeTargetRect.height = Mathf.Max(0, activeTargetRect.height);

            Panel.visualTree.RegisterCallback<PointerDownEvent>(OnPanelClick, TrickleDown.TrickleDown);
            Panel.visualTree.RegisterCallback<NavigationCancelEvent>(OnCancelClick, TrickleDown.TrickleDown);
            Panel.visualTree.pickingMode = PickingMode.Position;

            NuiTask.OnEndOfFrame(AddActiveElement, 0);

            activeInitialized = false;
        }

        public void HideDropdown(VisualElement element)
        {
            if (activeDropdown != element || activeDropdown == null) return;

            Panel.visualTree.UnregisterCallback<PointerDownEvent>(OnPanelClick, TrickleDown.TrickleDown);
            Panel.visualTree.UnregisterCallback<NavigationCancelEvent>(OnCancelClick, TrickleDown.TrickleDown);
            Panel.visualTree.pickingMode = PickingMode.Ignore;

            activeDropdown.style.position = StyleKeyword.Null;
            activeDropdown.style.transformOrigin = StyleKeyword.Null;
            activeDropdown.style.translate = StyleKeyword.Null;

            activeDropdown.UnregisterCallback<GeometryChangedEvent>(OnDropdownAdded);
            if (activeDropdown is IDropdownElement dropdownElement) dropdownElement.OnDropdownBeforeHide();
            activeDropdown = null;
            activeInitialized = false;
            activeHideCallback?.Invoke();
            activeHideCallback = null;
        }

        private void OnPanelClick(PointerDownEvent evt)
        {
            if (evt.target is VisualElement targetElement && activeInitialized)
            {
                if (targetElement != activeDropdown && !targetElement.IsDescendantOf(activeDropdown))
                {
                    HideDropdown(activeDropdown);
                }
            }
        }

        private void OnCancelClick(NavigationCancelEvent evt)
        {
            if (activeInitialized) HideDropdown(activeDropdown);
        }

        private void AddActiveElement()
        {
            if (activeDropdown == null || activeInitialized) return;
            Panel.visualTree.Add(activeDropdown);
            activeDropdown.RegisterCallback<GeometryChangedEvent>(OnDropdownAdded);
        }

        private void OnDropdownAdded(GeometryChangedEvent e)
        {
            var dropdownSize = activeDropdown.contentRect.size;
            var targetRect = activeTargetRect;
            var panelSize = Panel.visualTree.contentRect.size;

            if (float.IsNaN(dropdownSize.x) || float.IsNaN(targetRect.width))
            {
                HideDropdown(activeDropdown);
                return;
            }

            activeDropdown.style.position = Position.Absolute;
            activeDropdown.style.transformOrigin = new TransformOrigin(Length.Percent(0f), Length.Percent(0f));

            var targetPos = new Vector2(targetRect.xMin, targetRect.yMax);

            targetPos.x = Mathf.Clamp(targetPos.x, PANEL_PADDING, panelSize.x - dropdownSize.x - PANEL_PADDING);

            if (targetPos.y + dropdownSize.y + PANEL_PADDING > panelSize.y)
            {
                targetPos.y = targetRect.y - dropdownSize.y;
                if (targetPos.y < PANEL_PADDING) targetPos.y = PANEL_PADDING;
            }

            activeDropdown.style.translate = (Vector3)Vector3Int.RoundToInt(targetPos);

            if (activeDropdown is IDropdownElement dropdownElement) dropdownElement.OnDropdownAfterShow();

            activeInitialized = true;
        }
    }
}
