//
// FixPointCS => Soft Float Library
//
// Copyright(c) Jere Sanisalo, Petri Kero, Ivan Gabriel
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

//
// GENERATED FILE!!!
//
// Generated from Fixed64.cs, part of the FixPointCS aka. Soft Float Library project (MIT license).
//

/* Coding style:
 * 
 * In order to keep the transpiled C++/Java code working, here are some generic
 * coding guidelines.
 * 
 * All 64bit constants should be of the form " -1234L" or " 0x1234L" (so start
 * with a whitespace and end with a capital L).
 * 
 * All definitions should be in dependency order. That is, define functions
 * that are used later first. This is because C++ processes things in order,
 * where as in C# the definition order doesn't matter.
 * 
 * Minimize the use of system libraries.
 * 
 * There is a very limited preprocessor, which accepts "#if <LANG>",
 * "#elif <LANG>", "#else", as well as "#if !TRANSPILE" directives. No nested
 * directives are allowed.
 * 
 * Use up-to C# 3 features to keep the library compatible with older versions
 * of Unity.
 */
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Noo.Tools
{
    /// <summary>
    /// Direct fixed point (signed 16.16) functions.
    /// </summary>
    public static class SfMath
    {
        public const int Shift = 16;
        public const int FractionMask = (1 << Shift) - 1;
        public const int IntegerMask = ~FractionMask;

        // Constants
        public const int Zero = 0;
        public const int NegOne = -1 << Shift;
        public const int One = 1 << Shift;
        public const int Two = 2 << Shift;
        public const int Three = 3 << Shift;
        public const int Four = 4 << Shift;
        public const int Half = One >> 1;
        public const int NegHalf = NegOne >> 1;
        public const int Pi = (int)(13493037705L >> 16); //(int)(Math.PI * 65536.0) << 16;
        public const int Pi2 = (int)(26986075409L >> 16);
        public const int PiHalf = (int)(6746518852L >> 16);
        public const int E = (int)(11674931555L >> 16);

        public const int MinValue = -2147483648;
        public const int MaxValue = 2147483647;

        // Private constants
        private const int RCP_LN2 = (int)(0x171547652L >> 16);    // 1.0 / log(2.0) ~= 1.4426950408889634
        private const int RCP_LOG2_E = (int)(2977044471L >> 16);     // 1.0 / log2(e) ~= 0.6931471805599453
        private const int RCP_TWO_PI = 683565276;                    // 1.0 / (4.0 * 0.5 * pi);  -- the 4.0 factor converts directly to s2.30

        /// <summary>
        /// Converts an integer to a fixed-point value.
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int FromInt(int v)
        {
            return (int)v << Shift;
        }

        /// <summary>
        /// Converts a double to a fixed-point value.
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int FromDouble(double v)
        {
            return (int)(v * 65536.0);
        }

        /// <summary>
        /// Converts a float to a fixed-point value.
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int FromFloat(float v)
        {
            return (int)(v * 65536.0f);
        }

        /// <summary>
        /// Converts a fixed-point value into an integer by rounding it up to nearest integer.
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int CeilToInt(int v)
        {
            return (int)((v + (One - 1)) >> Shift);
        }

        /// <summary>
        /// Converts a fixed-point value into an integer by rounding it down to nearest integer.
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int FloorToInt(int v)
        {
            return (int)(v >> Shift);
        }

        /// <summary>
        /// Converts a fixed-point value into an integer by rounding it to nearest integer.
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int RoundToInt(int v)
        {
            return (int)((v + Half) >> Shift);
        }

        /// <summary>
        /// Converts a fixed-point value into a double.
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static double ToDouble(int v)
        {
            return (double)v * (1.0 / 65536.0);
        }

        /// <summary>
        /// Converts a FP value into a float.
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static float ToFloat(int v)
        {
            return (float)v * (1.0f / 65536.0f);
        }

        /// <summary>
        /// Converts the value to a human readable string.
        /// </summary>
        public static string ToString(int v)
        {
            return ToFloat(v).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the value to a human readable string.
        /// </summary>
        public static string ToString(int v, string format)
        {
            return ToFloat(v).ToString(format, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the absolute (positive) value of x.
        /// Fails with MinValue
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int Abs(int x)
        {
            // \note fails with MinValue
            int mask = x >> 31;
            return (x + mask) ^ mask;
        }

        /// <summary>
        /// Negative absolute value (returns -abs(x)).
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int Nabs(int x)
        {
            return -Abs(x);
        }

        /// <summary>
        /// Round up to nearest integer.
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int Ceil(int x)
        {
            return (x + FractionMask) & IntegerMask;
        }

        /// <summary>
        /// Round down to nearest integer.
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int Floor(int x)
        {
            return x & IntegerMask;
        }

        /// <summary>
        /// Round to nearest integer.
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int Round(int x)
        {
            return (x + Half) & IntegerMask;
        }

        /// <summary>
        /// Returns the fractional part of x. Equal to 'x - floor(x)'.
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int Fract(int x)
        {
            return x & FractionMask;
        }

        /// <summary>
        /// Returns the minimum of the two values.
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int Min(int a, int b)
        {
            return (a < b) ? a : b;
        }

        /// <summary>
        /// Returns the maximum of the two values.
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int Max(int a, int b)
        {
            return (a > b) ? a : b;
        }

        /// <summary>
        /// Returns the value clamped between min and max.
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int Clamp(int a, int min, int max)
        {
            return (a > max) ? max : (a < min) ? min : a;
        }

        /// <summary>
        /// Returns the sign of the value (-1 if negative, 0 if zero, 1 if positive).
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int Sign(int x)
        {
            // https://stackoverflow.com/questions/14579920/fast-sign-of-integer-in-c/14612418#14612418
            return ((x >> 31) | (int)(((uint)-x) >> 31));
        }

        /// <summary>
        /// Adds the two FP numbers together.
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int Add(int a, int b)
        {
            return a + b;
        }

        /// <summary>
        /// Subtracts the two FP numbers from each other.
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int Sub(int a, int b)
        {
            return a - b;
        }

        /// <summary>
        /// Multiplies two FP values together.
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int Mul(int a, int b)
        {
            return (int)(((long)a * (long)b) >> Shift);
        }

        /// <summary>
        /// Linearly interpolate from a to b by t.
        /// </summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int Lerp(int a, int b, int t)
        {
            long ta = (long)a * (One - (long)t);
            long tb = (long)b * (long)t;
            return (int)((ta + tb) >> Shift);
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        private static int Nlz(uint x)
        {
            //return System.Numerics.BitOperations.LeadingZeroCount(x); \note Disabled as this is slower in benchmarks
            int n = 0;
            if (x <= 0x0000FFFF) { n = n + 16; x = x << 16; }
            if (x <= 0x00FFFFFF) { n = n + 8; x = x << 8; }
            if (x <= 0x0FFFFFFF) { n = n + 4; x = x << 4; }
            if (x <= 0x3FFFFFFF) { n = n + 2; x = x << 2; }
            if (x <= 0x7FFFFFFF) { n = n + 1; }
            if (x == 0) return 32;
            return n;
        }

        /// <summary>
        /// Divides two FP values.
        /// </summary>
        public static int DivPrecise(int a, int b)
        {
            if (b == MinValue || b == 0)
                return 0;

            int res = (int)(((long)a << Shift) / (long)b);
            return res;
        }

        /// <summary>
        /// Calculates division approximation.
        /// </summary>
        public static int Div(int a, int b)
        {
            if (b == MinValue || b == 0)
            {
                SfUtil.ThrowArgumentException("SfMath.Div", "b", b);
                return 0;
            }

            return (int)(((long)a << 16) / b);
        }

        /// <summary>
        /// Calculates division approximation.
        /// </summary>
        public static int DivFast(int a, int b)
        {
            if (b == MinValue || b == 0)
            {
                SfUtil.ThrowArgumentException("SfMath.DivFast", "b", b);
                return 0;
            }

            // Handle negative values.
            int sign = (b < 0) ? -1 : 1;
            b *= sign;

            // Normalize input into [1.0, 2.0( range (convert to s2.30).
            int offset = 29 - Nlz((uint)b);
            int n = SfUtil.ShiftRight(b, offset - 28);
            const int ONE = (1 << 30);
            SfUtil.Assert(n >= ONE);

            // Polynomial approximation.
            int res = SfUtil.RcpPoly6(n - ONE);

            // Multiply by reciprocal, apply exponent, convert back to s16.16.
            int y = SfUtil.Qmul30(res, a);
            return SfUtil.ShiftRight(sign * y, offset - 14);
        }

        /// <summary>
        /// Calculates division approximation.
        /// </summary>
        public static int DivFastest(int a, int b)
        {
            if (b == MinValue || b == 0)
            {
                SfUtil.ThrowArgumentException("SfMath.DivFastest", "b", b);
                return 0;
            }

            // Handle negative values.
            int sign = (b < 0) ? -1 : 1;
            b *= sign;

            // Normalize input into [1.0, 2.0( range (convert to s2.30).
            int offset = 29 - Nlz((uint)b);
            int n = SfUtil.ShiftRight(b, offset - 28);
            const int ONE = (1 << 30);
            SfUtil.Assert(n >= ONE);

            // Polynomial approximation.
            int res = SfUtil.RcpPoly4(n - ONE);

            // Multiply by reciprocal, apply exponent, convert back to s16.16.
            int y = SfUtil.Qmul30(res, a);
            return SfUtil.ShiftRight(sign * y, offset - 14);
        }

        /// <summary>
        /// Divides two FP values and returns the modulus.
        /// </summary>
        public static int Mod(int a, int b)
        {
            if (b == 0)
            {
                SfUtil.ThrowArgumentException("SfMath.Mod", "b", b);
                return 0;
            }

            return a % b;
        }

        /// <summary>
        /// Calculates the square root of the given number.
        /// </summary>
        public static int SqrtPrecise(int a)
        {
            // Adapted from https://github.com/chmike/fpsqrt
            if (a <= 0)
            {
                if (a < 0)
                    SfUtil.ThrowArgumentException("SfMath.SqrtPrecise", "a", a);
                return 0;
            }

            uint r = (uint)a;
            uint b = 0x40000000;
            uint q = 0;
            while (b > 0x40)
            {
                uint t = q + b;
                if (r >= t)
                {
                    r -= t;
                    q = t + b;
                }
                r <<= 1;
                b >>= 1;
            }
            q >>= 8;
            return (int)q;
        }

        public static int Sqrt(int x)
        {
            // Return 0 for all non-positive values.
            if (x <= 0)
            {
                if (x < 0)
                    SfUtil.ThrowArgumentException("SfMath.Sqrt", "x", x);
                return 0;
            }

            // Constants (s2.30).
            const int ONE = (1 << 30);
            const int SQRT2 = 1518500249; // sqrt(2.0)

            // Normalize input into [1.0, 2.0( range (as s2.30).
            int offset = 15 - Nlz((uint)x);
            int n = SfUtil.ShiftRight(x, offset - 14);
            SfUtil.Assert(n >= ONE);
            int y = SfUtil.SqrtPoly3Lut8(n - ONE);

            // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
            int adjust = ((offset & 1) != 0) ? SQRT2 : ONE;
            offset = offset >> 1;

            // Apply exponent, convert back to s16.16.
            int yr = SfUtil.Qmul30(adjust, y);
            return SfUtil.ShiftRight(yr, 14 - offset);
        }

        public static int SqrtFast(int x)
        {
            // Return 0 for all non-positive values.
            if (x <= 0)
            {
                if (x < 0)
                    SfUtil.ThrowArgumentException("SfMath.SqrtFast", "x", x);
                return 0;
            }

            // Constants (s2.30).
            const int ONE = (1 << 30);
            const int SQRT2 = 1518500249; // sqrt(2.0)

            // Normalize input into [1.0, 2.0( range (as s2.30).
            int offset = 15 - Nlz((uint)x);
            int n = SfUtil.ShiftRight(x, offset - 14);
            SfUtil.Assert(n >= ONE);
            int y = SfUtil.SqrtPoly4(n - ONE);

            // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
            int adjust = ((offset & 1) != 0) ? SQRT2 : ONE;
            offset = offset >> 1;

            // Apply exponent, convert back to s16.16.
            int yr = SfUtil.Qmul30(adjust, y);
            return SfUtil.ShiftRight(yr, 14 - offset);
        }

        public static int SqrtFastest(int x)
        {
            // Return 0 for all non-positive values.
            if (x <= 0)
            {
                if (x < 0)
                    SfUtil.ThrowArgumentException("SfMath.SqrtFastest", "x", x);
                return 0;
            }

            // Constants (s2.30).
            const int ONE = (1 << 30);
            const int SQRT2 = 1518500249; // sqrt(2.0)

            // Normalize input into [1.0, 2.0( range (as s2.30).
            int offset = 15 - Nlz((uint)x);
            int n = SfUtil.ShiftRight(x, offset - 14);
            SfUtil.Assert(n >= ONE);
            int y = SfUtil.SqrtPoly3(n - ONE);

            // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
            int adjust = ((offset & 1) != 0) ? SQRT2 : ONE;
            offset = offset >> 1;

            // Apply exponent, convert back to s16.16.
            int yr = SfUtil.Qmul30(adjust, y);
            return SfUtil.ShiftRight(yr, 14 - offset);
        }

        /// <summary>
        /// Calculates the reciprocal square root.
        /// </summary>
        public static int RSqrt(int x)
        {
            // Return 0 for invalid values
            if (x <= 0)
            {
                SfUtil.ThrowArgumentException("SfMath.RSqrt", "x", x);
                return 0;
            }

            // Constants (s2.30).
            const int ONE = (1 << 30);
            const int HALF_SQRT2 = 759250125; // 0.5 * sqrt(2.0)

            // Normalize input into [1.0, 2.0( range (as s2.30).
            int offset = 1 - Nlz((uint)x);
            int n = SfUtil.ShiftRight(x, offset);
            SfUtil.Assert(n >= ONE);
            int y = SfUtil.RSqrtPoly3Lut16(n - ONE);

            // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
            int adjust = ((offset & 1) != 0) ? HALF_SQRT2 : ONE;
            offset = offset >> 1;

            // Apply exponent, convert back to s16.16.
            int yr = SfUtil.Qmul30(adjust, y);
            return SfUtil.ShiftRight(yr, offset + 21);
        }

        /// <summary>
        /// Calculates the reciprocal square root.
        /// </summary>
        public static int RSqrtFast(int x)
        {
            // Return 0 for invalid values
            if (x <= 0)
            {
                SfUtil.ThrowArgumentException("SfMath.RSqrtFast", "x", x);
                return 0;
            }

            // Constants (s2.30).
            const int ONE = (1 << 30);
            const int HALF_SQRT2 = 759250125; // 0.5 * sqrt(2.0)

            // Normalize input into [1.0, 2.0( range (as s2.30).
            int offset = 1 - Nlz((uint)x);
            int n = SfUtil.ShiftRight(x, offset);
            SfUtil.Assert(n >= ONE);
            int y = SfUtil.RSqrtPoly5(n - ONE);

            // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
            int adjust = ((offset & 1) != 0) ? HALF_SQRT2 : ONE;
            offset = offset >> 1;

            // Apply exponent, convert back to s16.16.
            int yr = SfUtil.Qmul30(adjust, y);
            return SfUtil.ShiftRight(yr, offset + 21);
        }

        /// <summary>
        /// Calculates the reciprocal square root.
        /// </summary>
        public static int RSqrtFastest(int x)
        {
            // Return 0 for invalid values
            if (x <= 0)
            {
                SfUtil.ThrowArgumentException("SfMath.RSqrtFastest", "x", x);
                return 0;
            }

            // Constants (s2.30).
            const int ONE = (1 << 30);
            const int HALF_SQRT2 = 759250125; // 0.5 * sqrt(2.0)

            // Normalize input into [1.0, 2.0( range (as s2.30).
            int offset = 1 - Nlz((uint)x);
            int n = SfUtil.ShiftRight(x, offset);
            SfUtil.Assert(n >= ONE);
            int y = SfUtil.RSqrtPoly3(n - ONE);

            // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
            int adjust = ((offset & 1) != 0) ? HALF_SQRT2 : ONE;
            offset = offset >> 1;

            // Apply exponent, convert back to s16.16.
            int yr = SfUtil.Qmul30(adjust, y);
            return SfUtil.ShiftRight(yr, offset + 21);
        }

        /// <summary>
        /// Calculates reciprocal approximation.
        /// </summary>
        public static int Rcp(int x)
        {
            if (x == MinValue || x == 0)
            {
                SfUtil.ThrowArgumentException("SfMath.Rcp", "x", x);
                return 0;
            }

            // Handle negative values.
            int sign = (x < 0) ? -1 : 1;
            x *= sign;

            // Normalize input into [1.0, 2.0( range (convert to s2.30).
            int offset = 29 - Nlz((uint)x);
            int n = SfUtil.ShiftRight(x, offset - 28);
            const int ONE = (1 << 30);
            SfUtil.Assert(n >= ONE);

            // Polynomial approximation.
            int res = SfUtil.RcpPoly4Lut8(n - ONE);

            // Apply exponent, convert back to s16.16.
            return SfUtil.ShiftRight(sign * res, offset);
        }

        /// <summary>
        /// Calculates reciprocal approximation.
        /// </summary>
        public static int RcpFast(int x)
        {
            if (x == MinValue || x == 0)
            {
                SfUtil.ThrowArgumentException("SfMath.RcpFast", "x", x);
                return 0;
            }

            // Handle negative values.
            int sign = (x < 0) ? -1 : 1;
            x *= sign;

            // Normalize input into [1.0, 2.0( range (convert to s2.30).
            int offset = 29 - Nlz((uint)x);
            int n = SfUtil.ShiftRight(x, offset - 28);
            const int ONE = (1 << 30);
            SfUtil.Assert(n >= ONE);

            // Polynomial approximation.
            int res = SfUtil.RcpPoly6(n - ONE);
            //int res = Util.RcpPoly3Lut8(n - ONE);

            // Apply exponent, convert back to s16.16.
            return SfUtil.ShiftRight(sign * res, offset);
        }

        /// <summary>
        /// Calculates reciprocal approximation.
        /// </summary>
        public static int RcpFastest(int x)
        {
            if (x == MinValue || x == 0)
            {
                SfUtil.ThrowArgumentException("SfMath.RcpFastest", "x", x);
                return 0;
            }

            // Handle negative values.
            int sign = (x < 0) ? -1 : 1;
            x *= sign;

            // Normalize input into [1.0, 2.0( range (convert to s2.30).
            int offset = 29 - Nlz((uint)x);
            int n = SfUtil.ShiftRight(x, offset - 28);
            const int ONE = (1 << 30);
            SfUtil.Assert(n >= ONE);

            // Polynomial approximation.
            int res = SfUtil.RcpPoly4(n - ONE);
            //int res = Util.RcpPoly3Lut4(n - ONE);

            // Apply exponent, convert back to s16.16.
            return SfUtil.ShiftRight(sign * res, offset);
        }

        /// <summary>
        /// Calculates the base 2 exponent.
        /// </summary>
        public static int Exp2(int x)
        {
            // Handle values that would under or overflow.
            if (x >= 15 * One) return MaxValue;
            if (x <= -16 * One) return 0;

            // Compute exp2 for fractional part.
            int k = (x & FractionMask) << 14;
            int y = SfUtil.Exp2Poly5(k);

            // Combine integer and fractional result, and convert back to s16.16.
            int intPart = x >> Shift;
            return SfUtil.ShiftRight(y, 14 - intPart);
        }

        /// <summary>
        /// Calculates the base 2 exponent.
        /// </summary>
        public static int Exp2Fast(int x)
        {
            // Handle values that would under or overflow.
            if (x >= 15 * One) return MaxValue;
            if (x <= -16 * One) return 0;

            // Compute exp2 for fractional part.
            int k = (x & FractionMask) << 14;
            int y = SfUtil.Exp2Poly4(k);

            // Combine integer and fractional result, and convert back to s16.16.
            int intPart = x >> Shift;
            return SfUtil.ShiftRight(y, 14 - intPart);
        }

        /// <summary>
        /// Calculates the base 2 exponent.
        /// </summary>
        public static int Exp2Fastest(int x)
        {
            // Handle values that would under or overflow.
            if (x >= 15 * One) return MaxValue;
            if (x <= -16 * One) return 0;

            // Compute exp2 for fractional part.
            int k = (x & FractionMask) << 14;
            int y = SfUtil.Exp2Poly3(k);

            // Combine integer and fractional result, and convert back to s16.16.
            int intPart = x >> Shift;
            return SfUtil.ShiftRight(y, 14 - intPart);
        }

        public static int Exp(int x)
        {
            // e^x == 2^(x / ln(2))
            return Exp2(Mul(x, RCP_LN2));
        }

        public static int ExpFast(int x)
        {
            // e^x == 2^(x / ln(2))
            return Exp2Fast(Mul(x, RCP_LN2));
        }

        public static int ExpFastest(int x)
        {
            // e^x == 2^(x / ln(2))
            return Exp2Fastest(Mul(x, RCP_LN2));
        }

        public static int Log(int x)
        {
            // Return 0 for invalid values
            if (x <= 0)
            {
                SfUtil.ThrowArgumentException("SfMath.Log", "x", x);
                return 0;
            }

            // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
            int offset = 15 - Nlz((uint)x);
            int n = SfUtil.ShiftRight(x, offset - 14);

            // Polynomial approximation.
            const int ONE = (1 << 30);
            SfUtil.Assert(n >= ONE);
            int y = SfUtil.LogPoly5Lut8(n - ONE);

            // Combine integer and fractional parts (into s16.16).
            return offset * RCP_LOG2_E + (y >> 14);
        }

        public static int LogFast(int x)
        {
            // Return 0 for invalid values
            if (x <= 0)
            {
                SfUtil.ThrowArgumentException("SfMath.LogFast", "x", x);
                return 0;
            }

            // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
            int offset = 15 - Nlz((uint)x);
            int n = SfUtil.ShiftRight(x, offset - 14);

            // Polynomial approximation.
            const int ONE = (1 << 30);
            SfUtil.Assert(n >= ONE);
            int y = SfUtil.LogPoly3Lut8(n - ONE);

            // Combine integer and fractional parts (into s16.16).
            return offset * RCP_LOG2_E + (y >> 14);
        }

        public static int LogFastest(int x)
        {
            // Return 0 for invalid values
            if (x <= 0)
            {
                SfUtil.ThrowArgumentException("SfMath.LogFastest", "x", x);
                return 0;
            }

            // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
            int offset = 15 - Nlz((uint)x);
            int n = SfUtil.ShiftRight(x, offset - 14);

            // Polynomial approximation.
            const int ONE = (1 << 30);
            SfUtil.Assert(n >= ONE);
            int y = SfUtil.LogPoly5(n - ONE);

            // Combine integer and fractional parts (into s16.16).
            return offset * RCP_LOG2_E + (y >> 14);
        }

        public static int Log2(int x)
        {
            // Return 0 for invalid values
            if (x <= 0)
            {
                SfUtil.ThrowArgumentException("SfMath.Log2", "x", x);
                return 0;
            }

            // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
            int offset = 15 - Nlz((uint)x);
            int n = SfUtil.ShiftRight(x, offset - 14);

            // Polynomial approximation of mantissa.
            const int ONE = (1 << 30);
            SfUtil.Assert(n >= ONE);
            int y = SfUtil.Log2Poly4Lut16(n - ONE);

            // Combine integer and fractional parts (into s16.16).
            return (offset << Shift) + (y >> 14);
        }

        public static int Log2Fast(int x)
        {
            // Return 0 for invalid values
            if (x <= 0)
            {
                SfUtil.ThrowArgumentException("SfMath.Log2Fast", "x", x);
                return 0;
            }

            // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
            int offset = 15 - Nlz((uint)x);
            int n = SfUtil.ShiftRight(x, offset - 14);

            // Polynomial approximation of mantissa.
            const int ONE = (1 << 30);
            SfUtil.Assert(n >= ONE);
            int y = SfUtil.Log2Poly3Lut16(n - ONE);

            // Combine integer and fractional parts (into s16.16).
            return (offset << Shift) + (y >> 14);
        }

        public static int Log2Fastest(int x)
        {
            // Return 0 for invalid values
            if (x <= 0)
            {
                SfUtil.ThrowArgumentException("SfMath.Log2Fastest", "x", x);
                return 0;
            }

            // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
            int offset = 15 - Nlz((uint)x);
            int n = SfUtil.ShiftRight(x, offset - 14);

            // Polynomial approximation of mantissa.
            const int ONE = (1 << 30);
            SfUtil.Assert(n >= ONE);
            int y = SfUtil.Log2Poly5(n - ONE);

            // Combine integer and fractional parts (into s16.16).
            return (offset << Shift) + (y >> 14);
        }

        /// <summary>
        /// Calculates x to the power of the exponent.
        /// </summary>
        public static int Pow(int x, int exponent)
        {
            // n^0 == 1
            if (exponent == 0)
                return One;

            // Return 0 for invalid values
            if (x <= 0)
            {
                if (x < 0)
                    SfUtil.ThrowArgumentException("SfMath.Pow", "x", x);
                return 0;
            }

            return Exp(Mul(exponent, Log(x)));
        }

        /// <summary>
        /// Calculates x to the power of the exponent.
        /// </summary>
        public static int PowFast(int x, int exponent)
        {
            // n^0 == 1
            if (exponent == 0)
                return One;

            // Return 0 for invalid values
            if (x <= 0)
            {
                if (x < 0)
                    SfUtil.ThrowArgumentException("SfMath.PowFast", "x", x);
                return 0;
            }

            return ExpFast(Mul(exponent, LogFast(x)));
        }

        /// <summary>
        /// Calculates x to the power of the exponent.
        /// </summary>
        public static int PowFastest(int x, int exponent)
        {
            // n^0 == 1
            if (exponent == 0)
                return One;

            // Return 0 for invalid values
            if (x <= 0)
            {
                if (x < 0)
                    SfUtil.ThrowArgumentException("SfMath.PowFastest", "x", x);
                return 0;
            }

            return ExpFastest(Mul(exponent, LogFastest(x)));
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        private static int UnitSin(int z)
        {
            // See: http://www.coranac.com/2009/07/sines/

            // Handle quadrants 1 and 2 by mirroring the [1, 3] range to [-1, 1] (by calculating 2 - z).
            // The if condition uses the fact that for the quadrants of interest are 0b01 and 0b10 (top two bits are different).
            if ((z ^ (z << 1)) < 0)
                z = (1 << 31) - z;

            // Now z is in range [-1, 1].
            const int ONE = (1 << 30);
            SfUtil.Assert((z >= -ONE) && (z <= ONE));

            // Polynomial approximation.
            int zz = SfUtil.Qmul30(z, z);
            int res = SfUtil.Qmul30(SfUtil.SinPoly4(zz), z);

            // Return as s2.30.
            return res;
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        private static int UnitSinFast(int z)
        {
            // See: http://www.coranac.com/2009/07/sines/

            // Handle quadrants 1 and 2 by mirroring the [1, 3] range to [-1, 1] (by calculating 2 - z).
            // The if condition uses the fact that for the quadrants of interest are 0b01 and 0b10 (top two bits are different).
            if ((z ^ (z << 1)) < 0)
                z = (1 << 31) - z;

            // Now z is in range [-1, 1].
            const int ONE = (1 << 30);
            SfUtil.Assert((z >= -ONE) && (z <= ONE));

            // Polynomial approximation.
            int zz = SfUtil.Qmul30(z, z);
            int res = SfUtil.Qmul30(SfUtil.SinPoly3(zz), z);

            // Return as s2.30.
            return res;
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        private static int UnitSinFastest(int z)
        {
            // See: http://www.coranac.com/2009/07/sines/

            // Handle quadrants 1 and 2 by mirroring the [1, 3] range to [-1, 1] (by calculating 2 - z).
            // The if condition uses the fact that for the quadrants of interest are 0b01 and 0b10 (top two bits are different).
            if ((z ^ (z << 1)) < 0)
                z = (1 << 31) - z;

            // Now z is in range [-1, 1].
            const int ONE = (1 << 30);
            SfUtil.Assert((z >= -ONE) && (z <= ONE));

            // Polynomial approximation.
            int zz = SfUtil.Qmul30(z, z);
            int res = SfUtil.Qmul30(SfUtil.SinPoly2(zz), z);

            // Return as s2.30.
            return res;
        }

        public static int Sin(int x)
        {
            // Map [0, 2pi] to [0, 4] (as s2.30).
            // This also wraps the values into one period.
            int z = Mul(RCP_TWO_PI, x);

            // Compute sin from s2.30 and convert back to s16.16.
            return UnitSin(z) >> 14;
        }

        public static int SinFast(int x)
        {
            // Map [0, 2pi] to [0, 4] (as s2.30).
            // This also wraps the values into one period.
            int z = Mul(RCP_TWO_PI, x);

            // Compute sin from s2.30 and convert back to s16.16.
            return UnitSinFast(z) >> 14;
        }

        public static int SinFastest(int x)
        {
            // Map [0, 2pi] to [0, 4] (as s2.30).
            // This also wraps the values into one period.
            int z = Mul(RCP_TWO_PI, x);

            // Compute sin from s2.30 and convert back to s16.16.
            return UnitSinFastest(z) >> 14;
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int Cos(int x)
        {
            return Sin(x + PiHalf);
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int CosFast(int x)
        {
            return SinFast(x + PiHalf);
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int CosFastest(int x)
        {
            return SinFastest(x + PiHalf);
        }

        public static int Tan(int x)
        {
            int z = Mul(RCP_TWO_PI, x);
            int sinX = UnitSin(z);
            int cosX = UnitSin(z + (1 << 30));
            return Div(sinX, cosX);
        }

        public static int TanFast(int x)
        {
            int z = Mul(RCP_TWO_PI, x);
            int sinX = UnitSinFast(z);
            int cosX = UnitSinFast(z + (1 << 30));
            return DivFast(sinX, cosX);
        }

        public static int TanFastest(int x)
        {
            int z = Mul(RCP_TWO_PI, x);
            int sinX = UnitSinFastest(z);
            int cosX = UnitSinFastest(z + (1 << 30));
            return DivFastest(sinX, cosX);
        }

        private static int Atan2Div(int y, int x)
        {
            SfUtil.Assert(y >= 0 && x > 0 && x >= y);

            // Normalize input into [1.0, 2.0( range (convert to s2.30).
            const int ONE = (1 << 30);
            const int HALF = (1 << 29);
            int offset = 1 - Nlz((uint)x);
            int n = SfUtil.ShiftRight(x, offset);
            SfUtil.Assert(n >= ONE);

            // Polynomial approximation of reciprocal.
            int oox = SfUtil.RcpPoly4Lut8(n - ONE);
            SfUtil.Assert(oox >= HALF && oox <= ONE);

            // Apply exponent and multiply.
            int yr = SfUtil.ShiftRight(y, offset);
            return SfUtil.Qmul30(yr, oox);
        }

        public static int Atan2(int y, int x)
        {
            // See: https://www.dsprelated.com/showarticle/1052.php

            if (x == 0)
            {
                if (y > 0) return PiHalf;
                if (y < 0) return -PiHalf;

                throw new ArgumentException($"Invalid arguments Fixed64.Atan2(): {y}, {x}", "y, x");
            }

            int nx = Abs(x);
            int ny = Abs(y);
            int negMask = ((x ^ y) >> 31);

            if (nx >= ny)
            {
                int k = Atan2Div(ny, nx);
                int z = SfUtil.AtanPoly5Lut8(k);
                int angle = (negMask ^ (z >> 14)) - negMask;
                if (x > 0) return angle;
                if (y >= 0) return angle + Pi;
                return angle - Pi;
            }
            else
            {
                int k = Atan2Div(nx, ny);
                int z = SfUtil.AtanPoly5Lut8(k);
                int angle = negMask ^ (z >> 14);
                return ((y > 0) ? PiHalf : -PiHalf) - angle;
            }
        }

        private static int Atan2DivFast(int y, int x)
        {
            SfUtil.Assert(y >= 0 && x > 0 && x >= y);

            // Normalize input into [1.0, 2.0( range (convert to s2.30).
            const int ONE = (1 << 30);
            const int HALF = (1 << 29);
            int offset = 1 - Nlz((uint)x);
            int n = SfUtil.ShiftRight(x, offset);

            // Polynomial approximation.
            int oox = SfUtil.RcpPoly6(n - ONE);
            SfUtil.Assert(oox >= HALF && oox <= ONE);

            // Apply exponent and multiply.
            int yr = SfUtil.ShiftRight(y, offset);
            return SfUtil.Qmul30(yr, oox);
        }

        public static int Atan2Fast(int y, int x)
        {
            // See: https://www.dsprelated.com/showarticle/1052.php

            if (x == 0)
            {
                if (y > 0) return PiHalf;
                if (y < 0) return -PiHalf;

                throw new ArgumentException($"Invalid arguments Fixed64.Atan2Fast(): {y}, {x}", "y, x");
            }

            int nx = Abs(x);
            int ny = Abs(y);
            int negMask = ((x ^ y) >> 31);

            if (nx >= ny)
            {
                int k = Atan2DivFast(ny, nx);
                int z = SfUtil.AtanPoly3Lut8(k);
                int angle = negMask ^ (z >> 14);
                if (x > 0) return angle;
                if (y >= 0) return angle + Pi;
                return angle - Pi;
            }
            else
            {
                int k = Atan2DivFast(nx, ny);
                int z = SfUtil.AtanPoly3Lut8(k);
                int angle = negMask ^ (z >> 14);
                return ((y > 0) ? PiHalf : -PiHalf) - angle;
            }
        }

        private static int Atan2DivFastest(int y, int x)
        {
            SfUtil.Assert(y >= 0 && x > 0 && x >= y);

            // Normalize input into [1.0, 2.0( range (convert to s2.30).
            const int ONE = (1 << 30);
            const int HALF = (1 << 29);
            int offset = 1 - Nlz((uint)x);
            int n = SfUtil.ShiftRight(x, offset);

            // Polynomial approximation.
            int oox = SfUtil.RcpPoly4(n - ONE);
            SfUtil.Assert(oox >= HALF && oox <= ONE);

            // Apply exponent and multiply.
            int yr = SfUtil.ShiftRight(y, offset);
            return SfUtil.Qmul30(yr, oox);
        }

        public static int Atan2Fastest(int y, int x)
        {
            // See: https://www.dsprelated.com/showarticle/1052.php

            if (x == 0)
            {
                if (y > 0) return PiHalf;
                if (y < 0) return -PiHalf;

                throw new ArgumentException($"Invalid arguments Fixed64.Atan2Fastest(): {y}, {x}", "y, x");

            }

            int nx = Abs(x);
            int ny = Abs(y);
            int negMask = ((x ^ y) >> 31);

            if (nx >= ny)
            {
                int k = Atan2DivFastest(ny, nx);
                int z = SfUtil.AtanPoly4(k);
                int angle = negMask ^ (z >> 14);
                if (x > 0) return angle;
                if (y >= 0) return angle + Pi;
                return angle - Pi;
            }
            else
            {
                int k = Atan2DivFastest(nx, ny);
                int z = SfUtil.AtanPoly4(k);
                int angle = negMask ^ (z >> 14);
                return ((y > 0) ? PiHalf : -PiHalf) - angle;
            }
        }

        public static int Asin(int x)
        {
            // Return 0 for invalid values
            if (x < -One || x > One)
            {
                SfUtil.ThrowArgumentException("SfMath.Asin", "x", x);
                return 0;
            }

            // Compute Atan2(x, Sqrt((1+x) * (1-x))), using s32.32.
            long xx = (long)(One + x) * (long)(One - x);
            long y = SdMath.Sqrt(xx);
            return (int)(SdMath.Atan2((long)x << 16, y) >> 16);
        }

        public static int AsinFast(int x)
        {
            // Return 0 for invalid values
            if (x < -One || x > One)
            {
                SfUtil.ThrowArgumentException("SfMath.AsinFast", "x", x);
                return 0;
            }

            // Compute Atan2(x, Sqrt((1+x) * (1-x))), using s32.32.
            long xx = (long)(One + x) * (long)(One - x);
            long y = SdMath.SqrtFast(xx);
            return (int)(SdMath.Atan2Fast((long)x << 16, y) >> 16);
        }

        public static int AsinFastest(int x)
        {
            // Return 0 for invalid values
            if (x < -One || x > One)
            {
                SfUtil.ThrowArgumentException("SfMath.AsinFastest", "x", x);
                return 0;
            }

            // Compute Atan2(x, Sqrt((1+x) * (1-x))), using s32.32.
            long xx = (long)(One + x) * (long)(One - x);
            long y = SdMath.SqrtFastest(xx);
            return (int)(SdMath.Atan2Fastest((long)x << 16, y) >> 16);
        }

        public static int Acos(int x)
        {
            // Return 0 for invalid values
            if (x < -One || x > One)
            {
                SfUtil.ThrowArgumentException("SfMath.Acos", "x", x);
                return 0;
            }

            // Compute Atan2(Sqrt((1+x) * (1-x)), x), using s32.32.
            long xx = (long)(One + x) * (long)(One - x);
            long y = SdMath.Sqrt(xx);
            return (int)(SdMath.Atan2(y, (long)x << 16) >> 16);
        }

        public static int AcosFast(int x)
        {
            // Return 0 for invalid values
            if (x < -One || x > One)
            {
                SfUtil.ThrowArgumentException("SfMath.AcosFast", "x", x);
                return 0;
            }

            // Compute Atan2(Sqrt((1+x) * (1-x)), x), using s32.32.
            long xx = (long)(One + x) * (long)(One - x);
            long y = SdMath.SqrtFast(xx);
            return (int)(SdMath.Atan2Fast(y, (long)x << 16) >> 16);
        }

        public static int AcosFastest(int x)
        {
            // Return 0 for invalid values
            if (x < -One || x > One)
            {
                SfUtil.ThrowArgumentException("SfMath.AcosFastest", "x", x);
                return 0;
            }

            // Compute Atan2(Sqrt((1+x) * (1-x)), x), using s32.32.
            long xx = (long)(One + x) * (long)(One - x);
            long y = SdMath.SqrtFastest(xx);
            return (int)(SdMath.Atan2Fastest(y, (long)x << 16) >> 16);
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int Atan(int x)
        {
            return Atan2(x, One);
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int AtanFast(int x)
        {
            return Atan2Fast(x, One);
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int AtanFastest(int x)
        {
            return Atan2Fastest(x, One);
        }
    } // Fixed32

}

// END GENERATED FILE
