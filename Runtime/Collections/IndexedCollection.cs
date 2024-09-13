using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Noo.Tools
{
    /// <summary>
    /// Indexed collection, same as Ordered Collection with fast Item acces (Remove, IndexOf, Contains)
    /// Add and remove are fast but holes with default value are left and Length is only decreased when cleared completely
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IndexedCollection<T> : IList<T>, IReadOnlyList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    {
        readonly Dictionary<T, int> itemIndex;
        int[] stack;
        T[] data;
        int stackLength;
        int length;
        int capacity;

        public IndexedCollection(int capacity = 0)
        {
            itemIndex = new Dictionary<T, int>();
            data = new T[capacity];
            stack = new int[capacity];
            this.capacity = capacity;
        }

        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return data[index];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                data[index] = value;
            }
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return length;
            }
        }

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return length;
            }
        }

        public bool IsReadOnly
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return false;
            }
        }

        public void Add(T item)
        {
            if (stackLength > 0)
            {
                stackLength--;
                var index = stack[stackLength];
                data[index] = item;
                itemIndex[item] = index;
            }
            else
            {
                if (capacity == length)
                {
                    Expand(capacity == 0 ? 1 : capacity * 2);
                }

                data[length] = item;
                itemIndex[item] = length;
                length++;
            }
        }

        public void Add(T item, out int index)
        {
            if (stackLength > 0)
            {
                stackLength--;
                index = stack[stackLength];
                data[index] = item;
            }
            else
            {
                if (capacity == length)
                {
                    Expand(capacity == 0 ? 1 : capacity * 2);
                }

                data[length] = item;

                index = length;
                length++;
            }

            itemIndex[item] = index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            itemIndex.Clear();
            Array.Clear(data, 0, length);
            length = 0;
            stackLength = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearFast()
        {
            itemIndex.Clear();
            length = 0;
            stackLength = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item)
        {
            return itemIndex.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            data.CopyTo(array, arrayIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item)
        {
            if (itemIndex.TryGetValue(item, out var index)) return index;
            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public bool Remove(T item)
        {
            var index = IndexOf(item);

            if (index == -1)
            {
                return false;
            }
            else
            {
                itemIndex.Remove(item);
                RemoveAt(index);
                return true;
            }
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < length)
            {
                data[index] = default;
                stack[stackLength] = index;
                stackLength++;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ListEnumeratorNonAlloc<T> GetEnumerator()
        {
            return new ListEnumeratorNonAlloc<T>(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void Expand(int capacity)
        {
            this.capacity = capacity;
            Array.Resize(ref data, this.capacity);
            Array.Resize(ref stack, this.capacity);
        }

        public override string ToString()
        {
            StringBuilder sb = new();

            sb.Append("[");

            for (int i = 0; i < length; i++)
            {
                sb.Append(data[i].ToString()).Append(",");
            }

            if (sb.Length > 1)
            {
                sb.Length--;
            }

            sb.Append("]");

            return sb.ToString();
        }

        
    }
}
