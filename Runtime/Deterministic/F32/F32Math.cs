using System.Runtime.CompilerServices;

namespace Nootools.Deterministic
{
    public partial struct F32
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F32 operator +(F32 x, F32 y)
        {
            return new F32(x.raw + y.raw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F32 operator -(F32 x, F32 y)
        {
            return new F32(x.raw - y.raw);
        }

        public static F32 operator *(F32 x, F32 y)
        {
            var product = (long)x.raw * y.raw;

            // The upper 17 bits should all be the same (the sign).
            var upper = (uint)(product >> 47);

            if (product < 0)
            {
                if (~upper != 0) return MaxValue;

                // This adjustment is required in order to round -1/2 correctly
                product--;
            }
            else if (upper != 0) return MaxValue;

            var result = product >> 16;
            result += (product & 0x8000) >> 15;

            return new F32((int)result);
        }

        public static F32 Abs(F32 x)
        {
            int mask = x.raw >> 31;
            return new F32((x.raw + mask) ^ mask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F32 Floor(F32 x)
        {
            return new F32((int)((ulong)x.raw & 0xFFFF0000UL));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F32 Ceil(F32 x)
        {
            return new F32((int)(((ulong)x.raw & 0xFFFF0000UL) + (((ulong)x.raw & 0x0000FFFFUL) != 0UL ? (ulong)One.raw : 0UL)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F32 Min(F32 x, F32 y)
        {
            return x.raw < y.raw ? x : y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F32 Max(F32 x, F32 y)
        {
            return x.raw > y.raw ? x : y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F32 Clamp(F32 x, F32 min, F32 max)
        {
            return Min(Max(x, min), max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F32 Clamp01(F32 x)
        {
            return x.raw > Raw.One ? One : (x.raw < Raw.Zero ? Zero : x);
        }

        
    }
}