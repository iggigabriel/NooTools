using System;
using System.Collections.Generic;
using System.Collections;

namespace Noo.Tools
{
    public readonly struct ListSpan<T> : IEquatable<ListSpan<T>>, IEnumerable<T>
    {
        public readonly IReadOnlyList<T> list;
        public readonly int index;
        public readonly int count;

        public ListSpan(IReadOnlyList<T> list, int index, int count)
        {
            this.list = list;
            this.index = index;
            this.count = count;
        }

        public override bool Equals(object obj)
        {
            return obj is ListSpan<T> span && Equals(span);
        }

        public bool Equals(ListSpan<T> other)
        {
            return list == other.list &&
                   index == other.index &&
                   count == other.count;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (list == null) yield break;
            for (var i = 0; i < count; i++) yield return list[index + i];
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(list, index, count);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static bool operator ==(ListSpan<T> span1, ListSpan<T> span2)
        {
            return span1.Equals(span2);
        }

        public static bool operator !=(ListSpan<T> span1, ListSpan<T> span2)
        {
            return !(span1 == span2);
        }
    }

    public static class ListSpanExtensions
    {
        public static ListSpan<T> GetSpan<T>(this IReadOnlyList<T> list, int index, int count)
        {
            return new ListSpan<T>(list, index, count);
        }
    }
}
