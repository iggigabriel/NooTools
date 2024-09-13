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
    /// Vector3 struct with signed 32.32 fixed point components.
    /// </summary>
    [Serializable]
    public struct Sdouble3 : IEquatable<Sdouble3>
    {
        public static Sdouble3 Zero      { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble3(SdMath.Zero, SdMath.Zero, SdMath.Zero); } }
        public static Sdouble3 One       { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble3(SdMath.One, SdMath.One, SdMath.One); } }
        public static Sdouble3 Down      { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble3(SdMath.Zero, SdMath.Neg1, SdMath.Zero); } }
        public static Sdouble3 Up        { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble3(SdMath.Zero, SdMath.One, SdMath.Zero); } }
        public static Sdouble3 Left      { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble3(SdMath.Neg1, SdMath.Zero, SdMath.Zero); } }
        public static Sdouble3 Right     { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble3(SdMath.One, SdMath.Zero, SdMath.Zero); } }
        public static Sdouble3 Forward   { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble3(SdMath.Zero, SdMath.Zero, SdMath.One); } }
        public static Sdouble3 Back      { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble3(SdMath.Zero, SdMath.Zero, SdMath.Neg1); } }
        public static Sdouble3 AxisX     { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble3(SdMath.One, SdMath.Zero, SdMath.Zero); } }
        public static Sdouble3 AxisY     { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble3(SdMath.Zero, SdMath.One, SdMath.Zero); } }
        public static Sdouble3 AxisZ     { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble3(SdMath.Zero, SdMath.Zero, SdMath.One); } }

        // Raw components
        public long RawX;
        public long RawY;
        public long RawZ;

#pragma warning disable IDE1006 // Naming Styles to match other Vector structs
        public Sdouble x { readonly get { return Sdouble.FromRaw(RawX); } set { RawX = value.Raw; } }
        public Sdouble y { readonly get { return Sdouble.FromRaw(RawY); } set { RawY = value.Raw; } }
        public Sdouble z { readonly get { return Sdouble.FromRaw(RawZ); } set { RawZ = value.Raw; } }
