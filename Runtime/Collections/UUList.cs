using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Noo.Tools
{
    /// <summary>
    /// Unique Unsorted List
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UUList<T> : IList<T>, IReadOnlyList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    {
        T[] m_data;
        int m_length;
        int m_capacity;
        readonly Dictionary<T, int> m_keys;

        public UUList()
        {
            m_keys = new Dictionary<T, int>();
            m_data = new T[0];
            m_capacity = 0;
        }

        public UUList(int capacity = 0)
        {
            m_keys = new Dictionary<T, int>();
            m_data = new T[capacity];
            m_capacity = capacity;
        }

        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_data[index];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                m_data[index] = value;
            }
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_length;
            }
        }

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_length;
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
            if (m_keys.ContainsKey(item)) return;

            while (m_length >= m_capacity)
            {
                Expand(m_capacity == 0 ? 1 : m_capacity * 2);
            }

            m_data[m_length] = item;
            m_keys[item] = m_length;
            m_length++;
        }

        public void Clear()
        {
            Array.Clear(m_data, 0, m_capacity);
            m_keys.Clear();
            m_length = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearFast()
        {
            m_keys.Clear();
            m_length = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item)
        {
            return m_keys.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_data.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T item)
        {
            if (m_keys.TryGetValue(item, out int index))
            {
                return index;
            }
            else
            {
                return -1;
            }
        }

        public void Insert(int index, T item)
        {
            if (index >= 0 && index < m_length)
            {
                // This is a unique list, if item is already added skip the rest
                if (m_keys.ContainsKey(item)) return;

                while (m_length >= m_capacity)
                {
                    Expand(m_capacity == 0 ? 1 : m_capacity * 2);
                }

                T oldItem = m_data[index];

                m_data[m_length] = oldItem;
                m_keys[oldItem] = m_length;

                m_data[index] = item;
                m_keys[item] = index;

                m_length++;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
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
                RemoveAt(index);
                return true;
            }
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < m_length)
            {
                m_length--;

                m_keys.Remove(m_data[index]);

                if (m_length != index)
                {
                    T newItem = m_data[m_length];
                    m_keys[newItem] = index;
                    m_data[index] = newItem;
                }

                m_data[m_length] = default;
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

        public void Resize(int length)
        {
            if (m_length < length)
            {
                Expand(length);
                m_length = length;
            }
            else if (m_length > length)
            {
                for (int i = length; i < m_length; i++)
                {
                    m_keys.Remove(m_data[i]);
                }

                Array.Clear(m_data, length, m_length - length);
                m_length = length;
            }
        }

        void Expand(int capacity)
        {
            m_capacity = capacity;
            Array.Resize(ref m_data, m_capacity);
        }

        public override string ToString()
        {
            StringBuilder sb = new();

            sb.Append("[");

            for (int i = 0; i < m_length; i++)
            {
                sb.Append(m_data[i].ToString()).Append(",");
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
