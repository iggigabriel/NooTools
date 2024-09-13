using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Noo.Tools
{
    /// <summary>
    /// Ordered collection, 
    /// add is fast, 
    /// remove and contains are still slow, 
    /// holes with default value are left and Length is only decreased when cleared completely
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OrderedCollection<T> : IList<T>, IReadOnlyList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    {
        int[] stack;
        T[] data;
        int stackLength;
        int length;
        int capacity;

        public OrderedCollection(int capacity = 0)
        {
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
            }
            else
            {
                if (capacity == length)
                {
                    Expand(capacity == 0 ? 1 : capacity * 2);
                }

                data[length] = item;
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
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Array.Clear(data, 0, length);
            length = 0;
            stackLength = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearFast()
        {
            length = 0;
            stackLength = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item)
        {
            return Array.IndexOf(data, item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            data.CopyTo(array, arrayIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item)
        {
            return Array.IndexOf(data, item);
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public bool Remove(T item)
        {
            var index = Array.IndexOf(data, item);

            if (index == -1)
            {
                return false;
            }
            else
            {
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
