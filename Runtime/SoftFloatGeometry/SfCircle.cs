using System;
using System.Runtime.CompilerServices;

namespace Noo.Tools
{
    [Serializable]
    public struct SfCircle : IEquatable<SfCircle>, ISfShape
    {
        public Sfloat2 origin;
        public Sfloat radius;

        public readonly SfRect Bounds
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => SfRect.MinMaxRect(origin - radius, origin + radius);
        }

        public SfCircle(Sfloat2 origin, Sfloat radius)
        {
            this.origin = origin;
            this.radius = radius;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is SfCircle circle && Equals(circle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(SfCircle other)
        {
            return origin == other.origin && radius == other.radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(SfCircle lhs, SfCircle rhs) => lhs.Equals(rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(SfCircle lhs, SfCircle rhs) => !lhs.Equals(rhs);

        public override readonly int GetHashCode() => HashCode.Combine(origin, radius);

        public override readonly string ToString() => $"Circle(origin: {origin}, radius: {radius})";
    }
}
