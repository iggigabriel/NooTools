using System;
using System.Collections.Generic;
using UnityEngine;

namespace Noo.Tools
{
    public static class CollectionExtensions
    {
        public static void RemoveAndPushBack<T>(this IList<T> list, int index)
        {
            if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException(nameof(index));
            var lastIndex = list.Count - 1;
            list[index] = list[lastIndex];
            list.RemoveAt(lastIndex);
        }

        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
        {
            if (!dictionary.TryGetValue(key, out var value))
            {
                value = new TValue();
                dictionary.Add(key, value);
            }

            return value;
        }

        public static void RemoveIfExists<TKey, TValue>(this SortedList<TKey, TValue> list, TValue value)
        {
            var index = list.IndexOfValue(value);
            if (index != -1) list.RemoveAt(index);
        }
    }
}
