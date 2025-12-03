using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Noo.Tools
{
    [Serializable]
    public sealed class MemoryLayout
    {
        [SerializeField]
        private int totalCapacity;

        [SerializeField]
        readonly List<Slice> slices = new();

        public IReadOnlyList<Slice> Slices => slices;

        public int Capacity => totalCapacity;

        [Serializable]
        public readonly struct Slice : IEquatable<Slice>
        {
            readonly int start;
            readonly int length;

            public int Start
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return start;
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

            public int End
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return start + length;
                }
            }

            public Slice(int start, int length)
            {
                this.start = start;
                this.length = length;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Equals(object obj)
            {
                return obj is Slice slice && Equals(slice);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(Slice other)
            {
                return start == other.start && length == other.length;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override int GetHashCode()
            {
                return HashCode.Combine(start, length);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator ==(Slice left, Slice right)
            {
                return left.Equals(right);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator !=(Slice left, Slice right)
            {
                return !(left == right);
            }

            public override string ToString()
            {
                return $"MemoryLayoutSlice(Start:{start}, Length:{length})";
            }
        }

        public MemoryLayout()
        {
            totalCapacity = 8;
            slices.Add(new(0, 8));
        }

        public MemoryLayout(int capacity)
        {
            totalCapacity = capacity;
            slices.Add(new(0, capacity));
        }

        public Slice Allocate(int capacity)
        {
            int? lastSliceIndex = default;

            for (int i = 0; i < slices.Count; i++)
            {
                var slice = slices[i];

                if (slice.End == totalCapacity) lastSliceIndex = i;

                if (slice.Length > capacity)
                {
                    RemoveAtSwapBack(i);
                    slices.Add(new Slice(slice.Start + capacity, slice.Length - capacity));
                    return new Slice(slice.Start, capacity);
                }
                else if (slice.Length == capacity)
                {
                    RemoveAtSwapBack(i);
                    return slice;
                }
            }

            if (lastSliceIndex.HasValue)
            {
                var lastSlice = slices[lastSliceIndex.Value];
                RemoveAtSwapBack(lastSliceIndex.Value);
                totalCapacity += capacity - lastSlice.Length;
                return new Slice(lastSlice.Start, capacity);
            }
            else
            {
                var newSlice = new Slice(totalCapacity, capacity);
                totalCapacity += capacity;
                return newSlice;
            }
        }

        public void Deallocate(Slice slice)
        {
            var newSlice = slice;

            do
            {
                slice = newSlice;

                for (int i = 0; i < slices.Count; i++)
                {
                    var current = slices[i];

                    var sliceEnd = newSlice.End;
                    var currentEnd = current.End;

                    if (sliceEnd < current.Start) continue;
                    if (newSlice.Start > currentEnd) continue;

                    RemoveAtSwapBack(i);

                    var newStart = math.min(newSlice.Start, current.Start);
                    var newEnd = math.max(sliceEnd, currentEnd);

                    newSlice = new Slice(newStart, newEnd - newStart);
                    break;
                }
            }
            while (slice != newSlice);

            slices.Add(newSlice);
        }

        public void Clear()
        {
            slices.Clear();
            slices.Add(new Slice(0, totalCapacity));
        }

        private void RemoveAtSwapBack(int index)
        {
            if (index < 0 || index >= slices.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var lastIndex = slices.Count - 1;
            slices[index] = slices[lastIndex];
            slices.RemoveAt(lastIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<Slice>.Enumerator GetEnumerator() => slices.GetEnumerator();

        public override string ToString()
        {
            var freeCount = 0;
            var partitionCount = slices.Count;

            for (int i = 0; i < partitionCount; i++)
            {
                freeCount += slices[i].Length;
            }

            return $"MemoryLayout(Capacity: {totalCapacity}, Free: {freeCount}, Partitions: {partitionCount})";
        }
    }
}
