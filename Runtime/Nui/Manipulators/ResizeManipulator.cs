using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    public class ResizeManipulator : PointerManipulator
    {
        private readonly VisualElement resizeTarget;
        private Vector2 minTargetSize;
        private Vector2 startTargetSize;

        private Vector2 startPointerPosition;
        protected bool isActive;
        private int pointerId;
        private readonly Action onResize;

        public ResizeManipulator(VisualElement resizeTarget, Action onResize = null)
        {
            this.resizeTarget = resizeTarget;
            this.onResize = onResize;

            pointerId = -1;
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            isActive = false;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }

        protected void OnPointerDown(PointerDownEvent e)
        {
            if (isActive)
            {
                e.StopImmediatePropagation();
                return;
            }

            if (resizeTarget != null && CanStartManipulation(e))
            {
                minTargetSize = new Vector2(resizeTarget.resolvedStyle.minWidth.value, resizeTarget.resolvedStyle.minHeight.value);
                startTargetSize = new Vector2(resizeTarget.resolvedStyle.width, resizeTarget.resolvedStyle.height);
                startPointerPosition = e.position;
                pointerId = e.pointerId;

                isActive = true;
                target.CapturePointer(pointerId);
                target.AddToClassList("resize-handle--active");
                e.StopPropagation();
            }
        }

        protected void OnPointerMove(PointerMoveEvent e)
        {
            if (!isActive || !target.HasPointerCapture(pointerId))
                return;

            var diff = (Vector2)e.position - startPointerPosition;
            var size = Vector2.Max(startTargetSize + diff, minTargetSize);

            resizeTarget.style.width = size.x;
            resizeTarget.style.height = size.y;

            e.StopPropagation();
        }

        protected void OnPointerUp(PointerUpEvent e)
        {
            if (!isActive || !target.HasPointerCapture(pointerId) || !CanStopManipulation(e))
                return;

            isActive = false;
            target.ReleaseMouse();
            target.RemoveFromClassList("resize-handle--active");
            e.StopPropagation();

            onResize?.Invoke();
        }
    }
}
