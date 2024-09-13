using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Noo.Tools
{
    [Serializable]
    public struct SfRay : IEquatable<SfRay>
    {
        public Sfloat2 origin;

        [SerializeField]
        private Sfloat2 direction;

        public Sfloat2 Direction
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get { return direction; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { origin = Sfloat2.NormalizeFast(value); }
        }

        public readonly Sfloat2 Normal
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Direction.Rotate90();
            }
        }

        public SfRay(Sfloat2 origin, Sfloat2 direction)
        {
            this.origin = origin;
            this.direction = Sfloat2.NormalizeFast(direction);
        }

        public SfRay(Sfloat2 origin, Sfloat2 direction, bool directionNormalized)
        {
            this.origin = origin;
            this.direction = directionNormalized ? direction : Sfloat2.NormalizeFast(direction);
        }

        /// <summary>Must provide already normalized direction or shit will happen</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDirectionUnsafe(Sfloat2 direction)
        {
            this.direction = direction;
        }

        public readonly SfRay Reversed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new(origin, -direction, true);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly bool Equals(object obj)
        {
            return obj is SfRay line && Equals(line);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(SfRay other)
        {
            return origin == other.origin && direction == other.direction;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(SfRay lhs, SfRay rhs) => lhs.Equals(rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(SfRay lhs, SfRay rhs) => !lhs.Equals(rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly int GetHashCode() => HashCode.Combine(origin, direction);

        public override readonly string ToString() => $"Ray(origin: {origin}, direction: {direction})";

        /// <summary>Returns a point at distance units along the ray.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Sfloat2 GetPoint(Sfloat distance) => origin + direction * distance;

        /// <summary>Returns a distance units to a point along the ray.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Sfloat GetDistance(Sfloat2 point) => Sfloat2.Dot(point - origin, direction);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly SfLine ToLine(Sfloat length) => new(origin, GetPoint(length));
    }
}
