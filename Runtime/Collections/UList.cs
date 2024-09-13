using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Noo.Tools
{
    /// <summary>
    /// Unsorted List
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UList<T> : IList<T>, IReadOnlyList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    {
        T[] m_data;
        int m_length;
        int m_capacity;

        public UList(int capacity = 0)
        {
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
            if (m_capacity == m_length)
            {
                Expand(m_capacity == 0 ? 1 : m_capacity * 2);
            }

            m_data[m_length] = item;
            m_length++;
        }

        public void Add(T item, out int index)
        {
            if (m_capacity == m_length)
            {
                Expand(m_capacity == 0 ? 1 : m_capacity * 2);
            }

            m_data[m_length] = item;

            index = m_length;

            m_length++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Array.Clear(m_data, 0, m_capacity);
            m_length = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearFast()
        {
            m_length = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item)
        {
            return Array.IndexOf(m_data, item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_data.CopyTo(array, arrayIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item)
        {
            return Array.IndexOf(m_data, item);
        }

        public void Insert(int index, T item)
        {
            if (index >= 0 && index < m_length)
            {
                if (m_capacity == m_length)
                {
                    Expand(m_capacity == 0 ? 1 : m_capacity * 2);
                }

                m_data[m_length] = m_data[index];
                m_data[index] = item;
                m_length++;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public bool Remove(T item)
        {
            var index = Array.IndexOf(m_data, item);

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
                m_data[index] = m_data[m_length];
                m_data[m_length] = default;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public void RemoveAt(int index, out T replacement)
        {
            if (index >= 0 && index < m_length)
            {
                m_length--;
                m_data[index] = m_data[m_length];
                m_data[m_length] = default;
                replacement = m_data[index];
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
