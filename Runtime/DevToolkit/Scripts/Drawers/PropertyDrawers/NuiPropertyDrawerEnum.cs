using Noo.Nui;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Noo.DevToolkit
{
    public class NuiPropertyDrawerEnum : NuiPropertyDrawer<Enum>
    {
        NuiButton btn;
        bool flags;

        int? lastSetValue;

        List<int> dropdownValues = new();
        DropdownSelect<int> dropdownSelect;
        DropdownMultiSelect<int> dropdownMultiSelect;
        Dictionary<int, string> nameValuePairs = new();

        public override void OnInit()
        {
            base.OnInit();
            flags = Property.PropertyType.IsDefined(typeof(FlagsAttribute), true);
            UpdateManipulator.updateIfFocused = true;

            if (flags) nameValuePairs.Add(0, "None");

            foreach (var item in EnumUtility.GetValueNamePairs(Property.PropertyType))
            {
                nameValuePairs.TryAdd(item.Key, item.Value);
            }
        }

        protected override void OnPropertyCreate()
        {
            lastSetValue = default;

            Root.WithClass("dtk-property-drawer-md");
            Root.WithClass("dtk-property-drawer-enum");
            Root.AddManipulator(UpdateManipulator);

            btn = NuiPool.Rent<NuiButton>().WithClass("nui-btn-light-black", "flex-grow").AppendTo(PropertyContainer);
            btn.IconRight = MatIcon.ArrowDropDown;
            btn.clicked += ShowDropdown;

            UpdateEnumButtonValue();
        }

        protected override void OnPropertyDestroy()
        {
            lastSetValue = default;

            Root.WithoutClass("dtk-property-drawer-md");
            Root.WithoutClass("dtk-property-drawer-enum");
            Root.RemoveManipulator(UpdateManipulator);

            if (btn != null)
            {
                btn.WithoutClass("nui-btn-light-black", "flex-grow").RemoveFromHierarchy();
                btn.clicked -= ShowDropdown;
                NuiPool.Return(btn);
                btn = null;
            }
        }

        protected override void OnPropertyUpdate()
        {
            base.OnPropertyUpdate();

            UpdateEnumButtonValue();
        }

        void UpdateEnumButtonValue()
        {
            var enumValue = Convert.ToInt32(Value);

            if (lastSetValue.HasValue && lastSetValue.Value == enumValue) return;

            lastSetValue = enumValue;

            var valueNamePairs = EnumUtility.GetValueNamePairs(Property.PropertyType);

            if (flags)
            {
                if (enumValue == default)
                {
                    btn.ButtonText = "None";
                }
                else
                {
                    btn.ButtonText = Value.ToString();
                }
            }
            else
            {
                if (valueNamePairs.TryGetValue(enumValue, out var name))
                {
                    btn.ButtonText = name;
                }
                else
                {
                    btn.ButtonText = "-";
                }
            }
        }

        private void ShowDropdown()
        {
            if (flags)
            {
                var values = EnumUtility.GetValues(Property.PropertyType);
                dropdownValues.Clear();
                var currentValue = Convert.ToInt32(Value);
                if (currentValue == 0) dropdownValues.Add(0);
                foreach (var value in values) if (value > 0 && (value & currentValue) == value) dropdownValues.Add(value);

                dropdownMultiSelect = new DropdownMultiSelect<int>(nameValuePairs, dropdownValues, DropdownValueChanged);
                dropdownMultiSelect.Show(btn);
            }
            else
            {
                dropdownSelect = new DropdownSelect<int>(nameValuePairs, Convert.ToInt32(Value), DropdownValueChanged);
                dropdownSelect.Show(btn);
            }
        }

        private void DropdownValueChanged(IReadOnlyList<int> values)
        {
            if (!flags) return;

            var intValue = 0;

            if (lastSetValue.HasValue && lastSetValue.Value != 0 && values.Contains(0))
            {
                intValue = 0;
            }
            else
            {
                for (int i = 0; i < values.Count; i++)
                {
                    intValue |= values[i];
                }
            }

            var enumValues = EnumUtility.GetValues(Property.PropertyType);
            dropdownValues.Clear();
            if (intValue == 0) dropdownValues.Add(0);
            foreach (var value in enumValues) if (value > 0 && (value & intValue) == value) dropdownValues.Add(value);

            if (intValue == 0)
            {
                dropdownMultiSelect.UpdateSelectedItemsWithoutNotify(dropdownValues, true);
            }
            else if (lastSetValue.HasValue && lastSetValue.Value == 0 && intValue != 0 && values.Contains(0))
            {
                dropdownMultiSelect.UpdateSelectedItemsWithoutNotify(dropdownValues, true);
            }
            else
            {
                dropdownMultiSelect.UpdateSelectedItemsWithoutNotify(dropdownValues, false);
            }

            try
            {
                if (intValue == 0)
                {
                    Value = default;
                }
                else if (Enum.ToObject(Property.PropertyType, intValue) is Enum enumValue)
                {
                    Value = enumValue;
                }
            }
            catch (Exception)
            {

            }

            UpdateEnumButtonValue();
        }

        private void DropdownValueChanged(int value)
        {
            if (flags) return;

            if (Enum.ToObject(Property.PropertyType, value) is Enum enumValue)
            {
                Value = enumValue;
            }
        }
    }
}
