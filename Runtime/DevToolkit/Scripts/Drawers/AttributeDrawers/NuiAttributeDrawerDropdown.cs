using Noo.Nui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Noo.DevToolkit
{
    public class NuiAttributeDrawerDropdown : NuiAttributeDrawer<DevDropdownAttribute>
    {
        NuiButton btn;

        object lastSetValue;

        DropdownSelect<object> dropdownSelect;
        readonly Dictionary<object, string> nameValuePairs = new();

        MemberInfo dataSourceMember;
        Exception dataSourceException;

        public override void OnInit()
        {
            base.OnInit();
            UpdateManipulator.updateIfFocused = true;

            var memberPath = DrawerAttribute.Items != null && DrawerAttribute.Items.Contains('.') ? DrawerAttribute.Items : $"{Property.DeclaringType.FullName}.{DrawerAttribute.Items}";

            try
            {
                dataSourceMember = DevToolkitUtility.ParseMemberInfo(memberPath);
            }
            catch (Exception e)
            {
                dataSourceException = e;
            }
        }

        protected override void OnPropertyCreate()
        {
            lastSetValue = default;

            Root.WithClass("dtk-property-drawer-md");
            Root.WithClass("dtk-property-drawer-dropdown");
            Root.AddManipulator(UpdateManipulator);

            btn = NuiPool.Rent<NuiButton>().WithClass("nui-btn-light-black", "flex-grow").AppendTo(PropertyContainer);
            btn.IconRight = MatIcon.ArrowDropDown;
            btn.clicked += ShowDropdown;

            UpdateNameValuePairs();
            UpdateButtonValue();
        }

        protected override void OnPropertyDestroy()
        {
            lastSetValue = default;

            Root.WithoutClass("dtk-property-drawer-md");
            Root.WithoutClass("dtk-property-drawer-dropdown");
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

            UpdateButtonValue();
        }

        void UpdateButtonValue()
        {
            if (lastSetValue != null && lastSetValue == Property.Value) return;

            lastSetValue = Property.Value;

            if (Property.Value != null && nameValuePairs.TryGetValue(Property.Value, out var valueName))
            {
                btn.ButtonText = valueName;
            }
            else
            {
                btn.ButtonText = " ";
            }
        }

        private void UpdateNameValuePairs()
        {
            nameValuePairs.Clear();

            if (dataSourceException != null)
            {
                Debug.LogError($"Dropdown data source invalid. ({dataSourceException.Message})");
                return;
            }

            if (dataSourceMember == null) return;

            if (!dataSourceMember.IsStatic())
            {
                Debug.LogError($"Dropdown data source must be static.");
                return;
            }

            object items = null;

            if (dataSourceMember is MethodInfo methodInfo)
            {
                try
                {
                    items = methodInfo.Invoke(null, Array.Empty<object>());
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            else if (!dataSourceMember.IsMemberReadable())
            {
                Debug.LogError($"Dropdown data source is not readable.");
                return;
            }
            else
            {
                try
                {
                    items = dataSourceMember.GetMemberValue(null);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }

            if (items is IEnumerable<(object, string)> objectTuples)
            {
                foreach (var (o, name) in objectTuples)
                {
                    if (o != null) nameValuePairs[o] = name;
                }
            }
            else if (items is IReadOnlyDictionary<object, string> objectDict)
            {
                foreach (var (o, name) in objectDict)
                {
                    if (o != null) nameValuePairs[o] = name;
                }
            }
            else if (items is IEnumerable objects)
            {
                foreach (var o in objects) if (o != null) nameValuePairs[o] = o.ToString();
            }
            else
            {
                Debug.LogError($"Dropdown data source must return `IEnumerable`, `IEnumerable<(object, string)>` or `IReadOnlyDictionary<object, string>` collection.");
                return;
            }
        }

        private void ShowDropdown()
        {
            UpdateNameValuePairs();
            dropdownSelect = new DropdownSelect<object>(nameValuePairs, Property.Value, DropdownValueChanged);
            dropdownSelect.Show(btn);
        }

        private void DropdownValueChanged(object value)
        {
            Property.Value = value;
        }
    }
}
