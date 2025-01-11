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

using System;
using System.Runtime.CompilerServices;

namespace Noo.Tools
{
    /// <summary>
    /// Signed 16.16 fixed point value struct.
    /// </summary>
    [Serializable]
    public struct Sfloat : IComparable<Sfloat>, IEquatable<Sfloat>, IComparable
    {
        // Constants
        public static Sfloat NegOne { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SfMath.NegOne); } }
        public static Sfloat NegHalf { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SfMath.NegHalf); } }
        public static Sfloat Zero { [MethodImpl(SfUtil.AggressiveInlining)] get { return default; } }
        public static Sfloat Half { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SfMath.Half); } }
        public static Sfloat One { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SfMath.One); } }
        public static Sfloat Two { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SfMath.Two); } }
        public static Sfloat Pi { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SfMath.Pi); } }
        public static Sfloat Pi2 { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SfMath.Pi2); } }
        public static Sfloat PiHalf { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SfMath.PiHalf); } }
        public static Sfloat E { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SfMath.E); } }

        public static Sfloat MinValue { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SfMath.MinValue); } }
        public static Sfloat MaxValue { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SfMath.MaxValue); } }

        // Raw fixed point value
        public int Raw;

        public Sfloat(int raw)
        {
            Raw = raw;
        }

        [MethodImpl(SfUtil.AggressiveInlining)] public readonly bool IsZero() => Raw == 0;

        // Construction
        [MethodImpl(SfUtil.AggressiveInlining)] public static Sfloat FromRaw(int raw) { return new Sfloat(raw); }
        [MethodImpl(SfUtil.AggressiveInlining)] public static Sfloat FromInt(int v) { return FromRaw(SfMath.FromInt(v)); }
        [MethodImpl(SfUtil.AggressiveInlining)] public static Sfloat FromFloat(float v) { return FromRaw(SfMath.FromFloat(v)); }
        [MethodImpl(SfUtil.AggressiveInlining)] public static Sfloat FromDouble(double v) { return FromRaw(SfMath.FromDouble(v)); }
        [MethodImpl(SfUtil.AggressiveInlining)] public static Sfloat FromSd(Sdouble v) { return FromRaw((int)(v.Raw >> 16)); }

        // Conversions
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int FloorToInt(Sfloat a) { return SfMath.FloorToInt(a.Raw); }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int CeilToInt(Sfloat a) { return SfMath.CeilToInt(a.Raw); }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int RoundToInt(Sfloat a) { return SfMath.RoundToInt(a.Raw); }

        public readonly float Float { [MethodImpl(SfUtil.AggressiveInlining)] get { return SfMath.ToFloat(Raw); } }
        public readonly double Double { [MethodImpl(SfUtil.AggressiveInlining)] get { return SfMath.ToDouble(Raw); } }

        // Creates the fixed point number that's a divided by b.
        public static Sfloat Ratio(int a, int b) { return Sfloat.FromRaw((int)(((long)a << 16) / b)); }
        // Creates the fixed point number that's a divided by 10.
        public static Sfloat Ratio10(int a) { return Sfloat.FromRaw((int)(((long)a << 16) / 10)); }
        // Creates the fixed point number that's a divided by 100.
        public static Sfloat Ratio100(int a) { return Sfloat.FromRaw((int)(((long)a << 16) / 100)); }
        // Creates the fixed point number that's a divided by 1000.
        public static Sfloat Ratio1000(int a) { return Sfloat.FromRaw((int)(((long)a << 16) / 1000)); }

        public static Sfloat Ratio10000(int a) { return Sfloat.FromRaw((int)(((long)a << 16) / 10000)); }

        // Operators
        public static Sfloat operator -(Sfloat v1) { return FromRaw(-v1.Raw); }

        //public static sf operator +(sf v1, sf v2) { sf r; r.raw = v1.raw + v2.raw; return r; }
        public static Sfloat operator +(Sfloat v1, Sfloat v2) { return FromRaw(v1.Raw + v2.Raw); }
        public static Sfloat operator -(Sfloat v1, Sfloat v2) { return FromRaw(v1.Raw - v2.Raw); }
        public static Sfloat operator *(Sfloat v1, Sfloat v2) { return FromRaw(SfMath.Mul(v1.Raw, v2.Raw)); }
        public static Sfloat operator /(Sfloat v1, Sfloat v2) { return FromRaw(SfMath.DivPrecise(v1.Raw, v2.Raw)); }
        public static Sfloat operator %(Sfloat v1, Sfloat v2) { return FromRaw(SfMath.Mod(v1.Raw, v2.Raw)); }

        public static Sfloat operator +(Sfloat v1, int v2) { return FromRaw(v1.Raw + SfMath.FromInt(v2)); }
        public static Sfloat operator +(int v1, Sfloat v2) { return FromRaw(SfMath.FromInt(v1) + v2.Raw); }
        public static Sfloat operator -(Sfloat v1, int v2) { return FromRaw(v1.Raw - SfMath.FromInt(v2)); }
        public static Sfloat operator -(int v1, Sfloat v2) { return FromRaw(SfMath.FromInt(v1) - v2.Raw); }
        public static Sfloat operator *(Sfloat v1, int v2) { return FromRaw(v1.Raw * (int)v2); }
        public static Sfloat operator *(int v1, Sfloat v2) { return FromRaw((int)v1 * v2.Raw); }
        public static Sfloat operator /(Sfloat v1, int v2) { return FromRaw(v1.Raw / (int)v2); }
        public static Sfloat operator /(int v1, Sfloat v2) { return FromRaw(SfMath.DivPrecise(SfMath.FromInt(v1), v2.Raw)); }
        public static Sfloat operator %(Sfloat v1, int v2) { return FromRaw(SfMath.Mod(v1.Raw, SfMath.FromInt(v2))); }
        public static Sfloat operator %(int v1, Sfloat v2) { return FromRaw(SfMath.Mod(SfMath.FromInt(v1), v2.Raw)); }

        public static Sfloat operator ++(Sfloat v1) { return FromRaw(v1.Raw + SfMath.One); }
        public static Sfloat operator --(Sfloat v1) { return FromRaw(v1.Raw - SfMath.One); }

        public static bool operator ==(Sfloat v1, Sfloat v2) { return v1.Raw == v2.Raw; }
        public static bool operator !=(Sfloat v1, Sfloat v2) { return v1.Raw != v2.Raw; }
        public static bool operator <(Sfloat v1, Sfloat v2) { return v1.Raw < v2.Raw; }
        public static bool operator <=(Sfloat v1, Sfloat v2) { return v1.Raw <= v2.Raw; }
        public static bool operator >(Sfloat v1, Sfloat v2) { return v1.Raw > v2.Raw; }
        public static bool operator >=(Sfloat v1, Sfloat v2) { return v1.Raw >= v2.Raw; }

        public static bool operator ==(int v1, Sfloat v2) { return SfMath.FromInt(v1) == v2.Raw; }
        public static bool operator ==(Sfloat v1, int v2) { return v1.Raw == SfMath.FromInt(v2); }
        public static bool operator !=(int v1, Sfloat v2) { return SfMath.FromInt(v1) != v2.Raw; }
        public static bool operator !=(Sfloat v1, int v2) { return v1.Raw != SfMath.FromInt(v2); }
        public static bool operator <(int v1, Sfloat v2) { return SfMath.FromInt(v1) < v2.Raw; }
        public static bool operator <(Sfloat v1, int v2) { return v1.Raw < SfMath.FromInt(v2); }
        public static bool operator <=(int v1, Sfloat v2) { return SfMath.FromInt(v1) <= v2.Raw; }
        public static bool operator <=(Sfloat v1, int v2) { return v1.Raw <= SfMath.FromInt(v2); }
        public static bool operator >(int v1, Sfloat v2) { return SfMath.FromInt(v1) > v2.Raw; }
        public static bool operator >(Sfloat v1, int v2) { return v1.Raw > SfMath.FromInt(v2); }
        public static bool operator >=(int v1, Sfloat v2) { return SfMath.FromInt(v1) >= v2.Raw; }
        public static bool operator >=(Sfloat v1, int v2) { return v1.Raw >= SfMath.FromInt(v2); }

        public static Sfloat operator >>(Sfloat a, int b) { return FromRaw(a.Raw >> b); }
        public static Sfloat operator <<(Sfloat a, int b) { return FromRaw(a.Raw << b); }

        public static Sfloat RadToDeg(Sfloat a) { return FromRaw(SfMath.Mul(a.Raw, 3754943)); }  // 180 / sf.Pi
        public static Sfloat DegToRad(Sfloat a) { return FromRaw(SfMath.Mul(a.Raw, 1143)); }     // sf.Pi / 180

        public static Sfloat Div2(Sfloat a) { return FromRaw(a.Raw >> 1); }

        /// <summary>
        /// Returns the absolute (positive) value of x.
        /// Fails with MinValue
        /// </summary>
        public static Sfloat Abs(Sfloat a) { return FromRaw(SfMath.Abs(a.Raw)); }
        public static Sfloat Nabs(Sfloat a) { return FromRaw(SfMath.Nabs(a.Raw)); }
        public static int Sign(Sfloat a) { return SfMath.Sign(a.Raw); }
        public static Sfloat Ceil(Sfloat a) { return FromRaw(SfMath.Ceil(a.Raw)); }
        public static Sfloat Floor(Sfloat a) { return FromRaw(SfMath.Floor(a.Raw)); }
        public static Sfloat Round(Sfloat a) { return FromRaw(SfMath.Round(a.Raw)); }
        public static Sfloat Fract(Sfloat a) { return FromRaw(SfMath.Fract(a.Raw)); }
        public static Sfloat Div(Sfloat a, Sfloat b) { return FromRaw(SfMath.Div(a.Raw, b.Raw)); }
        public static Sfloat DivFast(Sfloat a, Sfloat b) { return FromRaw(SfMath.DivFast(a.Raw, b.Raw)); }
        public static Sfloat DivFastest(Sfloat a, Sfloat b) { return FromRaw(SfMath.DivFastest(a.Raw, b.Raw)); }
        public static Sfloat SqrtPrecise(Sfloat a) { return FromRaw(SfMath.SqrtPrecise(a.Raw)); }
        public static Sfloat Sqrt(Sfloat a) { return FromRaw(SfMath.Sqrt(a.Raw)); }
        public static Sfloat SqrtFast(Sfloat a) { return FromRaw(SfMath.SqrtFast(a.Raw)); }
        public static Sfloat SqrtFastest(Sfloat a) { return FromRaw(SfMath.SqrtFastest(a.Raw)); }
        public static Sfloat RSqrt(Sfloat a) { return FromRaw(SfMath.RSqrt(a.Raw)); }
        public static Sfloat RSqrtFast(Sfloat a) { return FromRaw(SfMath.RSqrtFast(a.Raw)); }
        public static Sfloat RSqrtFastest(Sfloat a) { return FromRaw(SfMath.RSqrtFastest(a.Raw)); }
        public static Sfloat Rcp(Sfloat a) { return FromRaw(SfMath.Rcp(a.Raw)); }
        public static Sfloat RcpFast(Sfloat a) { return FromRaw(SfMath.RcpFast(a.Raw)); }
        public static Sfloat RcpFastest(Sfloat a) { return FromRaw(SfMath.RcpFastest(a.Raw)); }
        public static Sfloat Exp(Sfloat a) { return FromRaw(SfMath.Exp(a.Raw)); }
        public static Sfloat ExpFast(Sfloat a) { return FromRaw(SfMath.ExpFast(a.Raw)); }
        public static Sfloat ExpFastest(Sfloat a) { return FromRaw(SfMath.ExpFastest(a.Raw)); }
        public static Sfloat Exp2(Sfloat a) { return FromRaw(SfMath.Exp2(a.Raw)); }
        public static Sfloat Exp2Fast(Sfloat a) { return FromRaw(SfMath.Exp2Fast(a.Raw)); }
        public static Sfloat Exp2Fastest(Sfloat a) { return FromRaw(SfMath.Exp2Fastest(a.Raw)); }
        public static Sfloat Log(Sfloat a) { return FromRaw(SfMath.Log(a.Raw)); }
        public static Sfloat LogFast(Sfloat a) { return FromRaw(SfMath.LogFast(a.Raw)); }
        public static Sfloat LogFastest(Sfloat a) { return FromRaw(SfMath.LogFastest(a.Raw)); }
        public static Sfloat Log2(Sfloat a) { return FromRaw(SfMath.Log2(a.Raw)); }
        public static Sfloat Log2Fast(Sfloat a) { return FromRaw(SfMath.Log2Fast(a.Raw)); }
        public static Sfloat Log2Fastest(Sfloat a) { return FromRaw(SfMath.Log2Fastest(a.Raw)); }

        public static Sfloat Sin(Sfloat a) { return FromRaw(SfMath.Sin(a.Raw)); }
        public static Sfloat SinFast(Sfloat a) { return FromRaw(SfMath.SinFast(a.Raw)); }
        public static Sfloat SinFastest(Sfloat a) { return FromRaw(SfMath.SinFastest(a.Raw)); }
        public static Sfloat Cos(Sfloat a) { return FromRaw(SfMath.Cos(a.Raw)); }
        public static Sfloat CosFast(Sfloat a) { return FromRaw(SfMath.CosFast(a.Raw)); }
        public static Sfloat CosFastest(Sfloat a) { return FromRaw(SfMath.CosFastest(a.Raw)); }
        public static Sfloat Tan(Sfloat a) { return FromRaw(SfMath.Tan(a.Raw)); }
        public static Sfloat TanFast(Sfloat a) { return FromRaw(SfMath.TanFast(a.Raw)); }
        public static Sfloat TanFastest(Sfloat a) { return FromRaw(SfMath.TanFastest(a.Raw)); }
        public static Sfloat Asin(Sfloat a) { return FromRaw(SfMath.Asin(a.Raw)); }
        public static Sfloat AsinFast(Sfloat a) { return FromRaw(SfMath.AsinFast(a.Raw)); }
        public static Sfloat AsinFastest(Sfloat a) { return FromRaw(SfMath.AsinFastest(a.Raw)); }
        public static Sfloat Acos(Sfloat a) { return FromRaw(SfMath.Acos(a.Raw)); }
        public static Sfloat AcosFast(Sfloat a) { return FromRaw(SfMath.AcosFast(a.Raw)); }
        public static Sfloat AcosFastest(Sfloat a) { return FromRaw(SfMath.AcosFastest(a.Raw)); }
        public static Sfloat Atan(Sfloat a) { return FromRaw(SfMath.Atan(a.Raw)); }
        public static Sfloat AtanFast(Sfloat a) { return FromRaw(SfMath.AtanFast(a.Raw)); }
        public static Sfloat AtanFastest(Sfloat a) { return FromRaw(SfMath.AtanFastest(a.Raw)); }
        public static Sfloat Atan2(Sfloat y, Sfloat x) { return FromRaw(SfMath.Atan2(y.Raw, x.Raw)); }
        public static Sfloat Atan2Fast(Sfloat y, Sfloat x) { return FromRaw(SfMath.Atan2Fast(y.Raw, x.Raw)); }
        public static Sfloat Atan2Fastest(Sfloat y, Sfloat x) { return FromRaw(SfMath.Atan2Fastest(y.Raw, x.Raw)); }
        public static Sfloat Pow(Sfloat a, Sfloat b) { return FromRaw(SfMath.Pow(a.Raw, b.Raw)); }
        public static Sfloat PowFast(Sfloat a, Sfloat b) { return FromRaw(SfMath.PowFast(a.Raw, b.Raw)); }
        public static Sfloat PowFastest(Sfloat a, Sfloat b) { return FromRaw(SfMath.PowFastest(a.Raw, b.Raw)); }

        public static Sfloat Min(Sfloat a, Sfloat b) { return FromRaw(SfMath.Min(a.Raw, b.Raw)); }
        public static Sfloat Max(Sfloat a, Sfloat b) { return FromRaw(SfMath.Max(a.Raw, b.Raw)); }
        public static Sfloat Clamp(Sfloat a, Sfloat min, Sfloat max) { return FromRaw(SfMath.Clamp(a.Raw, min.Raw, max.Raw)); }
        public static Sfloat Clamp01(Sfloat a) { return FromRaw(SfMath.Clamp(a.Raw, SfMath.Zero, SfMath.One)); }

        public static Sfloat Lerp(Sfloat a, Sfloat b, Sfloat t)
        {
            int tb = t.Raw;
            int ta = SfMath.One - tb;
            return FromRaw(SfMath.Mul(a.Raw, ta) + SfMath.Mul(b.Raw, tb));
        }

        public static Sfloat LerpInverse(Sfloat a, Sfloat b, Sfloat t)
        {
            if (a == b) return Zero;
            return (t - a) / (b - a);
        }

        public static Sfloat Remap(Sfloat value, Sfloat fromMin, Sfloat fromMax, Sfloat toMin, Sfloat toMax)
        {
            return Lerp(toMin, toMax, LerpInverse(fromMin, fromMax, value));
        }

        public static Sfloat Remap(Sfloat value, Sfloat fromMin, Sfloat fromMid, Sfloat fromMax, Sfloat toMin, Sfloat toMid, Sfloat toMax)
        {
            if (value < fromMid) return Remap(value, fromMin, fromMid, toMin, toMid);
            else return Remap(value, fromMid, fromMax, toMid, toMax);
        }

        public readonly bool Equals(Sfloat other)
        {
            return (Raw == other.Raw);
        }

        public override readonly bool Equals(object obj)
        {
            if (obj is not Sfloat)
                return false;
            return ((Sfloat)obj).Raw == Raw;
        }

        public readonly int CompareTo(Sfloat other)
        {
            if (Raw < other.Raw) return -1;
            if (Raw > other.Raw) return +1;
            return 0;
        }

        public readonly override string ToString()
        {
            return SfMath.ToString(Raw);
        }

        public readonly string ToString(string format)
        {
            return SfMath.ToString(Raw, format);
        }

        public readonly override int GetHashCode()
        {
            return Raw;
        }

        readonly int IComparable.CompareTo(object obj)
        {
            if (obj is Sfloat other)
                return CompareTo(other);
            else if (obj is null)
                return 1;
            // don't allow comparisons with other numeric or non-numeric types.
            throw new ArgumentException("sf can only be compared against another sf.");
        }
    }
}
