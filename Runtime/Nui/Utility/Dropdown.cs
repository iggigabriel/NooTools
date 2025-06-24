using System;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    public abstract class Dropdown
    {
        protected NuiDropdownList List { get; private set; }
        protected List<NuiDropdownList.Item> Items { get; private set; }
        protected VisualElement Target { get; private set; }
        protected bool Showing { get; private set; }

        public event Action OnHide;

        public void Show(VisualElement target)
        {
            if (Showing || target == null || target.panel == null) return;

            Showing = true;
            Target = target;

            List = NuiPool.Rent<NuiDropdownList>();
            List.RegisterCallback<NuiDropdownList.SelectionChanged>(OnSelectionChanged);

            Items = ListPool<NuiDropdownList.Item>.Get();

            GenerateItems(Items);

            List.SetItems(Items);

            if (target.panel.TryGetNuiSystem<NuiDropdownSystem>(out var dropdownSystem))
            {
                dropdownSystem.ShowAsDropdown(List, target.worldBound, Hide);
            }
        }

        void Clear()
        {
            OnClear();

            if (Showing)
            {
                Showing = false;

                if (List != null && Target.panel.TryGetNuiSystem<NuiDropdownSystem>(out var dropdownSystem))
                {
                    dropdownSystem.HideDropdown(List);
                }
            }

            if (List != null)
            {
                List.UnregisterCallback<NuiDropdownList.SelectionChanged>(OnSelectionChanged);
                NuiPool.Return(List);
                List = null;
            }

            if (Items != null)
            {
                foreach (var item in Items) NuiPool.Return(item);
                ListPool<NuiDropdownList.Item>.Release(Items);
                Items = null;
            }

            Target = null;
        }

        public void Hide()
        {
            Clear();
            OnHide?.Invoke();
            OnHide = null;
        }

        protected abstract void GenerateItems(List<NuiDropdownList.Item> items);
        protected virtual void OnSelectionChanged(NuiDropdownList.SelectionChanged e) { }
        protected virtual void OnClear() { }
    }
}
