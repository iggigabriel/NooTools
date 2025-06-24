using Noo.Nui;
using UnityEngine.UIElements;

namespace Noo.DevToolkit
{
    public class NuiPropertyDrawerString : NuiPropertyDrawer<string>
    {
        protected TextField field;
        protected VisualElement inputBackground;

        protected override void OnPropertyCreate()
        {
            Root.WithClass("dtk-property-drawer-md");

            field = NuiPool.Rent<TextField>().WithClass("dtk-input-field", "dtk-string-field").AppendTo(PropertyContainer);
            field.value = Value;
            field.isReadOnly = Property.IsReadOnly;
            field.RegisterValueChangedCallback(OnChanged);
            field.AddManipulator(UpdateManipulator);

            inputBackground = NuiPool.Rent<VisualElement>().WithClass("dtk-input-field__background").PrependTo(field);
        }

        protected override void OnPropertyDestroy()
        {
            Root.WithoutClass("dtk-property-drawer-md");

            if (inputBackground != null)
            {
                NuiPool.Return(inputBackground.WithoutClass("dtk-input-field__background"));
                inputBackground = null;
            }

            if (field != null)
            {
                field.RemoveManipulator(UpdateManipulator);
                field.isReadOnly = false;
                field.UnregisterValueChangedCallback(OnChanged);
                NuiPool.Return(field.WithoutClass("dtk-input-field", "dtk-string-field"));
                field = null;
            }
        }

        void OnChanged(ChangeEvent<string> e)
        {
            Property.Value = e.newValue;
        }

        protected override void OnPropertyUpdate()
        {
            field?.SetValueWithoutNotify(Value);
        }

        protected override bool OnPropertyFilter(string query)
        {
            return base.OnPropertyFilter(query);
        }
    }
}
