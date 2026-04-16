using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

namespace Noo.Tools
{
    public readonly struct DynamicCallback : IEquatable<DynamicCallback>
    {
        readonly byte stateCount;
        readonly object state0;
        readonly object state1;
        readonly object callback;

        private DynamicCallback(byte stateCount, object callback, object state0, object state1)
        {
            this.stateCount = stateCount;
            this.state0 = state0;
            this.state1 = state1;
            this.callback = callback;
        }

        public static DynamicCallback Create(Action callback)
        {
            return new DynamicCallback(1, callback, null, null);
        }

        public static DynamicCallback Create<T0>(T0 param0, Action<T0> callback)
        {
            return new DynamicCallback(2, callback, param0, null);
        }

        public static DynamicCallback Create<T0, T1>(T0 param0, T1 param1, Action<T0, T1> callback)
        {
            return new DynamicCallback(3, callback, param0, param1);
        }

        public override bool Equals(object obj)
        {
            return obj is DynamicCallback callback && Equals(callback);
        }

        public bool Equals(DynamicCallback other)
        {
            return stateCount == other.stateCount &&
                   EqualityComparer<object>.Default.Equals(state0, other.state0) &&
                   EqualityComparer<object>.Default.Equals(state1, other.state1) &&
                   EqualityComparer<object>.Default.Equals(callback, other.callback);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(stateCount, state0, state1, callback);
        }

        public void Invoke()
        {
            var callback = this.callback;

            if (callback == null) return;

            switch (stateCount)
            {
                case 1:
                    UnsafeUtility.As<object, Action>(ref callback)?.Invoke();
                    break;

                case 2:
                    UnsafeUtility.As<object, Action<object>>(ref callback)?.Invoke(state0);
                    break;

                case 3:
                    UnsafeUtility.As<object, Action<object, object>>(ref callback)?.Invoke(state0, state1);
                    break;
            }
        }

        public static bool operator ==(DynamicCallback left, DynamicCallback right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DynamicCallback left, DynamicCallback right)
        {
            return !(left == right);
        }
    }

    public readonly struct DynamicCallback<T> : IEquatable<DynamicCallback<T>>
    {
        readonly byte stateCount;
        readonly object state0;
        readonly object state1;
        readonly object callback;

        private DynamicCallback(byte stateCount, object callback, object state0, object state1)
        {
            this.stateCount = stateCount;
            this.state0 = state0;
            this.state1 = state1;
            this.callback = callback;
        }

        public static DynamicCallback<T> Create(Action<T> callback)
        {
            return new DynamicCallback<T>(1, callback, null, null);
        }

        public static DynamicCallback<T> Create<T0>(T0 param0, Action<T, T0> callback)
        {
            return new DynamicCallback<T>(2, callback, param0, null);
        }

        public static DynamicCallback<T> Create<T0, T1>(T0 param0, T1 param1, Action<T, T0, T1> callback)
        {
            return new DynamicCallback<T>(3, callback, param0, param1);
        }

        public override bool Equals(object obj)
        {
            return obj is DynamicCallback<T> callback && Equals(callback);
        }

        public bool Equals(DynamicCallback<T> other)
        {
            return stateCount == other.stateCount &&
                   EqualityComparer<object>.Default.Equals(state0, other.state0) &&
                   EqualityComparer<object>.Default.Equals(state1, other.state1) &&
                   EqualityComparer<object>.Default.Equals(callback, other.callback);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(stateCount, state0, state1, callback);
        }

        public void Invoke(T param)
        {
            var callback = this.callback;

            if (callback == null) return;

            switch (stateCount)
            {
                case 1:
                    UnsafeUtility.As<object, Action<T>>(ref callback)?.Invoke(param);
                    break;

                case 2:
                    UnsafeUtility.As<object, Action<T, object>>(ref callback)?.Invoke(param, state0);
                    break;

                case 3:
                    UnsafeUtility.As<object, Action<T, object, object>>(ref callback)?.Invoke(param, state0, state1);
                    break;
            }
        }

        public static bool operator ==(DynamicCallback<T> left, DynamicCallback<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DynamicCallback<T> left, DynamicCallback<T> right)
        {
            return !(left == right);
        }
    }
}
