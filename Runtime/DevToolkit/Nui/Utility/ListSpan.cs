using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Noo.Nui
{
    public readonly struct ListSpan<T> : IEquatable<ListSpan<T>>, IEnumerable<T>
    {
        public readonly IReadOnlyList<T> List { get; }
        public readonly int Offset { get; }
        public readonly int Count { get; }

        public T this[int index]
        {
            get => List[Offset + index];
        }

        public ListSpan(IReadOnlyList<T> list, int offset, int count)
        {
            List = list;
            Offset = offset;
            Count = count;
        }

        public override bool Equals(object obj)
        {
            return obj is ListSpan<T> span && Equals(span);
        }

        public bool Equals(ListSpan<T> other)
        {
            return List == other.List &&
                   Offset == other.Offset &&
                   Count == other.Count;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(List, Offset, Count);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        public static bool operator ==(ListSpan<T> span1, ListSpan<T> span2)
        {
            return span1.Equals(span2);
        }

        public static bool operator !=(ListSpan<T> span1, ListSpan<T> span2)
        {
            return !(span1 == span2);
        }

        public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
        {
            readonly ListSpan<T> span;

            int index;

            public Enumerator(ListSpan<T> span)
            {
                this.span = span;
                index = -1;
            }

            public readonly T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return span.List[span.Offset + index];
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
                return ++index < span.Count;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                index = -1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly Enumerator GetEnumerator()
            {
                return this;
            }
        }
    }

    internal static class ListSpanExtensions
    {
        public static ListSpan<T> ToListSpan<T>(this IReadOnlyList<T> list)
        {
            return GetSpan(list, 0, list?.Count ?? 0);
        }

        public static ListSpan<T> GetSpan<T>(this IReadOnlyList<T> list, int offset, int count)
        {
            return new ListSpan<T>(list, offset, count);
        }
    }
}
