using Noo.Nui;
using UnityEngine.UIElements;

namespace Noo.DevToolkit
{
    public class NuiPropertyDrawerBool : NuiPropertyDrawer<bool>
    {
        Toggle toggle;
        NuiIconMat toggleIcon;

        protected override void OnPropertyCreate()
        {
            Root.WithClass("dtk-property-drawer-bool");
            if (!Property.IsReadOnly) LabelContainer.RegisterCallback<ClickEvent>(Toggle);

            toggle = NuiPool.Rent<Toggle>().WithClass("dtk-bool-field").AppendTo(PropertyContainer);
            toggle.value = Value;
            toggle.RegisterValueChangedCallback(OnChange);

            var checkmark = toggle.Q("unity-checkmark");
            if (checkmark != null)
            {
                toggleIcon = NuiPool.Rent<NuiIconMat>().WithClass("dtk-bool-field__checkmark").AppendTo(checkmark);
                toggleIcon.Icon = MatIcon.Check;
            }
        }

        protected override void OnPropertyDestroy()
        {
            Root.WithoutClass("dtk-property-drawer-bool");
            LabelContainer.UnregisterCallback<ClickEvent>(Toggle);

            if (toggleIcon != null)
            {
                NuiPool.Return(toggleIcon.WithoutClass("dtk-bool-field__checkmark"));
                toggleIcon = null;
            }

            if (toggle != null)
            {
                toggle.value = false;
                toggle.SetEnabled(true);
                toggle.UnregisterValueChangedCallback(OnChange);
                NuiPool.Return(toggle.WithoutClass("dtk-bool-field"));
            }
        }

        void OnChange(ChangeEvent<bool> e)
        {
            if (Property.IsReadOnly)
            {
                toggle.SetValueWithoutNotify(Value);
            }
            else
            {
                Value = e.newValue;
            }
        }

        void Toggle(ClickEvent e)
        {
            e.StopImmediatePropagation();
            toggle.value = !toggle.value;
        }

        protected override void OnPropertyUpdate()
        {
            toggle?.SetValueWithoutNotify(Value);
        }
    }
}
