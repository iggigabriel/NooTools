using Unity.Collections;

namespace Noo.Tools
{
    public class ArrayBuffer<T> where T : unmanaged
    {
        readonly T[] data;
        readonly int[] dataCounts;
        readonly int capacityPerSlice;

        public int Length => data.Length;
        public int CapacityPerSlice => capacityPerSlice;

        public ArrayBuffer(int capacity, int capacityPerSlice)
        {
            this.capacityPerSlice = capacityPerSlice;
            dataCounts = new int[capacity];
            data = new T[capacity * capacityPerSlice];
        }

        public ArrayBufferSlice<T> GetSlice(int index)
        {
            return new ArrayBufferSlice<T>(data, dataCounts, capacityPerSlice, index);
        }

        public void ToNativeArray(out NativeArray<T> data, out NativeArray<int> dataCounts, Allocator allocator = Allocator.TempJob)
        {
            data = new NativeArray<T>(this.data, allocator);
            dataCounts = new NativeArray<int>(this.dataCounts, allocator);
        }

        public void CopyToAndDispose(NativeArray<T> data, NativeArray<int> dataCounts)
        {
            CopyTo(data, dataCounts);
            data.Dispose();
            dataCounts.Dispose();
        }

        public void CopyTo(NativeArray<T> data, NativeArray<int> dataCounts)
        {
            NativeArray<T>.Copy(this.data, 0, data, 0, data.Length * capacityPerSlice);
            NativeArray<int>.Copy(this.dataCounts, 0, dataCounts, 0, this.dataCounts.Length);
        }
    }
}
