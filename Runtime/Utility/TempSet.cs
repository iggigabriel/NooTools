using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Noo.Tools
{
    public readonly struct TempSet<T> : IReadOnlyCollection<T>, IDisposable
    {
        private readonly HashSet<T> set;

        public int Count => set.Count;

        public TempSet(ReadOnlySpan<T> list)
        {
            set = HashSetPool<T>.Get();
            AddRange(list);
        }

        public TempSet(IReadOnlyList<T> list)
        {
            set = HashSetPool<T>.Get();
            AddRange(list);
        }

        public TempSet(IEnumerable<T> list)
        {
            set = HashSetPool<T>.Get();
            AddRange(list);
        }

        public bool Add(T item) => set.Add(item);
        public bool Remove(T item) => set.Remove(item);
        public void Clear() => set.Clear();
        public bool Contains(T item) => set.Contains(item);

        public void AddRange(ReadOnlySpan<T> list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                set.Add(list[i]);
            }
        }

        public void AddRange(IReadOnlyList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                set.Add(list[i]);
            }
        }

        public void AddRange(IEnumerable<T> list)
        {
            foreach (var item in list) set.Add(item);
        }

        public void Dispose()
        {
            HashSetPool<T>.Release(set);
        }

        public HashSet<T>.Enumerator GetEnumerator() => set.GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public HashSet<T> AsHashSet() => set;

        public TempList<T> ToTempList(bool dispose = false)
        {
            var list = TempList<T>.Empty();
            foreach (var item in set) list.Add(item);
            if (dispose) Dispose();
            return list;
        }

        public static TempList<T> Empty() => new(default(ReadOnlySpan<T>));

    }

    public static class TempSetUtility
    {
        public static TempSet<T> ToTempSet<T>(this IEnumerable<T> collection) => new(collection);
        public static TempSet<T> ToTempSet<T>(this IReadOnlyList<T> list) => new(list);
    }

}
