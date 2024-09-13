using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace Noo.Tools
{

    public struct SfRandom : IEquatable<SfRandom>
    {
        const uint C1 = 842502087, C2 = 3579807591, C3 = 273326509;

        uint4 state;

        public SfRandom(uint seed)
        {
            state = new uint4(seed, C1, C2, C3);
        }

        public SfRandom(uint4 state)
        {
            this.state = state;
        }

        /// <summary>Creates totally non-deterministic random instance based on <see cref="Environment.TickCount"/></summary>
        public static SfRandom New() => new((uint)Environment.TickCount);

        public uint NextUint()
        {
            uint t = (state.x ^ (state.x << 11));
            state.x = state.y; state.y = state.z; state.z = state.w;
            state.w = (state.w ^ (state.w >> 19)) ^ (t ^ (t >> 8));
            return state.w;
        }

        /// <summary>Sfloat value in full range</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Sfloat Next() => Sfloat.FromRaw((int)NextUint());

        /// <summary>Sfloat value in range [0(inclusive)-1(exclusive)]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Sfloat Range01() => Sfloat.FromRaw(SfMath.Fract((int)NextUint()));

        /// <summary>Sfloat value in range [min(inclusive)-max(exclusive)]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Sfloat Range(Sfloat min, Sfloat max)
        {
            return min + (max - min) * Range01();
        }

        /// <summary>Sfloat value in range [0(inclusive)-max(exclusive)]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Sfloat Range(Sfloat max)
        {
            return Range01() * max;
        }

        /// <summary>Sfloat value in [-<paramref name="range"/> to <paramref name="range"/>]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public Sfloat Spread(Sfloat range) => Range(-range, range);

        /// <summary>Sfloat value in [-<paramref name="range"/> to <paramref name="range"/>] from <paramref name="origin"/></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public Sfloat Spread(Sfloat origin, Sfloat range) => Range(origin - range, origin + range);

        /// <summary>Sfloat value in range [min(inclusive)-max(exclusive)]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Range(int min, int max)
        {
            return min + Sfloat.FloorToInt(Sfloat.FromInt(max - min) * Range01());
        }

        /// <summary>Sfloat value in range [0(inclusive)-max(exclusive)]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Range(int max)
        {
            return Sfloat.FloorToInt(Sfloat.FromInt(max) * Range01());
        }

        /// <summary>True in 50% cases</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool NextBool()
        {
            return Range01().Raw < SfMath.Half;
        }

        /// <param name="chance">Expected range [0-1]</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool NextBool(Sfloat chance)
        {
            return Range01().Raw < chance.Raw;
        }

        public override readonly string ToString()
        {
            return $"SfRandom({ToHexString()})";
        }

        public readonly string ToHexString()
        {
            return $"{state.x:X8}{state.y:X8}{state.z:X8}{state.w:X8}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is SfRandom random && Equals(random);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(SfRandom other)
        {
            return state.Equals(other.state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(SfRandom left, SfRandom right)
        {
            return left.state.Equals(right.state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(SfRandom left, SfRandom right)
        {
            return !left.state.Equals(right.state);
        }
    }
}
