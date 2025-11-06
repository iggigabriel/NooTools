using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Noo.Tools
{
    public readonly struct DynamicCallback
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
    }

    public readonly struct DynamicCallback<T>
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
    }
}
