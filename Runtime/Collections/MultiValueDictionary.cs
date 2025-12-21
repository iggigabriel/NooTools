using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace Noo.Tools
{
    public sealed class MultiValueDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        const int DefaultKeyCapacity = 8;
        const int DefaultValueCapacity = 16;

        struct KeyInfo
        {
            public int index;
            public int valueCount;
            public MemoryLayout.Slice memory;
        }

        readonly MemoryLayout memoryLayout = new(DefaultValueCapacity);
        readonly Dictionary<TKey, KeyInfo> keyInfos = new(DefaultKeyCapacity);

        TKey[] keys = Array.Empty<TKey>();
        TValue[] values = Array.Empty<TValue>();

        int keyCount;
        int valueCount;
        int version;

        public int Count => valueCount;
        public int Capacity => values.Length;
        public int KeyCount => keyCount;

        public KeyCollection Keys => new(this);
        public ValueCollection this[TKey key] => new(this, key);

        public bool Add(TKey key, TValue value)
        {
            if (Contains(key, value)) return false;

            if (keyInfos.TryGetValue(key, out var keyInfo))
            {
                if (keyInfo.valueCount == keyInfo.memory.Length)
                {
                    Expand(ref keyInfo);
                }

                values[keyInfo.memory.Start + keyInfo.valueCount] = value;

                keyInfo.valueCount++;
                valueCount++;

                keyInfos[key] = keyInfo;
            }
            else
            {
                keyInfo.index = keyCount;

                EnsureKeyCapacity(keyCount + 1);

                Expand(ref keyInfo);

                keys[keyCount] = key;
                values[keyInfo.memory.Start] = value;

                keyInfo.valueCount++;
                keyInfos[key] = keyInfo;

                keyCount++;
                valueCount++;
            }

            version++;

            return true;
        }

        private void Expand(ref KeyInfo keyInfo)
        {
            var nextCapacity = Mathf.Max(2, keyInfo.memory.Length * 2);

            var newSlice = memoryLayout.Allocate(nextCapacity);

            if (memoryLayout.Capacity > values.Length)
            {
                Array.Resize(ref values, memoryLayout.Capacity);
            }

            for (var i = 0; i < keyInfo.memory.Length; i++)
            {
                values[newSlice.Start + i] = values[keyInfo.memory.Start + i];
                values[keyInfo.memory.Start + i] = default;
            }

            if (keyInfo.memory.Length > 0)
            {
                memoryLayout.Deallocate(keyInfo.memory);
            }

            keyInfo.memory = newSlice;
        }

        public bool Remove(TKey key)
        {
            if (!keyInfos.TryGetValue(key, out var keyInfo))
            {
                return false;
            }

            for (int i = 0; i < keyInfo.valueCount; i++)
            {
                values[keyInfo.memory.Start + i] = default;
            }

            valueCount -= keyInfo.valueCount;

            keys[keyInfo.index] = default;
            keyInfos.Remove(key);

            keyCount--;

            if (keyCount > 0 && keyCount != keyInfo.index)
            {
                var lastKey = keys[keyCount];
                var lastKeyInfo = keyInfos[lastKey];
                lastKeyInfo.index = keyInfo.index;
                keyInfos[lastKey] = lastKeyInfo;
                keys[keyInfo.index] = lastKey;
                keys[keyCount] = default;
            }

            version++;

            return true;
        }

        public bool Remove(TKey key, TValue value)
        {
            if (!keyInfos.TryGetValue(key, out var keyInfo))
            {
                return false;
            }

            var cmp = EqualityComparer<TValue>.Default;

            for (int i = 0; i < keyInfo.valueCount; i++)
            {
                var index = keyInfo.memory.Start + i;

                if (cmp.Equals(values[index], value))
                {
                    keyInfo.valueCount--;

                    if (keyInfo.valueCount == 0)
                    {
                        return Remove(key);
                    }

                    var lastIndex = keyInfo.memory.Start + keyInfo.valueCount;

                    values[index] = values[lastIndex];
                    values[lastIndex] = default;

                    keyInfos[key] = keyInfo;

                    valueCount--;

                    version++;

                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            keyCount = 0;
            valueCount = 0;
            memoryLayout.Clear();
            keyInfos.Clear();
            Array.Clear(values, 0, values.Length);
            Array.Clear(keys, 0, keys.Length);
            version++;
        }

        private void EnsureKeyCapacity(int capacity)
        {
            capacity = Mathf.NextPowerOfTwo(Mathf.Max(DefaultKeyCapacity, capacity));

            if (keys.Length < capacity)
            {
                Array.Resize(ref keys, capacity);
            }
        }

        public bool Contains(TKey key)
        {
            return keyInfos.ContainsKey(key);
        }

        public bool Contains(TKey key, TValue value)
        {
            return GetValues(key).Contains(value);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public ValueCollection GetValues(TKey key)
        {
            return new ValueCollection(this, key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct KeyCollection : IEnumerable<TKey>, IEnumerator<TKey>
        {
            readonly MultiValueDictionary<TKey, TValue> map;
            readonly int version;

            int index;

            public readonly TKey Current => map.keys[index];

            readonly object IEnumerator.Current => Current;

            public readonly int Count => map.keyCount;

            public KeyCollection(MultiValueDictionary<TKey, TValue> map)
            {
                this.map = map;
                version = map.version;
                index = -1;
            }

            public readonly bool Contains(TKey key)
            {
                return map.Contains(key);
            }

            public readonly KeyCollection GetEnumerator()
            {
                return this;
            }

            readonly void EnsureNotModified()
            {
                if (map.version != version)
                {
                    throw new InvalidOperationException("Collection was modified during enumeration.");
                }
            }

            public bool MoveNext()
            {
                EnsureNotModified();

                index++;

                return index < map.keyCount;
            }

            public void Reset()
            {
                index = -1;
            }

            public readonly void Dispose()
            {
            }

            readonly IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
            {
                return GetEnumerator();
            }

            readonly IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public struct ValueCollection : IEnumerable<TValue>, IEnumerator<TValue>
        {
            readonly MultiValueDictionary<TKey, TValue> map;
            readonly KeyInfo keyInfo;
            readonly int version;

            int index;

            public ValueCollection(MultiValueDictionary<TKey, TValue> map, TKey key)
            {
                this.map = map;
                map.keyInfos.TryGetValue(key, out keyInfo);
                version = map.version;
                index = -1;
            }

            public readonly TValue Current => map.values[keyInfo.memory.Start + index];

            readonly object IEnumerator.Current => Current;

            public readonly int Count => keyInfo.valueCount;

            public readonly bool Contains(TValue value)
            {
                EnsureNotModified();

                var cmp = EqualityComparer<TValue>.Default;

                if (keyInfo.valueCount == 0) return false;

                for (var i = 0; i < keyInfo.valueCount; i++)
                {
                    if (cmp.Equals(map.values[keyInfo.memory.Start + i], value))
                    {
                        return true;
                    }
                }

                return false;
            }

            public readonly void Dispose()
            {
            }

            public bool MoveNext()
            {
                EnsureNotModified();
                index++;
                return index < keyInfo.valueCount;
            }

            public void Reset()
            {
                index = -1;
            }

            public readonly ValueCollection GetEnumerator()
            {
                return this;
            }

            readonly void EnsureNotModified()
            {
                if (map.version != version)
                {
                    throw new InvalidOperationException("Collection was modified during enumeration.");
                }
            }

            readonly IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                return GetEnumerator();
            }

            readonly IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>
        {
            readonly MultiValueDictionary<TKey, TValue> map;
            readonly int version;

            KeyCollection keys;
            ValueCollection values;
            bool hasValue;

            public Enumerator(MultiValueDictionary<TKey, TValue> map)
            {
                this.map = map;
                version = map.version;

                keys = map.Keys;
                values = default;
                hasValue = false;
            }

            public readonly KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    if (!hasValue) return default;
                    EnsureNotModified();
                    return new KeyValuePair<TKey, TValue>(keys.Current, values.Current);
                }
            }

            readonly object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                EnsureNotModified();

                while (true)
                {
                    if (!hasValue)
                    {
                        if (!keys.MoveNext())
                        {
                            return false;
                        }

                        values = map.GetValues(keys.Current);
                        hasValue = true;
                    }

                    if (values.MoveNext())
                    {
                        return true;
                    }
                    else
                    {
                        hasValue = false;
                    }
                }
            }

            public void Reset()
            {
                keys.Reset();
                values = default;
                hasValue = false;
            }

            public readonly void Dispose()
            {
            }

            readonly void EnsureNotModified()
            {
                if (map.version != version)
                {
                    throw new InvalidOperationException("Collection was modified during enumeration.");
                }
            }

            public readonly IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return this;
            }

            readonly IEnumerator IEnumerable.GetEnumerator()
            {
                return this;
            }
        }
    }
}