#pragma warning restore IDE1006 // Naming Styles

        public Sdouble3(Sdouble x, Sdouble y, Sdouble z)
        {
            RawX = x.Raw;
            RawY = y.Raw;
            RawZ = z.Raw;
        }

        public Sdouble3(Sfloat3 src)
        {
            RawX = src.RawX << 16;
            RawY = src.RawY << 16;
            RawZ = src.RawZ << 16;
        }

        // raw ctor for internal use only
        private Sdouble3(long x, long y, long z)
        {
            RawX = x;
            RawY = y;
            RawZ = z;
        }

        public static Sdouble3 FromRaw(long rawX, long rawY, long rawZ) { return new Sdouble3(rawX, rawY, rawZ); }
        public static Sdouble3 FromInt(int x, int y, int z) { return new Sdouble3(SdMath.FromInt(x), SdMath.FromInt(y), SdMath.FromInt(z)); }
        public static Sdouble3 FromFloat(float x, float y, float z) { return new Sdouble3(SdMath.FromFloat(x), SdMath.FromFloat(y), SdMath.FromFloat(z)); }
        public static Sdouble3 FromDouble(double x, double y, double z) { return new Sdouble3(SdMath.FromDouble(x), SdMath.FromDouble(y), SdMath.FromDouble(z)); }

        public static Sdouble3 operator -(Sdouble3 a) { return new Sdouble3(-a.RawX, -a.RawY, -a.RawZ); }
        public static Sdouble3 operator +(Sdouble3 a, Sdouble3 b) { return new Sdouble3(a.RawX + b.RawX, a.RawY + b.RawY, a.RawZ + b.RawZ); }
        public static Sdouble3 operator -(Sdouble3 a, Sdouble3 b) { return new Sdouble3(a.RawX - b.RawX, a.RawY - b.RawY, a.RawZ - b.RawZ); }
        public static Sdouble3 operator *(Sdouble3 a, Sdouble3 b) { return new Sdouble3(SdMath.Mul(a.RawX, b.RawX), SdMath.Mul(a.RawY, b.RawY), SdMath.Mul(a.RawZ, b.RawZ)); }
        public static Sdouble3 operator /(Sdouble3 a, Sdouble3 b) { return new Sdouble3(SdMath.DivPrecise(a.RawX, b.RawX), SdMath.DivPrecise(a.RawY, b.RawY), SdMath.DivPrecise(a.RawZ, b.RawZ)); }
        public static Sdouble3 operator %(Sdouble3 a, Sdouble3 b) { return new Sdouble3(a.RawX % b.RawX, a.RawY % b.RawY, a.RawZ % b.RawZ); }

        public static Sdouble3 operator +(Sdouble a, Sdouble3 b) { return new Sdouble3(a.Raw + b.RawX, a.Raw + b.RawY, a.Raw + b.RawZ); }
        public static Sdouble3 operator +(Sdouble3 a, Sdouble b) { return new Sdouble3(a.RawX + b.Raw, a.RawY + b.Raw, a.RawZ + b.Raw); }
        public static Sdouble3 operator -(Sdouble a, Sdouble3 b) { return new Sdouble3(a.Raw - b.RawX, a.Raw - b.RawY, a.Raw - b.RawZ); }
        public static Sdouble3 operator -(Sdouble3 a, Sdouble b) { return new Sdouble3(a.RawX - b.Raw, a.RawY - b.Raw, a.RawZ - b.Raw); }
        public static Sdouble3 operator *(Sdouble a, Sdouble3 b) { return new Sdouble3(SdMath.Mul(a.Raw, b.RawX), SdMath.Mul(a.Raw, b.RawY), SdMath.Mul(a.Raw, b.RawZ)); }
        public static Sdouble3 operator *(Sdouble3 a, Sdouble b) { return new Sdouble3(SdMath.Mul(a.RawX, b.Raw), SdMath.Mul(a.RawY, b.Raw), SdMath.Mul(a.RawZ, b.Raw)); }
        public static Sdouble3 operator /(Sdouble a, Sdouble3 b) { return new Sdouble3(SdMath.DivPrecise(a.Raw, b.RawX), SdMath.DivPrecise(a.Raw, b.RawY), SdMath.DivPrecise(a.Raw, b.RawZ)); }
        public static Sdouble3 operator /(Sdouble3 a, Sdouble b) { return new Sdouble3(SdMath.DivPrecise(a.RawX, b.Raw), SdMath.DivPrecise(a.RawY, b.Raw), SdMath.DivPrecise(a.RawZ, b.Raw)); }
        public static Sdouble3 operator %(Sdouble a, Sdouble3 b) { return new Sdouble3(a.Raw % b.RawX, a.Raw % b.RawY, a.Raw % b.RawZ); }
        public static Sdouble3 operator %(Sdouble3 a, Sdouble b) { return new Sdouble3(a.RawX % b.Raw, a.RawY % b.Raw, a.RawZ % b.Raw); }

        public static bool operator ==(Sdouble3 a, Sdouble3 b) { return a.RawX == b.RawX && a.RawY == b.RawY && a.RawZ == b.RawZ; }
        public static bool operator !=(Sdouble3 a, Sdouble3 b) { return a.RawX != b.RawX || a.RawY != b.RawY || a.RawZ != b.RawZ; }

        public static Sdouble3 Div(Sdouble3 a, Sdouble b) { long oob = SdMath.Rcp(b.Raw); return new Sdouble3(SdMath.Mul(a.RawX, oob), SdMath.Mul(a.RawY, oob), SdMath.Mul(a.RawZ, oob)); }
        public static Sdouble3 DivFast(Sdouble3 a, Sdouble b) { long oob = SdMath.RcpFast(b.Raw); return new Sdouble3(SdMath.Mul(a.RawX, oob), SdMath.Mul(a.RawY, oob), SdMath.Mul(a.RawZ, oob)); }
        public static Sdouble3 DivFastest(Sdouble3 a, Sdouble b) { long oob = SdMath.RcpFastest(b.Raw); return new Sdouble3(SdMath.Mul(a.RawX, oob), SdMath.Mul(a.RawY, oob), SdMath.Mul(a.RawZ, oob)); }
        public static Sdouble3 Div(Sdouble3 a, Sdouble3 b) { return new Sdouble3(SdMath.Div(a.RawX, b.RawX), SdMath.Div(a.RawY, b.RawY), SdMath.Div(a.RawZ, b.RawZ)); }
        public static Sdouble3 DivFast(Sdouble3 a, Sdouble3 b) { return new Sdouble3(SdMath.DivFast(a.RawX, b.RawX), SdMath.DivFast(a.RawY, b.RawY), SdMath.DivFast(a.RawZ, b.RawZ)); }
        public static Sdouble3 DivFastest(Sdouble3 a, Sdouble3 b) { return new Sdouble3(SdMath.DivFastest(a.RawX, b.RawX), SdMath.DivFastest(a.RawY, b.RawY), SdMath.DivFastest(a.RawZ, b.RawZ)); }
        public static Sdouble3 SqrtPrecise(Sdouble3 a) { return new Sdouble3(SdMath.SqrtPrecise(a.RawX), SdMath.SqrtPrecise(a.RawY), SdMath.SqrtPrecise(a.RawZ)); }
        public static Sdouble3 Sqrt(Sdouble3 a) { return new Sdouble3(SdMath.Sqrt(a.RawX), SdMath.Sqrt(a.RawY), SdMath.Sqrt(a.RawZ)); }
        public static Sdouble3 SqrtFast(Sdouble3 a) { return new Sdouble3(SdMath.SqrtFast(a.RawX), SdMath.SqrtFast(a.RawY), SdMath.SqrtFast(a.RawZ)); }
        public static Sdouble3 SqrtFastest(Sdouble3 a) { return new Sdouble3(SdMath.SqrtFastest(a.RawX), SdMath.SqrtFastest(a.RawY), SdMath.SqrtFastest(a.RawZ)); }
        public static Sdouble3 RSqrt(Sdouble3 a) { return new Sdouble3(SdMath.RSqrt(a.RawX), SdMath.RSqrt(a.RawY), SdMath.RSqrt(a.RawZ)); }
        public static Sdouble3 RSqrtFast(Sdouble3 a) { return new Sdouble3(SdMath.RSqrtFast(a.RawX), SdMath.RSqrtFast(a.RawY), SdMath.RSqrtFast(a.RawZ)); }
        public static Sdouble3 RSqrtFastest(Sdouble3 a) { return new Sdouble3(SdMath.RSqrtFastest(a.RawX), SdMath.RSqrtFastest(a.RawY), SdMath.RSqrtFastest(a.RawZ)); }
        public static Sdouble3 Rcp(Sdouble3 a) { return new Sdouble3(SdMath.Rcp(a.RawX), SdMath.Rcp(a.RawY), SdMath.Rcp(a.RawZ)); }
        public static Sdouble3 RcpFast(Sdouble3 a) { return new Sdouble3(SdMath.RcpFast(a.RawX), SdMath.RcpFast(a.RawY), SdMath.RcpFast(a.RawZ)); }
        public static Sdouble3 RcpFastest(Sdouble3 a) { return new Sdouble3(SdMath.RcpFastest(a.RawX), SdMath.RcpFastest(a.RawY), SdMath.RcpFastest(a.RawZ)); }
        public static Sdouble3 Exp(Sdouble3 a) { return new Sdouble3(SdMath.Exp(a.RawX), SdMath.Exp(a.RawY), SdMath.Exp(a.RawZ)); }
        public static Sdouble3 ExpFast(Sdouble3 a) { return new Sdouble3(SdMath.ExpFast(a.RawX), SdMath.ExpFast(a.RawY), SdMath.ExpFast(a.RawZ)); }
        public static Sdouble3 ExpFastest(Sdouble3 a) { return new Sdouble3(SdMath.ExpFastest(a.RawX), SdMath.ExpFastest(a.RawY), SdMath.ExpFastest(a.RawZ)); }
        public static Sdouble3 Exp2(Sdouble3 a) { return new Sdouble3(SdMath.Exp2(a.RawX), SdMath.Exp2(a.RawY), SdMath.Exp2(a.RawZ)); }
        public static Sdouble3 Exp2Fast(Sdouble3 a) { return new Sdouble3(SdMath.Exp2Fast(a.RawX), SdMath.Exp2Fast(a.RawY), SdMath.Exp2Fast(a.RawZ)); }
        public static Sdouble3 Exp2Fastest(Sdouble3 a) { return new Sdouble3(SdMath.Exp2Fastest(a.RawX), SdMath.Exp2Fastest(a.RawY), SdMath.Exp2Fastest(a.RawZ)); }
        public static Sdouble3 Log(Sdouble3 a) { return new Sdouble3(SdMath.Log(a.RawX), SdMath.Log(a.RawY), SdMath.Log(a.RawZ)); }
        public static Sdouble3 LogFast(Sdouble3 a) { return new Sdouble3(SdMath.LogFast(a.RawX), SdMath.LogFast(a.RawY), SdMath.LogFast(a.RawZ)); }
        public static Sdouble3 LogFastest(Sdouble3 a) { return new Sdouble3(SdMath.LogFastest(a.RawX), SdMath.LogFastest(a.RawY), SdMath.LogFastest(a.RawZ)); }
        public static Sdouble3 Log2(Sdouble3 a) { return new Sdouble3(SdMath.Log2(a.RawX), SdMath.Log2(a.RawY), SdMath.Log2(a.RawZ)); }
        public static Sdouble3 Log2Fast(Sdouble3 a) { return new Sdouble3(SdMath.Log2Fast(a.RawX), SdMath.Log2Fast(a.RawY), SdMath.Log2Fast(a.RawZ)); }
        public static Sdouble3 Log2Fastest(Sdouble3 a) { return new Sdouble3(SdMath.Log2Fastest(a.RawX), SdMath.Log2Fastest(a.RawY), SdMath.Log2Fastest(a.RawZ)); }
        public static Sdouble3 Sin(Sdouble3 a) { return new Sdouble3(SdMath.Sin(a.RawX), SdMath.Sin(a.RawY), SdMath.Sin(a.RawZ)); }
        public static Sdouble3 SinFast(Sdouble3 a) { return new Sdouble3(SdMath.SinFast(a.RawX), SdMath.SinFast(a.RawY), SdMath.SinFast(a.RawZ)); }
        public static Sdouble3 SinFastest(Sdouble3 a) { return new Sdouble3(SdMath.SinFastest(a.RawX), SdMath.SinFastest(a.RawY), SdMath.SinFastest(a.RawZ)); }
        public static Sdouble3 Cos(Sdouble3 a) { return new Sdouble3(SdMath.Cos(a.RawX), SdMath.Cos(a.RawY), SdMath.Cos(a.RawZ)); }
        public static Sdouble3 CosFast(Sdouble3 a) { return new Sdouble3(SdMath.CosFast(a.RawX), SdMath.CosFast(a.RawY), SdMath.CosFast(a.RawZ)); }
        public static Sdouble3 CosFastest(Sdouble3 a) { return new Sdouble3(SdMath.CosFastest(a.RawX), SdMath.CosFastest(a.RawY), SdMath.CosFastest(a.RawZ)); }

        public static Sdouble3 Pow(Sdouble3 a, Sdouble b) { return new Sdouble3(SdMath.Pow(a.RawX, b.Raw), SdMath.Pow(a.RawY, b.Raw), SdMath.Pow(a.RawZ, b.Raw)); }
        public static Sdouble3 PowFast(Sdouble3 a, Sdouble b) { return new Sdouble3(SdMath.PowFast(a.RawX, b.Raw), SdMath.PowFast(a.RawY, b.Raw), SdMath.PowFast(a.RawZ, b.Raw)); }
        public static Sdouble3 PowFastest(Sdouble3 a, Sdouble b) { return new Sdouble3(SdMath.PowFastest(a.RawX, b.Raw), SdMath.PowFastest(a.RawY, b.Raw), SdMath.PowFastest(a.RawZ, b.Raw)); }
        public static Sdouble3 Pow(Sdouble a, Sdouble3 b) { return new Sdouble3(SdMath.Pow(a.Raw, b.RawX), SdMath.Pow(a.Raw, b.RawY), SdMath.Pow(a.Raw, b.RawZ)); }
        public static Sdouble3 PowFast(Sdouble a, Sdouble3 b) { return new Sdouble3(SdMath.PowFast(a.Raw, b.RawX), SdMath.PowFast(a.Raw, b.RawY), SdMath.PowFast(a.Raw, b.RawZ)); }
        public static Sdouble3 PowFastest(Sdouble a, Sdouble3 b) { return new Sdouble3(SdMath.PowFastest(a.Raw, b.RawX), SdMath.PowFastest(a.Raw, b.RawY), SdMath.PowFastest(a.Raw, b.RawZ)); }
        public static Sdouble3 Pow(Sdouble3 a, Sdouble3 b) { return new Sdouble3(SdMath.Pow(a.RawX, b.RawX), SdMath.Pow(a.RawY, b.RawY), SdMath.Pow(a.RawZ, b.RawZ)); }
        public static Sdouble3 PowFast(Sdouble3 a, Sdouble3 b) { return new Sdouble3(SdMath.PowFast(a.RawX, b.RawX), SdMath.PowFast(a.RawY, b.RawY), SdMath.PowFast(a.RawZ, b.RawZ)); }
        public static Sdouble3 PowFastest(Sdouble3 a, Sdouble3 b) { return new Sdouble3(SdMath.PowFastest(a.RawX, b.RawX), SdMath.PowFastest(a.RawY, b.RawY), SdMath.PowFastest(a.RawZ, b.RawZ)); }

        public static Sdouble Length(Sdouble3 a) { return Sdouble.FromRaw(SdMath.Sqrt(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY) + SdMath.Mul(a.RawZ, a.RawZ))); }
        public static Sdouble LengthFast(Sdouble3 a) { return Sdouble.FromRaw(SdMath.SqrtFast(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY) + SdMath.Mul(a.RawZ, a.RawZ))); }
        public static Sdouble LengthFastest(Sdouble3 a) { return Sdouble.FromRaw(SdMath.SqrtFastest(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY) + SdMath.Mul(a.RawZ, a.RawZ))); }
        public static Sdouble LengthSqr(Sdouble3 a) { return Sdouble.FromRaw(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY) + SdMath.Mul(a.RawZ, a.RawZ)); }
        public static Sdouble3 Normalize(Sdouble3 a) { Sdouble ooLen = Sdouble.FromRaw(SdMath.RSqrt(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY) + SdMath.Mul(a.RawZ, a.RawZ))); return ooLen * a; }
        public static Sdouble3 NormalizeFast(Sdouble3 a) { Sdouble ooLen = Sdouble.FromRaw(SdMath.RSqrtFast(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY) + SdMath.Mul(a.RawZ, a.RawZ))); return ooLen * a; }
        public static Sdouble3 NormalizeFastest(Sdouble3 a) { Sdouble ooLen = Sdouble.FromRaw(SdMath.RSqrtFastest(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY) + SdMath.Mul(a.RawZ, a.RawZ))); return ooLen * a; }

        public static Sdouble Dot(Sdouble3 a, Sdouble3 b) { return Sdouble.FromRaw(SdMath.Mul(a.RawX, b.RawX) + SdMath.Mul(a.RawY, b.RawY) + SdMath.Mul(a.RawZ, b.RawZ)); }
        public static Sdouble Distance(Sdouble3 a, Sdouble3 b) { return Length(a - b); }
        public static Sdouble DistanceFast(Sdouble3 a, Sdouble3 b) { return LengthFast(a - b); }
        public static Sdouble DistanceFastest(Sdouble3 a, Sdouble3 b) { return LengthFastest(a - b); }

        public static Sdouble3 Min(Sdouble3 a, Sdouble3 b) { return new Sdouble3(SdMath.Min(a.RawX, b.RawX), SdMath.Min(a.RawY, b.RawY), SdMath.Min(a.RawZ, b.RawZ)); }
        public static Sdouble3 Max(Sdouble3 a, Sdouble3 b) { return new Sdouble3(SdMath.Max(a.RawX, b.RawX), SdMath.Max(a.RawY, b.RawY), SdMath.Max(a.RawZ, b.RawZ)); }

        public static Sdouble3 Clamp(Sdouble3 a, Sdouble min, Sdouble max)
        {
            return new Sdouble3(
                SdMath.Clamp(a.RawX, min.Raw, max.Raw),
                SdMath.Clamp(a.RawY, min.Raw, max.Raw),
                SdMath.Clamp(a.RawZ, min.Raw, max.Raw));
        }

        public static Sdouble3 Clamp(Sdouble3 a, Sdouble3 min, Sdouble3 max)
        {
            return new Sdouble3(
                SdMath.Clamp(a.RawX, min.RawX, max.RawX),
                SdMath.Clamp(a.RawY, min.RawY, max.RawY),
                SdMath.Clamp(a.RawZ, min.RawZ, max.RawZ));
        }

        public static Sdouble3 Lerp(Sdouble3 a, Sdouble3 b, Sdouble t)
        {
            long tb = t.Raw;
            long ta = SdMath.One - tb;
            return new Sdouble3(
                SdMath.Mul(a.RawX, ta) + SdMath.Mul(b.RawX, tb),
                SdMath.Mul(a.RawY, ta) + SdMath.Mul(b.RawY, tb),
                SdMath.Mul(a.RawZ, ta) + SdMath.Mul(b.RawZ, tb));
        }

        public static Sdouble3 Cross(Sdouble3 a, Sdouble3 b)
        {
            return new Sdouble3(
                SdMath.Mul(a.RawY, b.RawZ) - SdMath.Mul(a.RawZ, b.RawY),
                SdMath.Mul(a.RawZ, b.RawX) - SdMath.Mul(a.RawX, b.RawZ),
                SdMath.Mul(a.RawX, b.RawY) - SdMath.Mul(a.RawY, b.RawX));
        }

        public bool Equals(Sdouble3 other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Sdouble3))
                return false;
            return ((Sdouble3)obj) == this;
        }

        public override string ToString()
        {
            return "(" + SdMath.ToString(RawX) + ", " + SdMath.ToString(RawY) + ", " + SdMath.ToString(RawZ) + ")";
        }

        public override int GetHashCode()
        {
            return RawX.GetHashCode() ^ RawY.GetHashCode() * 7919 ^ RawZ.GetHashCode() * 4513;
        }
    }
}
