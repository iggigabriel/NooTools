using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace Noo.Tools
{
    public static class CollectionExtensions
    {
        public static bool TryAddOnce<T>(this IList<T> list, T item)
        {
            if (list.Contains(item)) return false;
            list.Add(item);
            return true;
        }

        public static void AddOnce<T>(this IList<T> list, T item)
        {
            if (list.Contains(item)) return;
            list.Add(item);
        }

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

        public static T[,] ResizeAndCopy<T>(this T[,] original, int columns, int rows)
        {
            var newArray = new T[columns, rows];
            int minRows = Math.Min(columns, original.GetLength(0));
            int minCols = Math.Min(rows, original.GetLength(1));
            for (int i = 0; i < minRows; i++)
                for (int j = 0; j < minCols; j++)
                    newArray[i, j] = original[i, j];
            return newArray;
        }

        public static void FilterContent<T, TPredicate>(this List<T> list, TPredicate predicate) where TPredicate : struct, IListPredicate<T>
        {
            using var tempList = TempList<T>.Empty();
            for (int i = 0; i < list.Count; i++) if (predicate.Test(list[i])) tempList.Add(list[i]);
            list.Clear();
            for (int i = 0; i < tempList.Count; i++) list.Add(tempList[i]);
        }

        public static void FilterContent<T, TPredicate>(this TempList<T> list, TPredicate predicate) where TPredicate : struct, IListPredicate<T>
        {
            using var tempList = TempList<T>.Empty();
            for (int i = 0; i < list.Count; i++) if (predicate.Test(list[i])) tempList.Add(list[i]);
            list.Clear();
            for (int i = 0; i < tempList.Count; i++) list.Add(tempList[i]);
        }

        public static bool TryFind<T, TPredicate>(this IList<T> list, TPredicate predicate, out T item) where TPredicate : struct, IListPredicate<T>
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate.Test(list[i]))
                {
                    item = list[i];
                    return true;
                }
            }

            item = default;
            return false;
        }

        public static void AddRangeNonAlloc<T>(this IList<T> list, IReadOnlyList<T> values)
        {
            if (values == null) return;
            for (int i = 0; i < values.Count; i++) list.Add(values[i]);
        }

        public static void RemoveWhere<TKey, TValue>(this Dictionary<TKey, TValue> dict, Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            using (HashSetPool<TKey>.Get(out var toRemove))
            {
                foreach (var item in dict)
                {
                    if (predicate?.Invoke(item) ?? false) toRemove.Add(item.Key);
                }

                foreach (var item in toRemove)
                {
                    dict.Remove(item);
                }
            }
        }

        public static void RemoveWhere<TKey, TValue, TState>(this Dictionary<TKey, TValue> dict, TState state, Func<KeyValuePair<TKey, TValue>, TState, bool> predicate)
        {
            using (HashSetPool<TKey>.Get(out var toRemove))
            {
                foreach (var item in dict) if (predicate?.Invoke(item, state) ?? false) toRemove.Add(item.Key);
                foreach (var item in toRemove) dict.Remove(item);
            }
        }

        public static void RemoveWhere<T>(this IList<T> list, Func<T, bool> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate?.Invoke(list[i]) ?? false) list.RemoveAt(i--);
            }
        }

        public static void RemoveWhere<T, TState>(this IList<T> list, TState state, Func<T, TState, bool> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate?.Invoke(list[i], state) ?? false) list.RemoveAt(i--);
            }
        }
    }

    public interface IListPredicate<T>
    {
        public bool Test(T item);
    }
}
