using System;
using System.Runtime.CompilerServices;

namespace Noo.Tools
{
    [Serializable]
    public struct SfLine : IEquatable<SfLine>, ISfShape
    {
        public Sfloat2 p1;
        public Sfloat2 p2;

        public SfLine(Sfloat2 p1, Sfloat2 p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public readonly SfLine Reversed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new(p2, p1);
            }
        }

        public readonly Sfloat2 Vector
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return p2 - p1;
            }
        }

        public readonly Sfloat LengthSqr
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Sfloat2.LengthSqr(p1 - p2).Sfloat;
            }
        }

        public Sfloat Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get
            {
                return Sfloat2.LengthFast(p1 - p2);
            }
            set
            {
                p2 = Direction * value;
            }
        }

        public readonly Sfloat2 Direction
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Vector / Length;
            }
        }

        public readonly Sfloat2 Normal
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Direction.Rotate270();
            }
        }

        public readonly SfLine Normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new(p1, p1 + Direction);
            }
        }

        public readonly Sfloat2 Center
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (p1 + p2) >> 1;
            }
        }

        public readonly SfRect Bounds
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => SfRect.FromPoints(p1, p2);
        }

        public readonly bool IsZeroLength => p1 == p2;

        public override readonly bool Equals(object obj)
        {
            return obj is SfLine line && Equals(line);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(SfLine other)
        {
            return p1 == other.p1 && p2 == other.p2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(SfLine lhs, SfLine rhs) => lhs.Equals(rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(SfLine lhs, SfLine rhs) => !lhs.Equals(rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly int GetHashCode() => HashCode.Combine(p1, p2);

        public override readonly string ToString() => $"Line(p1: {p1.ToString("0.000")}, p2: {p2.ToString("0.000")})";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly SfRay ToRay() => new(p1, Vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly SfRay ToRay(out Sfloat length)
        {
            var vec = p2 - p1;
            length = vec.Magnitude;
            return new(p1, vec / length, true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Sfloat2 GetPoint(Sfloat t)
        {
            return p1 + (p2 - p1) * t;
        }
    }
}
