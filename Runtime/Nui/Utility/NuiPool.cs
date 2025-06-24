using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    public interface INuiPoolable
    {
        void OnRentFromPool();
        void OnReturnToPool();
    }

    public static class NuiPool
    {
        static readonly Dictionary<Type, List<object>> pools = new();

        public static T Rent<T>() where T : class, new()
        {
            T item = default;

            if (pools.TryGetValue(typeof(T), out var pool) && pool.Count > 0)
            {
                var index = pool.Count - 1;
                item = pool[index] as T;
                pool.RemoveAt(index);
            }

            item ??= new T();

            if (item is INuiPoolable poolable)
            {
                try { poolable.OnRentFromPool(); }
                catch (Exception e) { NuiUtility.LogException(e); }
            }

            return item;
        }

        public static void Return<T>(T item) where T : class
        {
            if (item == null) return;

            var itemType = item.GetType();

            if (!pools.TryGetValue(itemType, out var pool))
            {
                pools[itemType] = pool = new List<object>();
            }

            if (item is VisualElement visualElement)
            {
                visualElement.RemoveFromHierarchy();
                visualElement.name = default;
                visualElement.userData = default;
                visualElement.style.width = StyleKeyword.Null;
                visualElement.style.height = StyleKeyword.Null;
                visualElement.style.translate = StyleKeyword.Null;
            }

            if (item is INuiPoolable poolable)
            {
                try { poolable.OnReturnToPool(); }
                catch (Exception e) { NuiUtility.LogException(e); }
            }

            pool.Add(item);
        }
    }
}
