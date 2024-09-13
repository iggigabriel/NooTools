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
    /// Signed 32.32 fixed point value struct.
    /// </summary>
    [Serializable]
    public struct Sdouble : IComparable<Sdouble>, IEquatable<Sdouble>, IComparable
    {
        // Constants
        public static Sdouble Neg1      { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SdMath.Neg1); } }
        public static Sdouble Zero      { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SdMath.Zero); } }
        public static Sdouble Half      { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SdMath.Half); } }
        public static Sdouble One       { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SdMath.One); } }
        public static Sdouble Two       { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SdMath.Two); } }
        public static Sdouble Pi        { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SdMath.Pi); } }
        public static Sdouble Pi2       { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SdMath.Pi2); } }
        public static Sdouble PiHalf    { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SdMath.PiHalf); } }
        public static Sdouble E         { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SdMath.E); } }

        public static Sdouble MinValue  { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SdMath.MinValue); } }
        public static Sdouble MaxValue  { [MethodImpl(SfUtil.AggressiveInlining)] get { return FromRaw(SdMath.MaxValue); } }

        // Raw fixed point value
        public long Raw;

        // Construction
        [MethodImpl(SfUtil.AggressiveInlining)] public static Sdouble FromRaw(long raw) { Sdouble v; v.Raw = raw; return v; }
        [MethodImpl(SfUtil.AggressiveInlining)] public static Sdouble FromInt(int v) { return FromRaw(SdMath.FromInt(v)); }
        [MethodImpl(SfUtil.AggressiveInlining)] public static Sdouble FromFloat(float v) { return FromRaw(SdMath.FromFloat(v)); }
        [MethodImpl(SfUtil.AggressiveInlining)] public static Sdouble FromDouble(double v) { return FromRaw(SdMath.FromDouble(v)); }
        [MethodImpl(SfUtil.AggressiveInlining)] public static Sdouble FromSfloat(Sfloat v) { return FromRaw((long)v.Raw << 16); }

        // Conversions
        public static int FloorToInt(Sdouble a) { return SdMath.FloorToInt(a.Raw); }
        public static int CeilToInt(Sdouble a) { return SdMath.CeilToInt(a.Raw); }
        public static int RoundToInt(Sdouble a) { return SdMath.RoundToInt(a.Raw); }
        public readonly float Float { [MethodImpl(SfUtil.AggressiveInlining)] get { return SdMath.ToFloat(Raw); } }
        public readonly double Double { [MethodImpl(SfUtil.AggressiveInlining)] get { return SdMath.ToDouble(Raw); } }
        public readonly Sfloat Sfloat { [MethodImpl(SfUtil.AggressiveInlining)] get { return Sfloat.FromRaw((int)(Raw >> 16)); } }

        // Creates the fixed point number that's a divided by b.
        public static Sdouble Ratio(int a, int b) { return Sdouble.FromRaw(((long)a << 32) / b); }
        // Creates the fixed point number that's a divided by 10.
        public static Sdouble Ratio10(int a) { return Sdouble.FromRaw(((long)a << 32) / 10); }
        // Creates the fixed point number that's a divided by 100.
        public static Sdouble Ratio100(int a) { return Sdouble.FromRaw(((long)a << 32) / 100); }
        // Creates the fixed point number that's a divided by 1000.
        public static Sdouble Ratio1000(int a) { return Sdouble.FromRaw(((long)a << 32) / 1000); }

        // Operators
        public static Sdouble operator -(Sdouble v1) { return FromRaw(-v1.Raw); }

        public static Sdouble operator +(Sdouble v1, Sdouble v2) { return FromRaw(v1.Raw + v2.Raw); }
        public static Sdouble operator -(Sdouble v1, Sdouble v2) { return FromRaw(v1.Raw - v2.Raw); }
        public static Sdouble operator *(Sdouble v1, Sdouble v2) { return FromRaw(SdMath.Mul(v1.Raw, v2.Raw)); }
        public static Sdouble operator /(Sdouble v1, Sdouble v2) { return FromRaw(SdMath.DivPrecise(v1.Raw, v2.Raw)); }
        public static Sdouble operator %(Sdouble v1, Sdouble v2) { return FromRaw(SdMath.Mod(v1.Raw, v2.Raw)); }

        public static Sdouble operator +(Sdouble v1, int v2) { return FromRaw(v1.Raw + SdMath.FromInt(v2)); }
        public static Sdouble operator +(int v1, Sdouble v2) { return FromRaw(SdMath.FromInt(v1) + v2.Raw); }
        public static Sdouble operator -(Sdouble v1, int v2) { return FromRaw(v1.Raw - SdMath.FromInt(v2)); }
        public static Sdouble operator -(int v1, Sdouble v2) { return FromRaw(SdMath.FromInt(v1) - v2.Raw); }
        public static Sdouble operator *(Sdouble v1, int v2) { return FromRaw(v1.Raw * (long)v2); }
        public static Sdouble operator *(int v1, Sdouble v2) { return FromRaw((long)v1 * v2.Raw); }
        public static Sdouble operator /(Sdouble v1, int v2) { return FromRaw(v1.Raw / (long)v2); }
        public static Sdouble operator /(int v1, Sdouble v2) { return FromRaw(SdMath.DivPrecise(SdMath.FromInt(v1), v2.Raw)); }
        public static Sdouble operator %(Sdouble v1, int v2) { return FromRaw(SdMath.Mod(v1.Raw, SdMath.FromInt(v2))); }
        public static Sdouble operator %(int v1, Sdouble v2) { return FromRaw(SdMath.Mod(SdMath.FromInt(v1), v2.Raw)); }

        public static Sdouble operator ++(Sdouble v1) { return FromRaw(v1.Raw + SdMath.One); }
        public static Sdouble operator --(Sdouble v1) { return FromRaw(v1.Raw - SdMath.One); }

        public static bool operator ==(Sdouble v1, Sdouble v2) { return v1.Raw == v2.Raw; }
        public static bool operator !=(Sdouble v1, Sdouble v2) { return v1.Raw != v2.Raw; }
        public static bool operator <(Sdouble v1, Sdouble v2) { return v1.Raw < v2.Raw; }
        public static bool operator <=(Sdouble v1, Sdouble v2) { return v1.Raw <= v2.Raw; }
        public static bool operator >(Sdouble v1, Sdouble v2) { return v1.Raw > v2.Raw; }
        public static bool operator >=(Sdouble v1, Sdouble v2) { return v1.Raw >= v2.Raw; }

        public static bool operator ==(int v1, Sdouble v2) { return SdMath.FromInt(v1) == v2.Raw; }
        public static bool operator ==(Sdouble v1, int v2) { return v1.Raw == SdMath.FromInt(v2); }
        public static bool operator !=(int v1, Sdouble v2) { return SdMath.FromInt(v1) != v2.Raw; }
        public static bool operator !=(Sdouble v1, int v2) { return v1.Raw != SdMath.FromInt(v2); }
        public static bool operator <(int v1, Sdouble v2) { return SdMath.FromInt(v1) < v2.Raw; }
        public static bool operator <(Sdouble v1, int v2) { return v1.Raw < SdMath.FromInt(v2); }
        public static bool operator <=(int v1, Sdouble v2) { return SdMath.FromInt(v1) <= v2.Raw; }
        public static bool operator <=(Sdouble v1, int v2) { return v1.Raw <= SdMath.FromInt(v2); }
        public static bool operator >(int v1, Sdouble v2) { return SdMath.FromInt(v1) > v2.Raw; }
        public static bool operator >(Sdouble v1, int v2) { return v1.Raw > SdMath.FromInt(v2); }
        public static bool operator >=(int v1, Sdouble v2) { return SdMath.FromInt(v1) >= v2.Raw; }
        public static bool operator >=(Sdouble v1, int v2) { return v1.Raw >= SdMath.FromInt(v2); }

        public static bool operator ==(Sfloat a, Sdouble b) { return Sdouble.FromSfloat(a) == b; }
        public static bool operator ==(Sdouble a, Sfloat b) { return a == Sdouble.FromSfloat(b); }
        public static bool operator !=(Sfloat a, Sdouble b) { return Sdouble.FromSfloat(a) != b; }
        public static bool operator !=(Sdouble a, Sfloat b) { return a != Sdouble.FromSfloat(b); }
        public static bool operator <(Sfloat a, Sdouble b) { return Sdouble.FromSfloat(a) < b; }
        public static bool operator <(Sdouble a, Sfloat b) { return a < Sdouble.FromSfloat(b); }
        public static bool operator <=(Sfloat a, Sdouble b) { return Sdouble.FromSfloat(a) <= b; }
        public static bool operator <=(Sdouble a, Sfloat b) { return a <= Sdouble.FromSfloat(b); }
        public static bool operator >(Sfloat a, Sdouble b) { return Sdouble.FromSfloat(a) > b; }
        public static bool operator >(Sdouble a, Sfloat b) { return a > Sdouble.FromSfloat(b); }
        public static bool operator >=(Sfloat a, Sdouble b) { return Sdouble.FromSfloat(a) >= b; }
        public static bool operator >=(Sdouble a, Sfloat b) { return a >= Sdouble.FromSfloat(b); }

        public static Sdouble RadToDeg(Sdouble a) { return FromRaw(SdMath.Mul(a.Raw, 246083499198)); } // 180 / sd.Pi
        public static Sdouble DegToRad(Sdouble a) { return FromRaw(SdMath.Mul(a.Raw, 74961320)); }     // sd.Pi / 180

        public static Sdouble Div2(Sdouble a) { return FromRaw(a.Raw >> 1); }
        public static Sdouble Abs(Sdouble a) { return FromRaw(SdMath.Abs(a.Raw)); }
        public static Sdouble Nabs(Sdouble a) { return FromRaw(SdMath.Nabs(a.Raw)); }
        public static int Sign(Sdouble a) { return SdMath.Sign(a.Raw); }
        public static Sdouble Ceil(Sdouble a) { return FromRaw(SdMath.Ceil(a.Raw)); }
        public static Sdouble Floor(Sdouble a) { return FromRaw(SdMath.Floor(a.Raw)); }
        public static Sdouble Round(Sdouble a) { return FromRaw(SdMath.Round(a.Raw)); }
        public static Sdouble Fract(Sdouble a) { return FromRaw(SdMath.Fract(a.Raw)); }
        public static Sdouble Div(Sdouble a, Sdouble b) { return FromRaw(SdMath.Div(a.Raw, b.Raw)); }
        public static Sdouble DivFast(Sdouble a, Sdouble b) { return FromRaw(SdMath.DivFast(a.Raw, b.Raw)); }
        public static Sdouble DivFastest(Sdouble a, Sdouble b) { return FromRaw(SdMath.DivFastest(a.Raw, b.Raw)); }
        public static Sdouble SqrtPrecise(Sdouble a) { return FromRaw(SdMath.SqrtPrecise(a.Raw)); }
        public static Sdouble Sqrt(Sdouble a) { return FromRaw(SdMath.Sqrt(a.Raw)); }
        public static Sdouble SqrtFast(Sdouble a) { return FromRaw(SdMath.SqrtFast(a.Raw)); }
        public static Sdouble SqrtFastest(Sdouble a) { return FromRaw(SdMath.SqrtFastest(a.Raw)); }
        public static Sdouble RSqrt(Sdouble a) { return FromRaw(SdMath.RSqrt(a.Raw)); }
        public static Sdouble RSqrtFast(Sdouble a) { return FromRaw(SdMath.RSqrtFast(a.Raw)); }
        public static Sdouble RSqrtFastest(Sdouble a) { return FromRaw(SdMath.RSqrtFastest(a.Raw)); }
        public static Sdouble Rcp(Sdouble a) { return FromRaw(SdMath.Rcp(a.Raw)); }
        public static Sdouble RcpFast(Sdouble a) { return FromRaw(SdMath.RcpFast(a.Raw)); }
        public static Sdouble RcpFastest(Sdouble a) { return FromRaw(SdMath.RcpFastest(a.Raw)); }
        public static Sdouble Exp(Sdouble a) { return FromRaw(SdMath.Exp(a.Raw)); }
        public static Sdouble ExpFast(Sdouble a) { return FromRaw(SdMath.ExpFast(a.Raw)); }
        public static Sdouble ExpFastest(Sdouble a) { return FromRaw(SdMath.ExpFastest(a.Raw)); }
        public static Sdouble Exp2(Sdouble a) { return FromRaw(SdMath.Exp2(a.Raw)); }
        public static Sdouble Exp2Fast(Sdouble a) { return FromRaw(SdMath.Exp2Fast(a.Raw)); }
        public static Sdouble Exp2Fastest(Sdouble a) { return FromRaw(SdMath.Exp2Fastest(a.Raw)); }
        public static Sdouble Log(Sdouble a) { return FromRaw(SdMath.Log(a.Raw)); }
        public static Sdouble LogFast(Sdouble a) { return FromRaw(SdMath.LogFast(a.Raw)); }
        public static Sdouble LogFastest(Sdouble a) { return FromRaw(SdMath.LogFastest(a.Raw)); }
        public static Sdouble Log2(Sdouble a) { return FromRaw(SdMath.Log2(a.Raw)); }
        public static Sdouble Log2Fast(Sdouble a) { return FromRaw(SdMath.Log2Fast(a.Raw)); }
        public static Sdouble Log2Fastest(Sdouble a) { return FromRaw(SdMath.Log2Fastest(a.Raw)); }

        public static Sdouble Sin(Sdouble a) { return FromRaw(SdMath.Sin(a.Raw)); }
        public static Sdouble SinFast(Sdouble a) { return FromRaw(SdMath.SinFast(a.Raw)); }
        public static Sdouble SinFastest(Sdouble a) { return FromRaw(SdMath.SinFastest(a.Raw)); }
        public static Sdouble Cos(Sdouble a) { return FromRaw(SdMath.Cos(a.Raw)); }
        public static Sdouble CosFast(Sdouble a) { return FromRaw(SdMath.CosFast(a.Raw)); }
        public static Sdouble CosFastest(Sdouble a) { return FromRaw(SdMath.CosFastest(a.Raw)); }
        public static Sdouble Tan(Sdouble a) { return FromRaw(SdMath.Tan(a.Raw)); }
        public static Sdouble TanFast(Sdouble a) { return FromRaw(SdMath.TanFast(a.Raw)); }
        public static Sdouble TanFastest(Sdouble a) { return FromRaw(SdMath.TanFastest(a.Raw)); }
        public static Sdouble Asin(Sdouble a) { return FromRaw(SdMath.Asin(a.Raw)); }
        public static Sdouble AsinFast(Sdouble a) { return FromRaw(SdMath.AsinFast(a.Raw)); }
        public static Sdouble AsinFastest(Sdouble a) { return FromRaw(SdMath.AsinFastest(a.Raw)); }
        public static Sdouble Acos(Sdouble a) { return FromRaw(SdMath.Acos(a.Raw)); }
        public static Sdouble AcosFast(Sdouble a) { return FromRaw(SdMath.AcosFast(a.Raw)); }
        public static Sdouble AcosFastest(Sdouble a) { return FromRaw(SdMath.AcosFastest(a.Raw)); }
        public static Sdouble Atan(Sdouble a) { return FromRaw(SdMath.Atan(a.Raw)); }
        public static Sdouble AtanFast(Sdouble a) { return FromRaw(SdMath.AtanFast(a.Raw)); }
        public static Sdouble AtanFastest(Sdouble a) { return FromRaw(SdMath.AtanFastest(a.Raw)); }
        public static Sdouble Atan2(Sdouble y, Sdouble x) { return FromRaw(SdMath.Atan2(y.Raw, x.Raw)); }
        public static Sdouble Atan2Fast(Sdouble y, Sdouble x) { return FromRaw(SdMath.Atan2Fast(y.Raw, x.Raw)); }
        public static Sdouble Atan2Fastest(Sdouble y, Sdouble x) { return FromRaw(SdMath.Atan2Fastest(y.Raw, x.Raw)); }
        public static Sdouble Pow(Sdouble a, Sdouble b) { return FromRaw(SdMath.Pow(a.Raw, b.Raw)); }
        public static Sdouble PowFast(Sdouble a, Sdouble b) { return FromRaw(SdMath.PowFast(a.Raw, b.Raw)); }
        public static Sdouble PowFastest(Sdouble a, Sdouble b) { return FromRaw(SdMath.PowFastest(a.Raw, b.Raw)); }

        public static Sdouble Min(Sdouble a, Sdouble b) { return FromRaw(SdMath.Min(a.Raw, b.Raw)); }
        public static Sdouble Max(Sdouble a, Sdouble b) { return FromRaw(SdMath.Max(a.Raw, b.Raw)); }
        public static Sdouble Clamp(Sdouble a, Sdouble min, Sdouble max) { return FromRaw(SdMath.Clamp(a.Raw, min.Raw, max.Raw)); }
        public static Sdouble Clamp01(Sdouble a) { return FromRaw(SdMath.Clamp(a.Raw, SdMath.Zero, SdMath.One)); }

        public static Sdouble Lerp(Sdouble a, Sdouble b, Sdouble t)
        {
            long tb = t.Raw;
            long ta = SdMath.One - tb;
            return FromRaw(SdMath.Mul(a.Raw, ta) + SdMath.Mul(b.Raw, tb));
        }

        public bool Equals(Sdouble other)
        {
            return (Raw == other.Raw);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Sdouble))
                return false;
            return ((Sdouble)obj).Raw == Raw;
        }

        public int CompareTo(Sdouble other)
        {
            if (Raw < other.Raw) return -1;
            if (Raw > other.Raw) return +1;
            return 0;
        }

        public override string ToString()
        {
            return SdMath.ToString(Raw);
        }

        public override int GetHashCode()
        {
            return Raw.GetHashCode();
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj is Sdouble other)
                return CompareTo(other);
            else if (obj is null)
                return 1;
            // don't allow comparisons with other numeric or non-numeric types.
            throw new ArgumentException("sd can only be compared against another sd.");
        }
    }
}
