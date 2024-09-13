using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Noo.Tools
{
    [BurstCompile]
    public static class NooNativeUtility 
    {
        [BurstCompile]
        public static void ClearArray<T>(NativeArray<T> array) where T : unmanaged
        {
            for (int i = 0; i < array.Length; i++) array[i] = default;
        }

        /// <summary>Returns true if array was reallocated</summary>
        [BurstDiscard]
        public static bool EnsureCapacity<TKey>(ref NativeParallelHashSet<TKey> data, int capacity, Allocator allocator = Allocator.Persistent) where TKey : unmanaged, IEquatable<TKey>
        {
            if (!data.IsCreated || data.Capacity < capacity)
            {
                data.Dispose();
                data = new NativeParallelHashSet<TKey>(capacity, allocator);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Returns true if array was reallocated</summary>
        [BurstDiscard]
        public static bool EnsureCapacity<TKey, TValue>(ref NativeParallelHashMap<TKey, TValue> data, int capacity, Allocator allocator = Allocator.Persistent) where TKey : unmanaged, IEquatable<TKey> where TValue : unmanaged
        {
            if (!data.IsCreated || data.Capacity < capacity)
            {
                data.Dispose();
                data = new NativeParallelHashMap<TKey, TValue>(capacity, allocator);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Returns true if array was reallocated</summary>
        [BurstDiscard]
        public static bool EnsureCapacity<TKey, TValue>(ref NativeParallelMultiHashMap<TKey, TValue> data, int capacity, Allocator allocator = Allocator.Persistent) where TKey : unmanaged, IEquatable<TKey> where TValue : unmanaged
        {
            if (!data.IsCreated || data.Capacity < capacity)
            {
                data.Dispose();
                data = new NativeParallelMultiHashMap<TKey, TValue>(capacity, allocator);
                return true;
            }
            else
            {
                return false;
            }
        }

        [BurstCompile]
        public struct JobClearNativeHashSet<TKey> : IJob where TKey : unmanaged, IEquatable<TKey>
        {
            [WriteOnly]
            public NativeParallelHashSet<TKey> data;
            public JobClearNativeHashSet(NativeParallelHashSet<TKey> data) => this.data = data;
            public void Execute() => data.Clear();
        }

        [BurstCompile]
        public struct JobClearNativeHashMap<TKey, TValue> : IJob where TKey : unmanaged, IEquatable<TKey> where TValue : unmanaged
        {
            [WriteOnly]
            public NativeParallelHashMap<TKey, TValue> data;
            public JobClearNativeHashMap(NativeParallelHashMap<TKey, TValue> data) => this.data = data;
            public void Execute() => data.Clear();
        }

        [BurstCompile]
        public struct JobClearNativeMultiHashMap<TKey, TValue> : IJob where TKey : unmanaged, IEquatable<TKey> where TValue : unmanaged
        {
            [WriteOnly]
            public NativeParallelMultiHashMap<TKey, TValue> data;
            public JobClearNativeMultiHashMap(NativeParallelMultiHashMap<TKey, TValue> data) => this.data = data;
            public void Execute() => data.Clear();
        }
    }
}
