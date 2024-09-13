using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace Noo.Tools
{
    [Serializable]
    public struct SODState<T> : IEquatable<SODState<T>> where T : struct
    {
        public T previousValue;

        public T previousTarget;

        public T target;

        public T velocity;

        [NonSerialized]
        public T value;

        public float3 kValues;

        [NonSerialized]
        public float timeFraction;

        public SODState(T value)
        {
            target = previousTarget = previousValue = this.value = value;
            kValues = default;
            velocity = default;
            timeFraction = 1f;
        }

        public override bool Equals(object obj)
        {
            return obj is SODState<T> state && Equals(state);
        }

        public bool Equals(SODState<T> other)
        {
            return EqualityComparer<T>.Default.Equals(previousValue, other.previousValue) &&
                   EqualityComparer<T>.Default.Equals(previousTarget, other.previousTarget) &&
                   EqualityComparer<T>.Default.Equals(target, other.target) &&
                   EqualityComparer<T>.Default.Equals(velocity, other.velocity) &&
                   EqualityComparer<T>.Default.Equals(value, other.value) &&
                   kValues.Equals(other.kValues) &&
                   timeFraction == other.timeFraction;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(previousValue, previousTarget, target, velocity, value, kValues, timeFraction);
        }

        public void Reset(T value, bool resetVelocity = true, bool resetTime = true)
        {
            target = previousTarget = previousValue = this.value = value;

            if (resetVelocity) velocity = default;
            if (resetTime) timeFraction = 1f;
        }

        public static bool operator ==(SODState<T> left, SODState<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SODState<T> left, SODState<T> right)
        {
            return !(left == right);
        }
    }
}