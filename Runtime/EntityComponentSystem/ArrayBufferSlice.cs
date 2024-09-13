using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;

namespace Noo.Tools
{
    public readonly struct ArrayBufferSlice<T> : IEnumerable<T> where T : unmanaged
    {
        readonly T[] data;
        readonly int[] dataCounts;
        readonly int index;
        readonly int capacity;

        public int Count => dataCounts[index];
        public int Capacity => capacity;

        public ArrayBufferSlice(T[] data, int[] dataCounts, int capacity, int index)
        {
            this.data = data;
            this.dataCounts = dataCounts;
            this.capacity = capacity;
            this.index = index;
        }

        public ArraySegment<T> AsArraySegment()
        {
            var count = dataCounts[index];
            return new ArraySegment<T>(data, index * capacity, count);
        }

        public ArraySegment<T>.Enumerator GetEnumerator() => AsArraySegment().GetEnumerator();

        public T[] ToArray() => AsArraySegment().ToArray();

        public bool TryAdd(T item)
        {
            var count = dataCounts[index];
            if (count == capacity) return false;
            data[index * capacity + count] = item;
            count++;
            dataCounts[index] = count;
            return true;
        }

        public void RemoveAtSwapBack(int index)
        {
            var count = dataCounts[this.index];
            if (index < 0 || index >= dataCounts.Length) throw new ArgumentOutOfRangeException(nameof(index));
            count--;
            if (count != index) data[this.index * capacity + index] = data[this.index * capacity + count];
            dataCounts[this.index] = count;
        }

        public void SetData(T[] array)
        {
            var count = Math.Min(capacity, array.Length);
            dataCounts[index] = count;
            for (int i = 0; i < count; i++) data[index * capacity + i] = array[i];
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [BurstCompile]
    public struct NativeArrayBufferSlice<T> : IDisposable, IEnumerable<T> where T : unmanaged
    {
        readonly int capacity;
        readonly int index;

        NativeArray<T> data;
        int count;

        public readonly int Count => count;
        public readonly int Capacity => capacity;

        public NativeArrayBufferSlice(NativeArray<T>.ReadOnly data, NativeArray<int>.ReadOnly dataCounts, int capacity, int index, Allocator allocator = Allocator.TempJob)
        {
            this.data = new NativeArray<T>(capacity, allocator);
            this.capacity = capacity;
            this.index = index;
            count = dataCounts[index];
            NativeArray<T>.Copy(data, index * capacity, this.data, 0, count);
        }

        public NativeArray<T>.Enumerator GetEnumerator() => data.GetEnumerator();

        public readonly NativeArray<T> ToNativeArray() => data;

        public bool TryAdd(T item)
        {
            if (count == capacity) return false;
            data[count] = item;
            count++;
            return true;
        }

        public void RemoveAtSwapBack(int index)
        {
            if (index < 0 || index >= capacity) throw new ArgumentOutOfRangeException(nameof(index));
            count--;
            if (count != index) data[index] = data[count];
        }

        public void CopyToAndDispose(NativeArray<T> data, NativeArray<int> dataCounts)
        {
            CopyTo(data, dataCounts);
            Dispose();
        }

        public readonly void CopyTo(NativeArray<T> data, NativeArray<int> dataCounts)
        {
            NativeArray<T>.Copy(this.data, 0, data, index * capacity, count);
            dataCounts[index] = count;
        }

        public void Dispose()
        {
            data.Dispose();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
