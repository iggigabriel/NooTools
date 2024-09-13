using System;

namespace Noo.Tools
{
    [Serializable]
    public struct SfloatSODState<T> where T : struct
    {
        public T previousValue;

        public T previousTarget;

        public T target;

        public T velocity;

        [NonSerialized]
        public T value;

        public Sfloat3 kValues;

        [NonSerialized]
        public Sfloat timeFraction;

        [NonSerialized]
        public Sfloat time;

        public SfloatSODState(T value)
        {
            target = previousTarget = previousValue = this.value = value;
            kValues = default;
            velocity = default;
            timeFraction = Sfloat.One;
            time = default;
        }

        public SfloatSODState(T value, T velocity, Sfloat3 kValues)
        {
            target = previousTarget = previousValue = this.value = value;
            this.kValues = kValues;
            this.velocity = velocity;
            timeFraction = Sfloat.One;
            time = default;
        }

        public void Reset(T value, bool resetVelocity = true, bool resetTime = true)
        {
            target = previousTarget = previousValue = this.value = value;

            if (resetVelocity) velocity = default;
            if (resetTime)
            {
                time = default;
                timeFraction = Sfloat.One;
            }
        }
    }
}