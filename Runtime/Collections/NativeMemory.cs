using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;

namespace Noo.Tools
{
    public struct NativeMemory<T> : IDisposable where T : unmanaged
    {
        public readonly int Capacity => layout.Capacity;

        NativeMemoryLayout layout;
        NativeList<T> data;

        public readonly bool IsCreated => data.IsCreated;

        public NativeMemory(int capacity, Allocator allocator)
        {
            layout = new NativeMemoryLayout(capacity, allocator);
            data = new NativeList<T>(capacity, allocator);
        }

        public void Dispose()
        {
            layout.Dispose();
            data.Dispose();
        }

        public NativeMemoryLayout.Slice Allocate(int capacity)
        {
            if (!IsCreated) throw new ObjectDisposedException("The NativeMemory is already disposed.");

            var memSlice = layout.Allocate(capacity);
            if (layout.Capacity > data.Length) data.Length = layout.Capacity;
            return memSlice;
        }

        public void Deallocate(in NativeMemoryLayout.Slice slice, bool clearMemory = false)
        {
            if (!IsCreated) throw new ObjectDisposedException("The NativeMemory is already disposed.");

            if (clearMemory) NooNativeUtility.ClearArray(GetSubArray(slice));

            layout.Deallocate(slice);
        }

        public NativeMemoryLayout.Slice Reallocate(in NativeMemoryLayout.Slice slice, int newCapacity, bool clearMemory = true)
        {
            if (!IsCreated) throw new ObjectDisposedException("The NativeMemory is already disposed.");

            var newSlice = Allocate(newCapacity);
            NativeArray<T>.Copy(GetSubArray(slice), GetSubArray(newSlice), math.min(slice.Length, newSlice.Length));
            Deallocate(slice, clearMemory);
            return newSlice;
        }

        /// <summary>Be careful not to reallocate memory while accessing this reference</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray<T> GetSubArray(in NativeMemoryLayout.Slice slice)
        {
            return data.AsArray().GetSubArray(slice.Start, slice.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int index, in T value)
        {
            data[index] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(in NativeMemoryLayout.Slice slice, int index, in T value)
        {
            data[slice.Start + index] = value;
        }

        /// <summary>Be careful not to reallocate memory while accessing this reference</summary>
        public NativeArray<T> AsArray() => data.AsArray();
    }
}
