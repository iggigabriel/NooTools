using UnityEngine.UIElements;

namespace Noo.Nui
{
    public class FocusForwardManipulator : Manipulator
    {
        public VisualElement forwardTarget;

        readonly EventCallback<FocusEvent> callback;

        public FocusForwardManipulator(VisualElement forwardTarget)
        {
            this.forwardTarget = forwardTarget;
            callback = OnFocus;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback(callback);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback(callback);
        }

        private void OnFocus(FocusEvent evt)
        {
            forwardTarget?.Focus();
        }
    }
}
