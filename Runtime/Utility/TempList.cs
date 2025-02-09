using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace Noo.Tools
{
    public readonly struct TempList<T> : IList<T>, IDisposable
    {
        private readonly List<T> list;

        public TempList(IList<T> list = null)
        {
            this.list = ListPool<T>.Get();

            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    this.list.Add(list[i]);
                }
            }
        }

        public TempList(IEnumerable<T> list = null)
        {
            this.list = ListPool<T>.Get();

            if (list != null)
            {
                foreach (var item in list) this.list.Add(item);
            }
        }

        public static TempList<T> Empty() => new(null);

        public T this[int index] { get => list[index]; set => list[index] = value; }

        public void Dispose()
        {
            if (list != null) ListPool<T>.Release(list);
        }

        public int Count => list?.Count ?? 0;

        public bool IsReadOnly => throw new NotImplementedException();

        public bool Contains(T item) => list?.Contains(item) ?? false;
        public List<T>.Enumerator GetEnumerator() => list.GetEnumerator();
        public int IndexOf(T item) => list?.IndexOf(item) ?? -1;
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Insert(int index, T item)
        {
            list.Insert(index, item);
        }

        public void RemoveAtPushBack(int index)
        {
            if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException(nameof(index));
            var lastIndex = list.Count - 1;
            list[index] = list[lastIndex];
            list.RemoveAt(lastIndex);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        public void Add(T item)
        {
            list.Add(item);
        }

        public void Clear()
        {
            list.Clear();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return list.Remove(item);
        }
    }

    public static class TempListUtility
    {
        public static TempList<T> ToTempList<T>(this IEnumerable<T> list) => new(list);
    }
}
