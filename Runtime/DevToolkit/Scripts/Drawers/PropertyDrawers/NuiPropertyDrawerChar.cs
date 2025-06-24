using Noo.Nui;
using UnityEngine.UIElements;

namespace Noo.DevToolkit
{
    public class NuiPropertyDrawerChar : NuiPropertyDrawer<char>
    {
        protected TextField field;
        protected VisualElement inputBackground;

        protected override void OnPropertyCreate()
        {
            field = NuiPool.Rent<TextField>().WithClass("dtk-input-field").AppendTo(PropertyContainer);
            field.value = Value.ToString();
            field.isReadOnly = Property.IsReadOnly;
            field.RegisterValueChangedCallback(OnChanged);
            field.AddManipulator(UpdateManipulator);

            inputBackground = NuiPool.Rent<VisualElement>().WithClass("dtk-input-field__background").PrependTo(field);
        }

        protected override void OnPropertyDestroy()
        {
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
                NuiPool.Return(field.WithoutClass("dtk-input-field"));
                field = null;
            }
        }

        void OnChanged(ChangeEvent<string> e)
        {
            var charValue = e.newValue.Length > 0 ? e.newValue[0] : default;
            Property.Value = charValue;
            if (charValue.ToString() != e.newValue) field.SetValueWithoutNotify(charValue.ToString());
        }

        protected override void OnPropertyUpdate()
        {
            field?.SetValueWithoutNotify(Value.ToString());
        }
    }
}
