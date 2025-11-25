using System;
using System.Buffers;
using System.Collections.Generic;
using UnityEngine;

namespace Noo.Tools
{
    public sealed class MultiValueHashMap<TKey, TValue>
    {
        TValue[] values = Array.Empty<TValue>();
        int[] lengths = Array.Empty<int>();
        int[] freeIndices = Array.Empty<int>();
        int freeIndicesCount;
        int maxValues;
        int count;

        public int Count => count;
        public int Capacity => values.Length;

        public ReadOnlySpan<TValue> this[TKey key]
        {
            get => GetValues(key);
        }

        readonly Dictionary<TKey, int> indices = new();

        public Dictionary<TKey, int>.KeyCollection Keys => indices.Keys;

        public bool Add(TKey key, TValue value)
        {
            if (!indices.TryGetValue(key, out var index))
            {
                if (freeIndicesCount > 0)
                {
                    freeIndicesCount--;
                    index = freeIndices[freeIndicesCount];
                }
                else
                {
                    index = indices.Count + 1;
                    AssertLength(index, maxValues);
                }
            }
            else
            {
                for (int i = 0; i < lengths[index]; i++)
                {
                    if (values[index * maxValues + i].Equals(value)) return false;
                }

                AssertLength(indices.Count, lengths[index] + 1);
            }

            indices[key] = index;

            var length = lengths[index];
            values[index * maxValues + length] = value;
            lengths[index] = length + 1;
            count++;

            return true;
        }

        public bool Remove(TKey key)
        {
            if (indices.TryGetValue(key, out var index))
            {
                freeIndices[freeIndicesCount++] = index;
                Array.Clear(values, index * maxValues, lengths[index]);
                count -= lengths[index];
                lengths[index] = 0;
                indices.Remove(key);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Remove(TKey key, TValue value)
        {
            if (indices.TryGetValue(key, out var index))
            {
                var offset = index * maxValues;

                for (var i = 0; i < lengths[index]; i++)
                {
                    if (values[offset + i].Equals(value))
                    {
                        for (var j = i + 1; j < lengths[index]; j++)
                        {
                            values[offset + j - 1] = values[offset + j];
                        }

                        lengths[index]--;
                        count--;

                        values[offset + lengths[index]] = default;

                        if (lengths[index] == 0)
                        {
                            freeIndices[freeIndicesCount++] = index;
                            indices.Remove(key);
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        void AssertLength(int keyCount, int maxValues)
        {
            keyCount = Mathf.NextPowerOfTwo(Mathf.Max(keyCount, 16));
            maxValues = Mathf.NextPowerOfTwo(Mathf.Max(maxValues, 2));

            var size = Mathf.NextPowerOfTwo(keyCount * maxValues);

            if (values.Length < size)
            {
                var next = ArrayPool<TValue>.Shared.Rent(size);

                Array.Clear(next, 0, size);

                for (int i = 0; i < lengths.Length; i++)
                {
                    for (int j = 0; j < this.maxValues; j++)
                    {
                        next[i * maxValues + j] = values[i * this.maxValues + j];
                    }
                }

                this.maxValues = maxValues;

                if (values.Length > 0)
                {
                    ArrayPool<TValue>.Shared.Return(values, true);
                }

                values = next;
            }

            if (lengths.Length < keyCount)
            {
                Array.Resize(ref lengths, keyCount);
                Array.Resize(ref freeIndices, keyCount);
            }
        }

        public ReadOnlySpan<TValue> GetValues(TKey key)
        {
            if (indices.TryGetValue(key, out var index))
            {
                return new ReadOnlySpan<TValue>(values, index * maxValues, lengths[index]);
            }
            else
            {
                return ReadOnlySpan<TValue>.Empty;
            }
        }

        public void Clear()
        {
            var length = indices.Count;

            indices.Clear();

            Array.Clear(values, 0, length);
            Array.Clear(lengths, 0, length);
            Array.Clear(freeIndices, 0, freeIndicesCount);

            freeIndicesCount = 0;
            maxValues = 0;
            count = 0;
        }

        public bool Contains(TKey key)
        {
            return indices.ContainsKey(key);
        }

        public bool Contains(TKey key, TValue value)
        {
            if (indices.TryGetValue(key, out var index))
            {
                for (var i = 0; i < lengths[index]; i++)
                {
                    if (values[index * maxValues + i].Equals(value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
