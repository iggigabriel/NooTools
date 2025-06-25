using Noo.Nui;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Noo.DevToolkit
{
    public abstract class NuiPropertyDrawerIntVectors<T> : NuiPropertyDrawer<T>
    {
        protected readonly Dictionary<string, int> fieldValues = new();

        struct InnerField
        {
            public NuiText label;
            public IntegerField input;
            public VisualElement parent;
            public VisualElement background;
            public FieldMouseDragger<int> labelDragController;
            public FocusForwardManipulator labelFocusController;
        }

        readonly Dictionary<string, InnerField> innerFields = new();

        protected NuiPropertyDrawerIntVectors(params string[] labels)
        {
            foreach (var label in labels) fieldValues.Add(label, default);
        }

        public override void OnInit()
        {
            base.OnInit();
        }

        protected override void OnPropertyCreate()
        {
            Root.WithClass("dtk-vector-input-drawer");
            Root.WithClass("dtk-property-drawer-lg");
            Root.AddManipulator(UpdateManipulator);

            foreach (var (fieldName, _) in fieldValues)
            {
                var field = new InnerField()
                {
                    label = NuiPool.Rent<NuiText>().WithClass("dtk-vector-field__label"),
                    parent = NuiPool.Rent<VisualElement>().WithClass("dtk-vector-field__container"),
                    input = NuiPool.Rent<IntegerField>().WithClass("dtk-input-field"),
                    background = NuiPool.Rent<VisualElement>().WithClass("dtk-input-field__background")
                };

                field.input.AppendTo(field.parent);
                field.background.PrependTo(field.input);
                field.label.AppendTo(PropertyContainer);
                field.parent.AppendTo(PropertyContainer);

                field.input.isReadOnly = Property.IsReadOnly;
                field.input.RegisterValueChangedCallback(OnChanged);

                field.label.pickingMode = PickingMode.Position;
                field.label.text = fieldName;
                field.label.focusable = true;

                field.labelDragController = new FieldMouseDragger<int>(field.input);
                field.labelDragController.SetDragZone(field.label);

                field.labelFocusController = new FocusForwardManipulator(field.input);
                field.label.AddManipulator(field.labelFocusController);

                innerFields.Add(fieldName, field);
            }

            OnPropertyUpdate();
        }

        protected override void OnPropertyDestroy()
        {
            Root.WithoutClass("dtk-vector-input-drawer");
            Root.WithoutClass("dtk-property-drawer-lg");
            Root.RemoveManipulator(UpdateManipulator);

            foreach (var (fieldName, field) in innerFields)
            {
                field.label.WithoutClass("dtk-vector-field__label");
                field.parent.WithoutClass("dtk-vector-field__container");
                field.input.WithoutClass("dtk-input-field");
                field.background.WithoutClass("dtk-input-field__background");

                field.input.Blur();
                field.input.isReadOnly = false;
                field.input.UnregisterValueChangedCallback(OnChanged);
                field.input.SetValueWithoutNotify(default);

                field.labelDragController?.SetDragZone(null);
                field.label.RemoveManipulator(field.labelFocusController);
                field.label.focusable = false;

                NuiPool.Return(field.label);
                NuiPool.Return(field.input);
                NuiPool.Return(field.background);
                NuiPool.Return(field.parent);
            }

            innerFields.Clear();
        }

        private void OnChanged(ChangeEvent<int> e)
        {
            foreach (var (fieldName, field) in innerFields)
            {
                fieldValues[fieldName] = field.input.value;
            }

            Value = VectorValue;
        }

        protected override void OnPropertyUpdate()
        {
            VectorValue = Value;

            foreach (var (fieldName, field) in innerFields)
            {
                if (fieldValues.TryGetValue(fieldName, out var fieldValue))
                {
                    field.input.SetValueWithoutNotify(fieldValue);
                }
            }
        }

        protected abstract T VectorValue { get; set; }
    }
}
