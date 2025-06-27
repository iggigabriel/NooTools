using System;
using System.Collections.Generic;

namespace Noo.Nui
{
    public class DropdownSelect<T> : Dropdown
    {
        public IReadOnlyDictionary<T, string> options;
        public T initialValue;
        public event Action<T> OnValueChanged;

        public DropdownSelect(IReadOnlyDictionary<T, string> options, T initialValue, Action<T> valueChangedCallback = null)
        {
            if (valueChangedCallback != null) OnValueChanged += valueChangedCallback;

            this.options = options;
            this.initialValue = initialValue;
        }

        protected override void GenerateItems(List<NuiDropdownList.Item> items)
        {
            if (options == null) return;

            foreach (var option in options)
            {
                var btn = List.GetPooledItem<NuiDropdownListButton>(option.Key, NuiUtility.AreEqual(option.Key, initialValue));
                btn.buttonText = option.Value;
                items.Add(btn);
            }
        }

        protected override void OnSelectionChanged(NuiDropdownList.SelectionChanged e)
        {
            if (e.SelectedItem != null && e.SelectedItem.Value is T tValue && !NuiUtility.AreEqual(tValue, initialValue))
            {
                OnValueChanged?.Invoke(tValue);
            }

            Hide();
        }

        protected override void OnClear()
        {
            base.OnClear();

            options = default;
            initialValue = default;
            OnValueChanged = null;
        }
    }
}
