using UnityEngine.UIElements;

namespace Noo.Nui
{
    public sealed class FocusListenerManipulator : Manipulator
    {
        public string FocusClass { get; private set; }

        public FocusListenerManipulator(string focusClass)
        {
            FocusClass = focusClass;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<FocusEvent>(OnFocus, TrickleDown.TrickleDown);
            target.RegisterCallback<BlurEvent>(OnBlur, TrickleDown.TrickleDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target?.RemoveFromClassList(FocusClass);
            target.UnregisterCallback<FocusEvent>(OnFocus, TrickleDown.TrickleDown);
            target.UnregisterCallback<BlurEvent>(OnBlur, TrickleDown.TrickleDown);
        }

        private void OnBlur(BlurEvent evt)
        {
            if (evt.target is VisualElement targetElement && targetElement.IsDescendantOf(target))
            {
                target?.RemoveFromClassList(FocusClass);
            }
        }

        private void OnFocus(FocusEvent evt)
        {
            if (evt.target is VisualElement targetElement && targetElement.IsDescendantOf(target))
            {
                target?.AddToClassList(FocusClass);
            }
        }
    }
}
