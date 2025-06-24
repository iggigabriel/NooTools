using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    public class NuiListView<T> : ListView
    {
        readonly List<T> items = new();
        readonly List<T> filteredItems = new();
        readonly List<T>[] filteredItemsSorted = new List<T>[] { new(), new(), new() };

        public readonly ScrollView scrollView;

        public delegate VisualElement CreateItemHandler(T data, int index);
        public delegate void DestroyItemHandler(T data, VisualElement element);
        public delegate bool FilterItemHandler(T item, string query);

        public CreateItemHandler createItem;
        public new DestroyItemHandler destroyItem;
        public FilterItemHandler filterItem;

        public IReadOnlyList<T> Items => items;
        public IReadOnlyList<T> FilteredItems => filteredItems;

        public bool itemsDraggable;

        string[] searchQueries;

        public string[] SearchQueries
        {
            get => searchQueries;
            set
            {
                if (value == searchQueries) return;
                searchQueries = value;
                FilterItems();
            }
        }

        public NuiListView() : base()
        {
            AddToClassList("nui-listview");
            scrollView = hierarchy[0] as ScrollView;
            NuiScrollView.InitializeScrollView(scrollView);

            reorderable = false;
            canStartDrag += StopDrag;

            makeItem = MakeItem;
            destroyItem = DestroyItem;
            bindItem = BindItem;
            unbindItem = UnbindItem;
        }

        public NuiListView(CreateItemHandler createItem, DestroyItemHandler destroyItem, FilterItemHandler filterItem = null, int? itemHeight = null) : this()
        {
            this.createItem = createItem;
            this.destroyItem = destroyItem;
            this.filterItem = filterItem;

            if (itemHeight.HasValue)
            {
                virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
                fixedItemHeight = itemHeight.Value;
            }
            else
            {
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            }

            selectionType = SelectionType.None;
        }

        public NuiListView(IEnumerable<T> items, CreateItemHandler createItem, DestroyItemHandler destroyItem, FilterItemHandler filterItem = null, int? itemHeight = null) : this(createItem, destroyItem, filterItem, itemHeight)
        {
            AddItems(items);
        }

        static VisualElement MakeItem()
        {
            var e = NuiPool.Rent<VisualElement>();
            e.AddToClassList("nui-listview__item");
            return e;
        }

        private void DestroyItem(T data, VisualElement element)
        {
            Assert.IsTrue(element.childCount == 0);
            element.ClearClassList();
            NuiPool.Return(element);
        }

        void BindItem(VisualElement element, int index)
        {
            if (createItem == null || index >= filteredItems.Count) return;
            element.userData = filteredItems[index];
            var resolvedItem = createItem(filteredItems[index], index);
            if (resolvedItem != null)
            {
                element.Add(resolvedItem);
            }
        }

        void UnbindItem(VisualElement element, int index)
        {
            var child = element.hierarchy.Children().FirstOrDefault();
            child?.RemoveFromHierarchy();

            if (element.userData is T item)
            {
                destroyItem?.Invoke(item, child);
                element.userData = null;
            }
        }

        public T GetItem(int index) => items[index];

        public void SetItems(IEnumerable<T> items)
        {
            this.items.Clear();
            if (items != null) this.items.AddRange(items);
            FilterItems();
        }

        public void AddItems(IEnumerable<T> items)
        {
            if (items != null) this.items.AddRange(items);
            FilterItems();
        }

        public void AddItem(T item)
        {
            items.Add(item);
            FilterItems();
        }

        public void RemoveItem(T item)
        {
            items.Remove(item);
            FilterItems();
        }

        public void ClearItems()
        {
            items.Clear();
            FilterItems();
        }

        public int IndexOf(T item) => items.IndexOf(item);

        bool StopDrag(CanStartDragArgs args)
        {
            return itemsDraggable;
        }

        private void FilterItems()
        {
            if (itemsSource != filteredItems) itemsSource = filteredItems;

            filteredItems.Clear();

            Rebuild();

            for (int i = 0; i < filteredItemsSorted.Length; i++) filteredItemsSorted[i].Clear();

            if (filterItem == null || searchQueries == null || searchQueries.Length == 0)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    filteredItems.Add(items[i]);
                }
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var score = 0;

                    for (int j = 0; j < searchQueries.Length; j++)
                    {
                        if (filterItem(item, searchQueries[j]))
                        {
                            if (++score > 2) break;
                        }
                    }

                    if (score == 1) filteredItemsSorted[0].Add(items[i]);
                    else if (score == 2) filteredItemsSorted[1].Add(items[i]);
                    else if (score > 2) filteredItemsSorted[2].Add(items[i]);
                }

                for (int i = 0; i < filteredItemsSorted[2].Count; i++)
                {
                    filteredItems.Add(filteredItemsSorted[2][i]);
                }

                for (int i = 0; i < filteredItemsSorted[1].Count; i++)
                {
                    filteredItems.Add(filteredItemsSorted[1][i]);
                }

                for (int i = 0; i < filteredItemsSorted[0].Count; i++)
                {
                    filteredItems.Add(filteredItemsSorted[0][i]);
                }
            }

            RefreshItems();
        }

        public void ScrollToStart()
        {
            scrollView.scrollOffset = new UnityEngine.Vector2(0f, 0.001f);
            scrollView.verticalScroller.value = 0.001f;
        }
    }
}
