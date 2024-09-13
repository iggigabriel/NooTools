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
    /// Vector2 struct with signed 32.32 fixed point components.
    /// </summary>
    [Serializable]
    public struct Sdouble2 : IEquatable<Sdouble2>
    {
        // Constants
        public static Sdouble2 Zero     { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble2(SdMath.Zero, SdMath.Zero); } }
        public static Sdouble2 One      { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble2(SdMath.One, SdMath.One); } }
        public static Sdouble2 Down     { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble2(SdMath.Zero, SdMath.Neg1); } }
        public static Sdouble2 Up       { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble2(SdMath.Zero, SdMath.One); } }
        public static Sdouble2 Left     { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble2(SdMath.Neg1, SdMath.Zero); } }
        public static Sdouble2 Right    { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble2(SdMath.One, SdMath.Zero); } }
        public static Sdouble2 AxisX    { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble2(SdMath.One, SdMath.Zero); } }
        public static Sdouble2 AxisY    { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble2(SdMath.Zero, SdMath.One); } }

        // Raw components
        public long RawX;
        public long RawY;

        // sd accessors
#pragma warning disable IDE1006 // Naming Styles to match other Vector structs
        public Sdouble x { readonly get { return Sdouble.FromRaw(RawX); } set { RawX = value.Raw; } }
        public Sdouble y { readonly get { return Sdouble.FromRaw(RawY); } set { RawY = value.Raw; } }
