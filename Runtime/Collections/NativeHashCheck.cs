using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Noo.Tools
{
    public struct NativeHashCheck<T> : IDisposable where T : unmanaged
    {
        readonly int bucketMask;

        NativeArray<int> count;
        NativeArray<int> buckets;
        NativeArray<int> next;
        NativeArray<T> items;

        public int Count => count[0];

        public NativeHashCheck(int capacity, int itemsPerBucket, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
        {
            items = new NativeArray<T>(capacity, allocator, options);
            next = new NativeArray<int>(capacity, allocator, NativeArrayOptions.UninitializedMemory);
            count = new NativeArray<int>(1, allocator);

            var bucketLength = math.ceilpow2(capacity / itemsPerBucket);
            bucketMask = bucketLength - 1;

            buckets = new NativeArray<int>(bucketLength, allocator, NativeArrayOptions.UninitializedMemory);
            Clear();
        }

        public bool TryAdd(T item)
        {
            var cnt = count[0];

            if (cnt == buckets.Length) return false;

            var bucket = item.GetHashCode() & bucketMask;

            var first = buckets[bucket];

            if (first == -1)
            {
                next[cnt] = -1;
                items[cnt] = item;
                buckets[bucket] = cnt;
                count[0] = cnt + 1;
                return true;
            }
            else
            {
                var it = next[first];

                while (it > -1)
                {
                    if (items[it].Equals(item)) return false;
                    first = it;
                    it = next[it];
                }

                next[cnt] = -1;
                items[cnt] = item;
                next[first] = cnt;
                count[0] = cnt + 1;
                return true;
            }
        }

        public bool Contains(T item)
        {
            var it = buckets[item.GetHashCode() & bucketMask];

            while (it > -1)
            {
                if (items[it].Equals(item)) return true;
                it = next[it];
            }

            return false;
        }

        public void Clear()
        {
            unsafe
            {
                if (buckets.IsCreated)
                {
                    UnsafeUtility.MemSet(buckets.GetUnsafePtr(), 0xff, UnsafeUtility.SizeOf<T>() * buckets.Length);
                }
            }
        }

        public void Dispose()
        {
            if (buckets.IsCreated) buckets.Dispose();
            if (next.IsCreated) next.Dispose();
            if (items.IsCreated) items.Dispose();
            if (count.IsCreated) count.Dispose();
        }
    }
}
