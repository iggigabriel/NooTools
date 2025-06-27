using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Noo.Nui
{
    public struct ListEnumeratorNonAlloc<T> : IEnumerator<T>, IEnumerator, IDisposable
    {
        readonly IReadOnlyList<T> list;

        int index;

        public ListEnumeratorNonAlloc(IReadOnlyList<T> list)
        {
            this.list = list;
            index = -1;
        }

        public readonly T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return list[index];
            }
        }

        readonly object IEnumerator.Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Current;
            }
        }

        public readonly void Dispose()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            return ++index < list.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            index = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ListEnumeratorNonAlloc<T> GetEnumerator()
        {
            return this;
        }
    }

    internal static class ListEnumeratorNonAllocUtil
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ListEnumeratorNonAlloc<T> EnumerateNonAlloc<T>(this IReadOnlyList<T> list)
        {
            return new ListEnumeratorNonAlloc<T>(list);
        }
    }
}
