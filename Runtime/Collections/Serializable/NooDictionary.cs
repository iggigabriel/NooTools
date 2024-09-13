using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Noo.Tools
{
    [Serializable]
    public class NooDictionary<TKey, TValue> : ISerializationCallbackReceiver, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        [SerializeField, HideInInspector]
        private List<TKey> keys = new();

        [SerializeField, HideInInspector]
        private List<TValue> values = new();

        [ShowInInspector, HideLabel]
        private readonly Dictionary<TKey, TValue> dictionary = new();

        public int Count => dictionary.Count;

        public bool IsReadOnly => false;

        public Dictionary<TKey, TValue>.KeyCollection Keys => dictionary.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => dictionary.Values;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => dictionary.Keys;
        ICollection<TValue> IDictionary<TKey, TValue>.Values => dictionary.Values;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => dictionary.Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => dictionary.Values;

        public TValue this[TKey key] { get => dictionary[key]; set => dictionary[key] = value; }

        public void OnAfterDeserialize()
        {
            dictionary.Clear();

            for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++) 
            {
                dictionary.Add(keys[i], values[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            keys.AddRange(dictionary.Keys);
            values.Clear();
            values.AddRange(dictionary.Values);
        }

        public void Add(TKey key, TValue value)
        {
            dictionary.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Add(item);
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Remove(item);
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
