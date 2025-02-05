using System;
using System.Runtime.CompilerServices;

namespace Noo.Tools
{
    [Serializable]
    public struct SfCapsule : IEquatable<SfCapsule>, ISfShape
    {
        public Sfloat2 p1;
        public Sfloat2 p2;
        public Sfloat radius;

        public readonly SfLine Line => new(p1, p2);

        public readonly SfRect Bounds
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => SfRect.Expand(SfRect.FromPoints(p1, p2), radius);
        }

        public SfCapsule(Sfloat2 p1, Sfloat2 p2, Sfloat radius)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.radius = radius;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is SfCapsule capsule && Equals(capsule);
        }

        public readonly bool Equals(SfCapsule other)
        {
            return p1.Equals(other.p1) &&
                   p2.Equals(other.p2) &&
                   radius.Equals(other.radius);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(p1, p2, radius);
        }

        public static bool operator ==(SfCapsule left, SfCapsule right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SfCapsule left, SfCapsule right)
        {
            return !(left == right);
        }

        public readonly bool Contains(Sfloat2 point)
        {
            return SfGeom.ShortestLineBetweenPointAndLine(point, Line).Length <= radius;
        }

        public override readonly string ToString() => $"Capsule(p1: {p1.ToString("0.000")}, p2: {p2.ToString("0.000")}, radius: {radius.ToString("0.000")})";
    }
}
