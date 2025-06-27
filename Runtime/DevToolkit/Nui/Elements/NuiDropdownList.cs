using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    public class NuiDropdownList : VisualElement, INuiPoolable, IDropdownElement
    {
        public abstract class Item : INuiPoolable
        {
            public NuiDropdownList List { get; internal set; }
            public object Value { get; set; }
            public VisualElement Root { get; private set; }

            private bool selected;

            public bool Selected
            {
                get => selected;
                set
                {
                    var changed = selected != value;
                    SetSelectedWithoutNotify(value);
                    if (changed) List?.ItemSelectionChanged(this);
                }
            }

            public virtual Focusable Focusable => null;

            public virtual void OnRentFromPool() { }

            public virtual void OnReturnToPool()
            {
                Destroy();
                List = default;
                Value = default;
                selected = default;
            }

            protected virtual void OnCreate() { }
            protected virtual void OnDestroy() { }

            public virtual bool Filter(string query) => true;

            internal void Create()
            {
                if (Root != null) return;
                Root = NuiPool.Rent<VisualElement>().WithClass("nui-dropdown-list__item");
                if (selected) Root.WithClass("nui--selected");
                OnCreate();
            }

            internal void Destroy()
            {
                if (Root == null) return;
                OnDestroy();
                if (Root.childCount > 0) throw new Exception($"Item ({GetType().Name}) was not properly destroyed, {Root.childCount} child remaining.");
                NuiPool.Return(Root.WithoutClass("nui-dropdown-list__item", "nui--selected"));
                Root = null;
            }

            public void SetSelectedWithoutNotify(bool isSelected)
            {
                selected = isSelected;
                Root?.EnableInClassList("nui--selected", isSelected);
            }
        }

        public class SelectionChanged : EventBase<SelectionChanged>
        {
            public IEnumerable<Item> SelectedItems { get; internal set; }
            public Item SelectedItem { get; internal set; }

            protected override void Init()
            {
                base.Init();

                bubbles = true;
                tricklesDown = false;
                SelectedItems = null;
                SelectedItem = null;
            }
        }

        public bool Multiselect
        {
            get => ClassListContains("nui--dropdown-list-multiselect");
            set => EnableInClassList("nui--dropdown-list-multiselect", value);
        }

        readonly VisualElement backgroundShadow;
        readonly VisualElement window;
        readonly NuiListView<Item> list;
        readonly NuiSearchField searchField;
        readonly HashSet<Item> selectedItems = new();

        public IEnumerable<Item> SelectedItems => selectedItems;
        public Item SelectedItem => selectedItems.FirstOrDefault();

        public NuiDropdownList() : base()
        {
            AddToClassList("nui-dropdown-list");

            backgroundShadow = new VisualElement().WithClass("nui-dropdown-list__shadow").AppendToHierarchy(this);
            backgroundShadow.pickingMode = PickingMode.Ignore;

            window = new VisualElement().WithClass("nui-dropdown-list__window").AppendTo(this);

            usageHints = UsageHints.DynamicTransform | UsageHints.DynamicColor;

            searchField = new NuiSearchField().WithClass("nui--sm").AppendTo(window);
            searchField.RegisterValueChangedCallback((e) => list.SearchQueries = NuiUtility.ParseSearchQuery(e.newValue));

            list = new NuiListView<Item>
            (
                (x, i) => { x.Create(); return x.Root; },
                (x, el) => x.Destroy(),
                (x, q) => x.Filter(q),
                30
            );

            list.WithClass("nui-dropdown-list__list").AppendTo(window);
            list.selectionType = SelectionType.None;

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        public T GetPooledItem<T>(object value = null, bool selected = false) where T : Item, new()
        {
            var item = NuiPool.Rent<T>();
            item.List = this;
            item.Value = value;
            item.SetSelectedWithoutNotify(selected);
            return item;
        }

        public void OnRentFromPool()
        {
        }

        public void OnReturnToPool()
        {
            style.width = StyleKeyword.Null;
            style.minWidth = StyleKeyword.Null;
            selectedItems.Clear();
            searchField.SetValueWithoutNotify(null);
            searchField.style.display = StyleKeyword.Null;
            list.SetItems(null);
            list.SearchQueries = null;
            Multiselect = false;
        }

        private void OnGeometryChanged(GeometryChangedEvent e)
        {
            if (style.width.keyword == StyleKeyword.Undefined) return;

            if (style.minWidth.keyword == StyleKeyword.Null)
            {
                style.minWidth = new Length(resolvedStyle.minWidth.value, LengthUnit.Pixel);
            }
            else
            {
                var targetWidth = Math.Clamp(e.newRect.width, resolvedStyle.minWidth.value, resolvedStyle.maxWidth.value);
                style.minWidth = new Length(targetWidth, LengthUnit.Pixel);
            }
        }

        public void SetItems(IReadOnlyList<Item> items)
        {
            if (items == null || items.Count == 0) return;

            selectedItems.Clear();

            list.SetItems(items);

            var listScrolled = false;

            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];

                if (item.Selected)
                {
                    if (!listScrolled)
                    {
                        list.ScrollToItem(i);
                        listScrolled = true;
                    }

                    if (!Multiselect && selectedItems.Count > 0)
                    {
                        item.SetSelectedWithoutNotify(false);
                    }
                    else
                    {
                        selectedItems.Add(items[i]);
                    }
                }
            }

            var showSearchfield = items.Count > 9;
            searchField.style.display = showSearchfield ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void RefreshSelection()
        {
            selectedItems.Clear();
            foreach (var item in list.Items) if (item.Selected) selectedItems.Add(item);
        }

        private void ItemSelectionChanged(Item item)
        {
            if (item.Selected && !Multiselect)
            {
                foreach (var i in selectedItems)
                {
                    if (i == item) continue;
                    i.SetSelectedWithoutNotify(false);
                }

                selectedItems.Clear();
            }

            if (item.Selected) selectedItems.Add(item);
            else selectedItems.Remove(item);

            using var e = SelectionChanged.GetPooled();
            e.target = this;
            e.SelectedItems = SelectedItems;
            e.SelectedItem = SelectedItem;
            panel.visualTree.SendEvent(e);
        }

        public void OnDropdownAfterShow()
        {
            this.GetFirstFocusableChild()?.Focus();
        }

        public void OnDropdownBeforeHide()
        {
        }
    }
}
