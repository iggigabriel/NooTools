using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;

namespace Noo.Tools
{
    public struct NativeMemoryLayout : IDisposable
    {
        private int totalCapacity;

        public readonly int Capacity => totalCapacity;

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
                return $"NativeMemoryLayoutSlice(Start:{start}, Length:{length})";
            }
        }

        readonly NativeList<Slice> slices;

        public NativeMemoryLayout(int capacity, Allocator allocator)
        {
            totalCapacity = capacity;

            slices = new NativeList<Slice>(4, allocator)
            {
                new Slice(0, capacity)
            };
        }

        public Slice Allocate(int capacity)
        {
            int? lastSliceIndex = default;

            for (int i = 0; i < slices.Length; i++)
            {
                var slice = slices[i];

                if (slice.End == totalCapacity) lastSliceIndex = i;

                if (slice.Length > capacity)
                {
                    slices.RemoveAtSwapBack(i);
                    slices.Add(new Slice(slice.Start + capacity, slice.Length - capacity));
                    return new Slice(slice.Start, capacity);
                }
                else if (slice.Length == capacity)
                {
                    slices.RemoveAtSwapBack(i);
                    return slice;
                }
            }

            if (lastSliceIndex.HasValue) //Expand mem
            {
                var lastSlice = slices[lastSliceIndex.Value];
                slices.RemoveAtSwapBack(lastSliceIndex.Value);
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

        public readonly void Deallocate(Slice slice)
        {
            var newSlice = slice;

            do
            {
                slice = newSlice;

                for (int i = 0; i < slices.Length; i++)
                {
                    var current = slices[i];

                    var sliceEnd = newSlice.End;
                    var currentEnd = current.End;

                    if (sliceEnd < current.Start) continue;
                    if (newSlice.Start > currentEnd) continue;

                    slices.RemoveAtSwapBack(i);

                    var newStart = math.min(newSlice.Start, current.Start);
                    var newEnd = math.max(sliceEnd, currentEnd);

                    newSlice = new Slice(newStart, newEnd - newStart);
                    break;
                }
            }
            while (slice != newSlice);

            slices.Add(newSlice);
        }

        public readonly void Clear()
        {
            slices.Clear();
            slices.Add(new Slice(0, totalCapacity));
        }

        public readonly void Dispose() => slices.Dispose();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeArray<Slice>.Enumerator GetEnumerator() => slices.GetEnumerator();

        public override readonly string ToString()
        {
            var freeCount = 0;
            var partitionCount = 0;

            if (slices.IsCreated && !slices.IsEmpty)
            {
                partitionCount = slices.Length;

                for (int i = 0; i < partitionCount; i++)
                {
                    freeCount += slices[i].Length;
                }
            }

            return $"NativeMemoryLayout(Capacity: {totalCapacity}, Free: {freeCount}, Partitions: {partitionCount})";
        }
    }
}
