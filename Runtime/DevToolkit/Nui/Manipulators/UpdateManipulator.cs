using System;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    public class UpdateManipulator : Manipulator
    {
        readonly Action updateAction;
        bool focused;
        public bool updateIfFocused = false;

        public UpdateManipulator(Action updateAction)
        {
            this.updateAction = updateAction;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<FocusEvent>(OnFocus);
            target.RegisterCallback<BlurEvent>(OnBlur);
            NuiTask.OnInterval(NuiTask.ExecutionOrder.LateUpdate, 1f / 12f, true, Update);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<FocusEvent>(OnFocus);
            target.UnregisterCallback<BlurEvent>(OnBlur);
            NuiTask.Cancel(Update);
        }

        private void OnBlur(BlurEvent evt)
        {
            focused = false;
        }

        private void OnFocus(FocusEvent evt)
        {
            focused = true;
        }

        void Update()
        {
            if (!focused || updateIfFocused) updateAction?.Invoke();
        }
    }
}
