using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Pool;

namespace Noo.Nui
{
    public class DropdownMultiSelect<T> : Dropdown
    {
        public IReadOnlyDictionary<T, string> options;
        public IReadOnlyList<T> initialValue;
        public event Action<IReadOnlyList<T>> OnValueChanged;

        public DropdownMultiSelect(IReadOnlyDictionary<T, string> options, IReadOnlyList<T> initialValue, Action<IReadOnlyList<T>> valueChangedCallback = null)
        {
            if (valueChangedCallback != null) OnValueChanged += valueChangedCallback;

            this.options = options;
            this.initialValue = initialValue;
        }

        protected override void GenerateItems(List<NuiDropdownList.Item> items)
        {
            if (options == null) return;

            List.Multiselect = true;

            foreach (var option in options)
            {
                var selected = initialValue != null && initialValue.Contains(option.Key);

                var btn = List.GetPooledItem<NuiDropdownListButton>(option.Key, selected);
                btn.buttonText = option.Value;
                items.Add(btn);
            }
        }

        protected override void OnSelectionChanged(NuiDropdownList.SelectionChanged e)
        {
            using var _ = ListPool<T>.Get(out var valueList);

            if (e.SelectedItems != null)
            {
                foreach (var item in e.SelectedItems)
                {
                    if (item != null && item.Value is T tValue) valueList.Add(tValue);
                }
            }

            if (!NuiUtility.AreEqual(valueList, initialValue))
            {
                OnValueChanged?.Invoke(valueList);
            }
        }

        public void UpdateSelectedItemsWithoutNotify(IReadOnlyList<T> values, bool updateListSelection)
        {
            foreach (var item in Items)
            {
                var selected = values != null && item.Value is T tValue && values.Contains(tValue);
                item.SetSelectedWithoutNotify(selected);
            }
            if (updateListSelection) List?.RefreshSelection();
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
