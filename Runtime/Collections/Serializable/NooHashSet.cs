using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Noo.Tools
{
    [Serializable]
    public class NooHashSet<T> : ISerializationCallbackReceiver, ISet<T>, IReadOnlyCollection<T>
    {
        [SerializeField, HideInInspector]
        private List<T> values = new();

        [ShowInInspector, HideLabel]
        private readonly HashSet<T> set = new();

        public int Count => set.Count;

        public bool IsReadOnly => ((ICollection<T>)set).IsReadOnly;

        public void OnAfterDeserialize()
        {
            set.Clear();
            foreach (var item in values) set.Add(item);
        }

        public void OnBeforeSerialize()
        {
            values.Clear();
            values.AddRange(set);
        }

        public bool Add(T item)
        {
            return set.Add(item);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            set.ExceptWith(other);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            set.IntersectWith(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return set.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return set.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return set.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return set.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return set.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return set.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            set.SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            set.UnionWith(other);
        }

        void ICollection<T>.Add(T item)
        {
            set.Add(item);
        }

        public void Clear()
        {
            set.Clear();
        }

        public bool Contains(T item)
        {
            return set.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            set.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return set.Remove(item);
        }

        public HashSet<T>.Enumerator GetEnumerator()
        {
            return set.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
