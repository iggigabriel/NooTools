using Noo.Nui;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Noo.DevToolkit
{
    public class NuiAttributeDrawerSlider : NuiAttributeDrawer<DevSliderAttribute>
    {
        Slider sliderFloat;
        SliderInt sliderInt;

        static readonly HashSet<Type> intTypes = new() { typeof(int), typeof(long), typeof(short), typeof(byte), typeof(uint), typeof(ulong), typeof(ushort), typeof(sbyte) };
        static readonly HashSet<Type> floatTypes = new() { typeof(float), typeof(double), typeof(decimal) };

        bool IsInt => intTypes.Contains(Property.PropertyType);
        bool IsFloat => floatTypes.Contains(Property.PropertyType);

        public override void OnInit()
        {
            base.OnInit();
            UpdateManipulator.updateIfFocused = true;
        }

        protected override void OnPropertyCreate()
        {
            Root.WithClass("dtk-property-drawer-md");
            Root.WithClass("dtk-property-drawer-slider");
            Root.AddManipulator(UpdateManipulator);

            if (IsInt)
            {
                sliderInt = NuiPool.Rent<SliderInt>();
                sliderInt.WithClass("dtk-slider-field");
                sliderInt.AppendTo(PropertyContainer);
                sliderInt.lowValue = Convert.ToInt32(DrawerAttribute.Min);
                sliderInt.highValue = Convert.ToInt32(DrawerAttribute.Max);
                sliderInt.RegisterValueChangedCallback(OnChange);
            }

            if (IsFloat)
            {
                sliderFloat = NuiPool.Rent<Slider>();
                sliderFloat.WithClass("dtk-slider-field");
                sliderFloat.AppendTo(PropertyContainer);
                sliderFloat.lowValue = DrawerAttribute.Min;
                sliderFloat.highValue = DrawerAttribute.Max;
                sliderFloat.RegisterValueChangedCallback(OnChange);
            }

            UpdateSliderValue();
            UpdateSliderLabel();
        }

        private void OnChange(ChangeEvent<float> e)
        {
            if (Property.IsReadOnly)
            {
                sliderFloat.SetValueWithoutNotify(Convert.ToSingle(Property.Value));
            }
            else
            {
                Property.Value = Convert.ChangeType(e.newValue, Property.PropertyType);
                UpdateSliderLabel();
            }
        }

        private void OnChange(ChangeEvent<int> e)
        {
            if (Property.IsReadOnly)
            {
                sliderInt.SetValueWithoutNotify(Convert.ToInt32(Property.Value));
            }
            else
            {
                Property.Value = Convert.ChangeType(e.newValue, Property.PropertyType);
                UpdateSliderLabel();
            }
        }

        protected override void OnPropertyDestroy()
        {
            lastSetValue = null;

            Root.WithoutClass("dtk-property-drawer-md");
            Root.WithoutClass("dtk-property-drawer-slider");
            Root.RemoveManipulator(UpdateManipulator);

            if (sliderFloat != null)
            {
                sliderFloat.label = null;
                sliderFloat.UnregisterValueChangedCallback(OnChange);
                sliderFloat.WithoutClass("dtk-slider-field");
                NuiPool.Return(sliderFloat);
                sliderFloat = null;
            }

            if (sliderInt != null)
            {
                sliderInt.label = null;
                sliderInt.UnregisterValueChangedCallback(OnChange);
                sliderInt.WithoutClass("dtk-slider-field");
                NuiPool.Return(sliderInt);
                sliderInt = null;
            }
        }

        protected override void OnPropertyUpdate()
        {
            base.OnPropertyUpdate();
            UpdateSliderValue();
            UpdateSliderLabel();
        }

        private void UpdateSliderValue()
        {
            sliderInt?.SetValueWithoutNotify(Property.Value == null ? sliderInt.lowValue : Convert.ToInt32(Property.Value));
            sliderFloat?.SetValueWithoutNotify(Property.Value == null ? sliderFloat.lowValue : Convert.ToSingle(Property.Value));
        }

        object lastSetValue;

        private void UpdateSliderLabel()
        {
            if (lastSetValue != null && lastSetValue == Property.Value) return;

            lastSetValue = Property.Value;

            if (sliderInt != null)
            {
                sliderInt.label = Convert.ToInt32(lastSetValue).ToString();
            }

            if (sliderFloat != null)
            {
                sliderFloat.label = Convert.ToSingle(lastSetValue).ToString("0.00");
            }
        }
    }
}