#pragma warning restore IDE1006 // Naming Styles

        public Sdouble2(Sdouble x, Sdouble y)
        {
            RawX = x.Raw;
            RawY = y.Raw;
        }

        // raw ctor only for internal usage
        private Sdouble2(long x, long y)
        {
            RawX = x;
            RawY = y;
        }

        public static Sdouble2 FromRaw(long rawX, long rawY) { return new Sdouble2(rawX, rawY); }
        public static Sdouble2 FromInt(int x, int y) { return new Sdouble2(SdMath.FromInt(x), SdMath.FromInt(y)); }
        public static Sdouble2 FromFloat(float x, float y) { return new Sdouble2(SdMath.FromFloat(x), SdMath.FromFloat(y)); }
        public static Sdouble2 FromDouble(double x, double y) { return new Sdouble2(SdMath.FromDouble(x), SdMath.FromDouble(y)); }

        public static Sdouble2 operator -(Sdouble2 a) { return new Sdouble2(-a.RawX, -a.RawY); }
        public static Sdouble2 operator +(Sdouble2 a, Sdouble2 b) { return new Sdouble2(a.RawX + b.RawX, a.RawY + b.RawY); }
        public static Sdouble2 operator -(Sdouble2 a, Sdouble2 b) { return new Sdouble2(a.RawX - b.RawX, a.RawY - b.RawY); }
        public static Sdouble2 operator *(Sdouble2 a, Sdouble2 b) { return new Sdouble2(SdMath.Mul(a.RawX, b.RawX), SdMath.Mul(a.RawY, b.RawY)); }
        public static Sdouble2 operator /(Sdouble2 a, Sdouble2 b) { return new Sdouble2(SdMath.DivPrecise(a.RawX, b.RawX), SdMath.DivPrecise(a.RawY, b.RawY)); }
        public static Sdouble2 operator %(Sdouble2 a, Sdouble2 b) { return new Sdouble2(a.RawX % b.RawX, a.RawY % b.RawY); }

        public static Sdouble2 operator +(Sdouble a, Sdouble2 b) { return new Sdouble2(a.Raw + b.RawX, a.Raw + b.RawY); }
        public static Sdouble2 operator +(Sdouble2 a, Sdouble b) { return new Sdouble2(a.RawX + b.Raw, a.RawY + b.Raw); }
        public static Sdouble2 operator -(Sdouble a, Sdouble2 b) { return new Sdouble2(a.Raw - b.RawX, a.Raw - b.RawY); }
        public static Sdouble2 operator -(Sdouble2 a, Sdouble b) { return new Sdouble2(a.RawX - b.Raw, a.RawY - b.Raw); }
        public static Sdouble2 operator *(Sdouble a, Sdouble2 b) { return new Sdouble2(SdMath.Mul(a.Raw, b.RawX), SdMath.Mul(a.Raw, b.RawY)); }
        public static Sdouble2 operator *(Sdouble2 a, Sdouble b) { return new Sdouble2(SdMath.Mul(a.RawX, b.Raw), SdMath.Mul(a.RawY, b.Raw)); }
        public static Sdouble2 operator /(Sdouble a, Sdouble2 b) { return new Sdouble2(SdMath.DivPrecise(a.Raw, b.RawX), SdMath.DivPrecise(a.Raw, b.RawY)); }
        public static Sdouble2 operator /(Sdouble2 a, Sdouble b) { return new Sdouble2(SdMath.DivPrecise(a.RawX, b.Raw), SdMath.DivPrecise(a.RawY, b.Raw)); }
        public static Sdouble2 operator %(Sdouble a, Sdouble2 b) { return new Sdouble2(a.Raw % b.RawX, a.Raw % b.RawY); }
        public static Sdouble2 operator %(Sdouble2 a, Sdouble b) { return new Sdouble2(a.RawX % b.Raw, a.RawY % b.Raw); }

        public static bool operator ==(Sdouble2 a, Sdouble2 b) { return a.RawX == b.RawX && a.RawY == b.RawY; }
        public static bool operator !=(Sdouble2 a, Sdouble2 b) { return a.RawX != b.RawX || a.RawY != b.RawY; }

        public static Sdouble2 Div(Sdouble2 a, Sdouble b) { long oob = SdMath.Rcp(b.Raw); return new Sdouble2(SdMath.Mul(a.RawX, oob), SdMath.Mul(a.RawY, oob)); }
        public static Sdouble2 DivFast(Sdouble2 a, Sdouble b) { long oob = SdMath.RcpFast(b.Raw); return new Sdouble2(SdMath.Mul(a.RawX, oob), SdMath.Mul(a.RawY, oob)); }
        public static Sdouble2 DivFastest(Sdouble2 a, Sdouble b) { long oob = SdMath.RcpFastest(b.Raw); return new Sdouble2(SdMath.Mul(a.RawX, oob), SdMath.Mul(a.RawY, oob)); }
        public static Sdouble2 Div(Sdouble2 a, Sdouble2 b) { return new Sdouble2(SdMath.Div(a.RawX, b.RawX), SdMath.Div(a.RawY, b.RawY)); }
        public static Sdouble2 DivFast(Sdouble2 a, Sdouble2 b) { return new Sdouble2(SdMath.DivFast(a.RawX, b.RawX), SdMath.DivFast(a.RawY, b.RawY)); }
        public static Sdouble2 DivFastest(Sdouble2 a, Sdouble2 b) { return new Sdouble2(SdMath.DivFastest(a.RawX, b.RawX), SdMath.DivFastest(a.RawY, b.RawY)); }
        public static Sdouble2 SqrtPrecise(Sdouble2 a) { return new Sdouble2(SdMath.SqrtPrecise(a.RawX), SdMath.SqrtPrecise(a.RawY)); }
        public static Sdouble2 Sqrt(Sdouble2 a) { return new Sdouble2(SdMath.Sqrt(a.RawX), SdMath.Sqrt(a.RawY)); }
        public static Sdouble2 SqrtFast(Sdouble2 a) { return new Sdouble2(SdMath.SqrtFast(a.RawX), SdMath.SqrtFast(a.RawY)); }
        public static Sdouble2 SqrtFastest(Sdouble2 a) { return new Sdouble2(SdMath.SqrtFastest(a.RawX), SdMath.SqrtFastest(a.RawY)); }
        public static Sdouble2 RSqrt(Sdouble2 a) { return new Sdouble2(SdMath.RSqrt(a.RawX), SdMath.RSqrt(a.RawY)); }
        public static Sdouble2 RSqrtFast(Sdouble2 a) { return new Sdouble2(SdMath.RSqrtFast(a.RawX), SdMath.RSqrtFast(a.RawY)); }
        public static Sdouble2 RSqrtFastest(Sdouble2 a) { return new Sdouble2(SdMath.RSqrtFastest(a.RawX), SdMath.RSqrtFastest(a.RawY)); }
        public static Sdouble2 Rcp(Sdouble2 a) { return new Sdouble2(SdMath.Rcp(a.RawX), SdMath.Rcp(a.RawY)); }
        public static Sdouble2 RcpFast(Sdouble2 a) { return new Sdouble2(SdMath.RcpFast(a.RawX), SdMath.RcpFast(a.RawY)); }
        public static Sdouble2 RcpFastest(Sdouble2 a) { return new Sdouble2(SdMath.RcpFastest(a.RawX), SdMath.RcpFastest(a.RawY)); }
        public static Sdouble2 Exp(Sdouble2 a) { return new Sdouble2(SdMath.Exp(a.RawX), SdMath.Exp(a.RawY)); }
        public static Sdouble2 ExpFast(Sdouble2 a) { return new Sdouble2(SdMath.ExpFast(a.RawX), SdMath.ExpFast(a.RawY)); }
        public static Sdouble2 ExpFastest(Sdouble2 a) { return new Sdouble2(SdMath.ExpFastest(a.RawX), SdMath.ExpFastest(a.RawY)); }
        public static Sdouble2 Exp2(Sdouble2 a) { return new Sdouble2(SdMath.Exp2(a.RawX), SdMath.Exp2(a.RawY)); }
        public static Sdouble2 Exp2Fast(Sdouble2 a) { return new Sdouble2(SdMath.Exp2Fast(a.RawX), SdMath.Exp2Fast(a.RawY)); }
        public static Sdouble2 Exp2Fastest(Sdouble2 a) { return new Sdouble2(SdMath.Exp2Fastest(a.RawX), SdMath.Exp2Fastest(a.RawY)); }
        public static Sdouble2 Log(Sdouble2 a) { return new Sdouble2(SdMath.Log(a.RawX), SdMath.Log(a.RawY)); }
        public static Sdouble2 LogFast(Sdouble2 a) { return new Sdouble2(SdMath.LogFast(a.RawX), SdMath.LogFast(a.RawY)); }
        public static Sdouble2 LogFastest(Sdouble2 a) { return new Sdouble2(SdMath.LogFastest(a.RawX), SdMath.LogFastest(a.RawY)); }
        public static Sdouble2 Log2(Sdouble2 a) { return new Sdouble2(SdMath.Log2(a.RawX), SdMath.Log2(a.RawY)); }
        public static Sdouble2 Log2Fast(Sdouble2 a) { return new Sdouble2(SdMath.Log2Fast(a.RawX), SdMath.Log2Fast(a.RawY)); }
        public static Sdouble2 Log2Fastest(Sdouble2 a) { return new Sdouble2(SdMath.Log2Fastest(a.RawX), SdMath.Log2Fastest(a.RawY)); }
        public static Sdouble2 Sin(Sdouble2 a) { return new Sdouble2(SdMath.Sin(a.RawX), SdMath.Sin(a.RawY)); }
        public static Sdouble2 SinFast(Sdouble2 a) { return new Sdouble2(SdMath.SinFast(a.RawX), SdMath.SinFast(a.RawY)); }
        public static Sdouble2 SinFastest(Sdouble2 a) { return new Sdouble2(SdMath.SinFastest(a.RawX), SdMath.SinFastest(a.RawY)); }
        public static Sdouble2 Cos(Sdouble2 a) { return new Sdouble2(SdMath.Cos(a.RawX), SdMath.Cos(a.RawY)); }
        public static Sdouble2 CosFast(Sdouble2 a) { return new Sdouble2(SdMath.CosFast(a.RawX), SdMath.CosFast(a.RawY)); }
        public static Sdouble2 CosFastest(Sdouble2 a) { return new Sdouble2(SdMath.CosFastest(a.RawX), SdMath.CosFastest(a.RawY)); }

        public static Sdouble2 Pow(Sdouble2 a, Sdouble b) { return new Sdouble2(SdMath.Pow(a.RawX, b.Raw), SdMath.Pow(a.RawY, b.Raw)); }
        public static Sdouble2 PowFast(Sdouble2 a, Sdouble b) { return new Sdouble2(SdMath.PowFast(a.RawX, b.Raw), SdMath.PowFast(a.RawY, b.Raw)); }
        public static Sdouble2 PowFastest(Sdouble2 a, Sdouble b) { return new Sdouble2(SdMath.PowFastest(a.RawX, b.Raw), SdMath.PowFastest(a.RawY, b.Raw)); }
        public static Sdouble2 Pow(Sdouble a, Sdouble2 b) { return new Sdouble2(SdMath.Pow(a.Raw, b.RawX), SdMath.Pow(a.Raw, b.RawY)); }
        public static Sdouble2 PowFast(Sdouble a, Sdouble2 b) { return new Sdouble2(SdMath.PowFast(a.Raw, b.RawX), SdMath.PowFast(a.Raw, b.RawY)); }
        public static Sdouble2 PowFastest(Sdouble a, Sdouble2 b) { return new Sdouble2(SdMath.PowFastest(a.Raw, b.RawX), SdMath.PowFastest(a.Raw, b.RawY)); }
        public static Sdouble2 Pow(Sdouble2 a, Sdouble2 b) { return new Sdouble2(SdMath.Pow(a.RawX, b.RawX), SdMath.Pow(a.RawY, b.RawY)); }
        public static Sdouble2 PowFast(Sdouble2 a, Sdouble2 b) { return new Sdouble2(SdMath.PowFast(a.RawX, b.RawX), SdMath.PowFast(a.RawY, b.RawY)); }
        public static Sdouble2 PowFastest(Sdouble2 a, Sdouble2 b) { return new Sdouble2(SdMath.PowFastest(a.RawX, b.RawX), SdMath.PowFastest(a.RawY, b.RawY)); }

        public static Sdouble Length(Sdouble2 a) { return Sdouble.FromRaw(SdMath.Sqrt(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY))); }
        public static Sdouble LengthFast(Sdouble2 a) { return Sdouble.FromRaw(SdMath.SqrtFast(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY))); }
        public static Sdouble LengthFastest(Sdouble2 a) { return Sdouble.FromRaw(SdMath.SqrtFastest(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY))); }
        public static Sdouble LengthSqr(Sdouble2 a) { return Sdouble.FromRaw(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY)); }
        public static Sdouble2 Normalize(Sdouble2 a) { Sdouble ooLen = Sdouble.FromRaw(SdMath.RSqrt(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY))); return ooLen * a; }
        public static Sdouble2 NormalizeFast(Sdouble2 a) { Sdouble ooLen = Sdouble.FromRaw(SdMath.RSqrtFast(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY))); return ooLen * a; }
        public static Sdouble2 NormalizeFastest(Sdouble2 a) { Sdouble ooLen = Sdouble.FromRaw(SdMath.RSqrtFastest(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY))); return ooLen * a; }

        public static Sdouble Dot(Sdouble2 a, Sdouble2 b) { return Sdouble.FromRaw(SdMath.Mul(a.RawX, b.RawX) + SdMath.Mul(a.RawY, b.RawY)); }
        public static Sdouble Distance(Sdouble2 a, Sdouble2 b) { return Length(a - b); }
        public static Sdouble DistanceFast(Sdouble2 a, Sdouble2 b) { return LengthFast(a - b); }
        public static Sdouble DistanceFastest(Sdouble2 a, Sdouble2 b) { return LengthFastest(a - b); }

        public static Sdouble2 Min(Sdouble2 a, Sdouble2 b) { return new Sdouble2(SdMath.Min(a.RawX, b.RawX), SdMath.Min(a.RawY, b.RawY)); }
        public static Sdouble2 Max(Sdouble2 a, Sdouble2 b) { return new Sdouble2(SdMath.Max(a.RawX, b.RawX), SdMath.Max(a.RawY, b.RawY)); }

        public static Sdouble2 Clamp(Sdouble2 a, Sdouble min, Sdouble max)
        {
            return new Sdouble2(
                SdMath.Clamp(a.RawX, min.Raw, max.Raw),
                SdMath.Clamp(a.RawY, min.Raw, max.Raw));
        }

        public static Sdouble2 Clamp(Sdouble2 a, Sdouble2 min, Sdouble2 max)
        {
            return new Sdouble2(
                SdMath.Clamp(a.RawX, min.RawX, max.RawX),
                SdMath.Clamp(a.RawY, min.RawY, max.RawY));
        }

        public static Sdouble2 Lerp(Sdouble2 a, Sdouble2 b, Sdouble t)
        {
            long tb = t.Raw;
            long ta = SdMath.One - tb;
            return new Sdouble2(
                SdMath.Mul(a.RawX, ta) + SdMath.Mul(b.RawX, tb),
                SdMath.Mul(a.RawY, ta) + SdMath.Mul(b.RawY, tb));
        }

        public bool Equals(Sdouble2 other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Sdouble2))
                return false;
            return ((Sdouble2)obj) == this;
        }

        public override string ToString()
        {
            return "(" + SdMath.ToString(RawX) + ", " + SdMath.ToString(RawY) + ")";
        }

        public override int GetHashCode()
        {
            return RawX.GetHashCode() ^ RawY.GetHashCode() * 7919;
        }
    }
}
