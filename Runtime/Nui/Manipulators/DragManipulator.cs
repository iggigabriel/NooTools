using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    public class DragManipulator : PointerManipulator
    {
        private readonly VisualElement dragTarget;
        private Vector2 startTargetPosition;

        private Vector2 startPointerPosition;
        protected bool isActive;
        private int pointerId;
        private readonly Action onDrag;

        public DragManipulator(VisualElement dragTarget, Action onDrag = null)
        {
            this.dragTarget = dragTarget;
            this.onDrag = onDrag;

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

            if (dragTarget != null && CanStartManipulation(e))
            {
                startTargetPosition = new Vector2(dragTarget.resolvedStyle.left, dragTarget.resolvedStyle.top);
                startPointerPosition = e.position;
                pointerId = e.pointerId;

                isActive = true;
                target.CapturePointer(pointerId);
                dragTarget.AddToClassList("dragging");
                e.StopPropagation();
            }
        }

        protected void OnPointerMove(PointerMoveEvent e)
        {
            if (!isActive || !target.HasPointerCapture(pointerId))
                return;

            var diff = (Vector2)e.position - startPointerPosition;

            dragTarget.style.left = startTargetPosition.x + diff.x;
            dragTarget.style.top = startTargetPosition.y + diff.y;
            e.StopPropagation();
        }

        protected void OnPointerUp(PointerUpEvent e)
        {
            if (!isActive || !target.HasPointerCapture(pointerId) || !CanStopManipulation(e))
                return;

            isActive = false;
            target.ReleaseMouse();
            dragTarget.RemoveFromClassList("dragging");
            e.StopPropagation();

            onDrag?.Invoke();
        }
    }
}
