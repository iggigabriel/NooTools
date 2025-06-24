using Noo.Nui;
using System;
using UnityEngine.UIElements;

namespace Noo.DevToolkit
{
    public abstract class NuiPropertyDrawerNumericGeneric<T, TInputField> : NuiPropertyDrawer<T>
        where TInputField : TextValueField<T>, new()
        where T : IConvertible
    {
        protected TInputField field;
        protected VisualElement inputBackground;
        protected FieldMouseDragger<T> labelDragController;

        protected override void OnPropertyCreate()
        {
            Root.WithClass("dtk-numeric-input-drawer");

            field = NuiPool.Rent<TInputField>().WithClass("dtk-input-field").AppendTo(PropertyContainer);
            field.value = Value;
            field.isReadOnly = Property.IsReadOnly;
            field.RegisterValueChangedCallback(OnChanged);
            field.AddManipulator(UpdateManipulator);

            inputBackground = NuiPool.Rent<VisualElement>().WithClass("dtk-input-field__background").PrependTo(field);

            if (!Property.IsReadOnly && LabelContainer != null)
            {
                labelDragController = new FieldMouseDragger<T>(field);
                labelDragController.SetDragZone(LabelContainer);
            }
        }

        protected override void OnPropertyDestroy()
        {
            Root.WithoutClass("dtk-numeric-input-drawer");

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

            if (labelDragController != null)
            {
                labelDragController.SetDragZone(null);
                labelDragController = null;
            }
        }

        void OnChanged(ChangeEvent<T> e)
        {
            Property.Value = e.newValue;
        }

        protected override void OnPropertyUpdate()
        {
            field?.SetValueWithoutNotify(Value);
        }
    }

    public abstract class NuiPropertyDrawerNumeric<T> : NuiPropertyDrawer<T> where T : IConvertible
    {
        protected LongField field;
        protected VisualElement inputBackground;
        protected FieldMouseDragger<long> labelDragController;

        protected override void OnPropertyCreate()
        {
            Root.WithClass("dtk-numeric-input-drawer");

            field = NuiPool.Rent<LongField>().WithClass("dtk-input-field").AppendTo(PropertyContainer);
            field.value = ToInt64(Value);
            field.isReadOnly = Property.IsReadOnly;
            field.RegisterValueChangedCallback(OnChanged);
            field.AddManipulator(UpdateManipulator);

            inputBackground = NuiPool.Rent<VisualElement>().WithClass("dtk-input-field__background").PrependTo(field);

            if (LabelContainer != null)
            {
                labelDragController = new FieldMouseDragger<long>(field);
                labelDragController.SetDragZone(LabelContainer);
            }
        }

        protected override void OnPropertyDestroy()
        {
            Root.WithoutClass("dtk-numeric-input-drawer");

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

            if (labelDragController != null)
            {
                labelDragController.SetDragZone(null);
                labelDragController = null;
            }
        }

        void OnChanged(ChangeEvent<long> e)
        {
            var value = FromInt64(e.newValue);
            Property.Value = value;

            var newValue = ToInt64(value);
            if (e.newValue != newValue) field.SetValueWithoutNotify(newValue);
        }

        protected abstract long ToInt64(T value);
        protected abstract T FromInt64(long value);

        protected override void OnPropertyUpdate()
        {
            field?.SetValueWithoutNotify(ToInt64(Value));
        }
    }
}
