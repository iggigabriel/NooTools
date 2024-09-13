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
    /// Vector4 struct with signed 16.16 fixed point components.
    /// </summary>
    [Serializable]
    public struct Sdouble4 : IEquatable<Sdouble4>
    {
        public static Sdouble4 Zero      { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble4(SdMath.Zero, SdMath.Zero, SdMath.Zero, SdMath.Zero); } }
        public static Sdouble4 One       { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble4(SdMath.One, SdMath.One, SdMath.One, SdMath.One); } }
        public static Sdouble4 AxisX     { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble4(SdMath.One, SdMath.Zero, SdMath.Zero, SdMath.Zero); } }
        public static Sdouble4 AxisY     { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble4(SdMath.Zero, SdMath.One, SdMath.Zero, SdMath.Zero); } }
        public static Sdouble4 AxisZ     { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble4(SdMath.Zero, SdMath.Zero, SdMath.One, SdMath.Zero); } }
        public static Sdouble4 AxisW     { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sdouble4(SdMath.Zero, SdMath.Zero, SdMath.Zero, SdMath.One); } }

        // Raw components
        public long RawX;
        public long RawY;
        public long RawZ;
        public long RawW;

        // sd accessors
#pragma warning disable IDE1006 // Naming Styles to match other Vector structs
        public Sdouble x { readonly get { return Sdouble.FromRaw(RawX); } set { RawX = value.Raw; } }
        public Sdouble y { readonly get { return Sdouble.FromRaw(RawY); } set { RawY = value.Raw; } }
        public Sdouble z { readonly get { return Sdouble.FromRaw(RawZ); } set { RawZ = value.Raw; } }
        public Sdouble w { readonly get { return Sdouble.FromRaw(RawW); } set { RawW = value.Raw; } }
#pragma warning restore IDE1006 // Naming Styles

        public Sdouble4(Sdouble x, Sdouble y, Sdouble z, Sdouble w)
        {
            RawX = x.Raw;
            RawY = y.Raw;
            RawZ = z.Raw;
            RawW = w.Raw;
        }

        // raw ctor only for internal usage
        private Sdouble4(long x, long y, long z, long w)
        {
            RawX = x;
            RawY = y;
            RawZ = z;
            RawW = w;
        }

        public static Sdouble4 FromInt(int x, int y, int z, int w) { return new Sdouble4(Sdouble.FromInt(x), Sdouble.FromInt(y), Sdouble.FromInt(z), Sdouble.FromInt(w)); }
        public static Sdouble4 FromFloat(float x, float y, float z, float w) { return new Sdouble4(Sdouble.FromFloat(x), Sdouble.FromFloat(y), Sdouble.FromFloat(z), Sdouble.FromFloat(w)); }
        public static Sdouble4 FromDouble(double x, double y, double z, double w) { return new Sdouble4(Sdouble.FromDouble(x), Sdouble.FromDouble(y), Sdouble.FromDouble(z), Sdouble.FromDouble(w)); }

        public static Sdouble4 operator -(Sdouble4 a) { return new Sdouble4(-a.RawX, -a.RawY, -a.RawZ, -a.RawW); }
        public static Sdouble4 operator +(Sdouble4 a, Sdouble4 b) { return new Sdouble4(a.RawX + b.RawX, a.RawY + b.RawY, a.RawZ + b.RawZ, a.RawW + b.RawW); }
        public static Sdouble4 operator -(Sdouble4 a, Sdouble4 b) { return new Sdouble4(a.RawX - b.RawX, a.RawY - b.RawY, a.RawZ - b.RawZ, a.RawW - b.RawW); }
        public static Sdouble4 operator *(Sdouble4 a, Sdouble4 b) { return new Sdouble4(SdMath.Mul(a.RawX, b.RawX), SdMath.Mul(a.RawY, b.RawY), SdMath.Mul(a.RawZ, b.RawZ), SdMath.Mul(a.RawW, b.RawW)); }
        public static Sdouble4 operator /(Sdouble4 a, Sdouble4 b) { return new Sdouble4(SdMath.DivPrecise(a.RawX, b.RawX), SdMath.DivPrecise(a.RawY, b.RawY), SdMath.DivPrecise(a.RawZ, b.RawZ), SdMath.DivPrecise(a.RawW, b.RawW)); }
        public static Sdouble4 operator %(Sdouble4 a, Sdouble4 b) { return new Sdouble4(a.RawX % b.RawX, a.RawY % b.RawY, a.RawZ % b.RawZ, a.RawW % b.RawW); }

        public static Sdouble4 operator +(Sdouble a, Sdouble4 b) { return new Sdouble4(a.Raw + b.RawX, a.Raw + b.RawY, a.Raw + b.RawZ, a.Raw + b.RawW); }
        public static Sdouble4 operator +(Sdouble4 a, Sdouble b) { return new Sdouble4(a.RawX + b.Raw, a.RawY + b.Raw, a.RawZ + b.Raw, a.RawW + b.Raw); }
        public static Sdouble4 operator -(Sdouble a, Sdouble4 b) { return new Sdouble4(a.Raw - b.RawX, a.Raw - b.RawY, a.Raw - b.RawZ, a.Raw - b.RawW); }
        public static Sdouble4 operator -(Sdouble4 a, Sdouble b) { return new Sdouble4(a.RawX - b.Raw, a.RawY - b.Raw, a.RawZ - b.Raw, a.RawW - b.Raw); }
        public static Sdouble4 operator *(Sdouble a, Sdouble4 b) { return new Sdouble4(SdMath.Mul(a.Raw, b.RawX), SdMath.Mul(a.Raw, b.RawY), SdMath.Mul(a.Raw, b.RawZ), SdMath.Mul(a.Raw, b.RawW)); }
        public static Sdouble4 operator *(Sdouble4 a, Sdouble b) { return new Sdouble4(SdMath.Mul(a.RawX, b.Raw), SdMath.Mul(a.RawY, b.Raw), SdMath.Mul(a.RawZ, b.Raw), SdMath.Mul(a.RawW, b.Raw)); }
        public static Sdouble4 operator /(Sdouble a, Sdouble4 b) { return new Sdouble4(SdMath.DivPrecise(a.Raw, b.RawX), SdMath.DivPrecise(a.Raw, b.RawY), SdMath.DivPrecise(a.Raw, b.RawZ), SdMath.DivPrecise(a.Raw, b.RawW)); }
        public static Sdouble4 operator /(Sdouble4 a, Sdouble b) { return new Sdouble4(SdMath.DivPrecise(a.RawX, b.Raw), SdMath.DivPrecise(a.RawY, b.Raw), SdMath.DivPrecise(a.RawZ, b.Raw), SdMath.DivPrecise(a.RawW, b.Raw)); }
        public static Sdouble4 operator %(Sdouble a, Sdouble4 b) { return new Sdouble4(a.Raw % b.RawX, a.Raw % b.RawY, a.Raw % b.RawZ, a.Raw % b.RawW); }
        public static Sdouble4 operator %(Sdouble4 a, Sdouble b) { return new Sdouble4(a.RawX % b.Raw, a.RawY % b.Raw, a.RawZ % b.Raw, a.RawW % b.Raw); }

        public static bool operator ==(Sdouble4 a, Sdouble4 b) { return a.RawX == b.RawX && a.RawY == b.RawY && a.RawZ == b.RawZ && a.RawW == b.RawW; }
        public static bool operator !=(Sdouble4 a, Sdouble4 b) { return a.RawX != b.RawX || a.RawY != b.RawY || a.RawZ != b.RawZ || a.RawW != b.RawW; }

        public static Sdouble4 Div(Sdouble4 a, Sdouble b) { long oob = SdMath.Rcp(b.Raw); return new Sdouble4(SdMath.Mul(a.RawX, oob), SdMath.Mul(a.RawY, oob), SdMath.Mul(a.RawZ, oob), SdMath.Mul(a.RawW, oob)); }
        public static Sdouble4 DivFast(Sdouble4 a, Sdouble b) { long oob = SdMath.RcpFast(b.Raw); return new Sdouble4(SdMath.Mul(a.RawX, oob), SdMath.Mul(a.RawY, oob), SdMath.Mul(a.RawZ, oob), SdMath.Mul(a.RawW, oob)); }
        public static Sdouble4 DivFastest(Sdouble4 a, Sdouble b) { long oob = SdMath.RcpFastest(b.Raw); return new Sdouble4(SdMath.Mul(a.RawX, oob), SdMath.Mul(a.RawY, oob), SdMath.Mul(a.RawZ, oob), SdMath.Mul(a.RawW, oob)); }
        public static Sdouble4 Div(Sdouble4 a, Sdouble4 b) { return new Sdouble4(SdMath.Div(a.RawX, b.RawX), SdMath.Div(a.RawY, b.RawY), SdMath.Div(a.RawZ, b.RawZ), SdMath.Div(a.RawW, b.RawW)); }
        public static Sdouble4 DivFast(Sdouble4 a, Sdouble4 b) { return new Sdouble4(SdMath.DivFast(a.RawX, b.RawX), SdMath.DivFast(a.RawY, b.RawY), SdMath.DivFast(a.RawZ, b.RawZ), SdMath.DivFast(a.RawW, b.RawW)); }
        public static Sdouble4 DivFastest(Sdouble4 a, Sdouble4 b) { return new Sdouble4(SdMath.DivFastest(a.RawX, b.RawX), SdMath.DivFastest(a.RawY, b.RawY), SdMath.DivFastest(a.RawZ, b.RawZ), SdMath.DivFastest(a.RawW, b.RawW)); }
        public static Sdouble4 SqrtPrecise(Sdouble4 a) { return new Sdouble4(SdMath.SqrtPrecise(a.RawX), SdMath.SqrtPrecise(a.RawY), SdMath.SqrtPrecise(a.RawZ), SdMath.SqrtPrecise(a.RawW)); }
        public static Sdouble4 Sqrt(Sdouble4 a) { return new Sdouble4(SdMath.Sqrt(a.RawX), SdMath.Sqrt(a.RawY), SdMath.Sqrt(a.RawZ), SdMath.Sqrt(a.RawW)); }
        public static Sdouble4 SqrtFast(Sdouble4 a) { return new Sdouble4(SdMath.SqrtFast(a.RawX), SdMath.SqrtFast(a.RawY), SdMath.SqrtFast(a.RawZ), SdMath.SqrtFast(a.RawW)); }
        public static Sdouble4 SqrtFastest(Sdouble4 a) { return new Sdouble4(SdMath.SqrtFastest(a.RawX), SdMath.SqrtFastest(a.RawY), SdMath.SqrtFastest(a.RawZ), SdMath.SqrtFastest(a.RawW)); }
        public static Sdouble4 RSqrt(Sdouble4 a) { return new Sdouble4(SdMath.RSqrt(a.RawX), SdMath.RSqrt(a.RawY), SdMath.RSqrt(a.RawZ), SdMath.RSqrt(a.RawW)); }
        public static Sdouble4 RSqrtFast(Sdouble4 a) { return new Sdouble4(SdMath.RSqrtFast(a.RawX), SdMath.RSqrtFast(a.RawY), SdMath.RSqrtFast(a.RawZ), SdMath.RSqrtFast(a.RawW)); }
        public static Sdouble4 RSqrtFastest(Sdouble4 a) { return new Sdouble4(SdMath.RSqrtFastest(a.RawX), SdMath.RSqrtFastest(a.RawY), SdMath.RSqrtFastest(a.RawZ), SdMath.RSqrtFastest(a.RawW)); }
        public static Sdouble4 Rcp(Sdouble4 a) { return new Sdouble4(SdMath.Rcp(a.RawX), SdMath.Rcp(a.RawY), SdMath.Rcp(a.RawZ), SdMath.Rcp(a.RawW)); }
        public static Sdouble4 RcpFast(Sdouble4 a) { return new Sdouble4(SdMath.RcpFast(a.RawX), SdMath.RcpFast(a.RawY), SdMath.RcpFast(a.RawZ), SdMath.RcpFast(a.RawW)); }
        public static Sdouble4 RcpFastest(Sdouble4 a) { return new Sdouble4(SdMath.RcpFastest(a.RawX), SdMath.RcpFastest(a.RawY), SdMath.RcpFastest(a.RawZ), SdMath.RcpFastest(a.RawW)); }
        public static Sdouble4 Exp(Sdouble4 a) { return new Sdouble4(SdMath.Exp(a.RawX), SdMath.Exp(a.RawY), SdMath.Exp(a.RawZ), SdMath.Exp(a.RawW)); }
        public static Sdouble4 ExpFast(Sdouble4 a) { return new Sdouble4(SdMath.ExpFast(a.RawX), SdMath.ExpFast(a.RawY), SdMath.ExpFast(a.RawZ), SdMath.ExpFast(a.RawW)); }
        public static Sdouble4 ExpFastest(Sdouble4 a) { return new Sdouble4(SdMath.ExpFastest(a.RawX), SdMath.ExpFastest(a.RawY), SdMath.ExpFastest(a.RawZ), SdMath.ExpFastest(a.RawW)); }
        public static Sdouble4 Exp2(Sdouble4 a) { return new Sdouble4(SdMath.Exp2(a.RawX), SdMath.Exp2(a.RawY), SdMath.Exp2(a.RawZ), SdMath.Exp2(a.RawW)); }
        public static Sdouble4 Exp2Fast(Sdouble4 a) { return new Sdouble4(SdMath.Exp2Fast(a.RawX), SdMath.Exp2Fast(a.RawY), SdMath.Exp2Fast(a.RawZ), SdMath.Exp2Fast(a.RawW)); }
        public static Sdouble4 Exp2Fastest(Sdouble4 a) { return new Sdouble4(SdMath.Exp2Fastest(a.RawX), SdMath.Exp2Fastest(a.RawY), SdMath.Exp2Fastest(a.RawZ), SdMath.Exp2Fastest(a.RawW)); }
        public static Sdouble4 Log(Sdouble4 a) { return new Sdouble4(SdMath.Log(a.RawX), SdMath.Log(a.RawY), SdMath.Log(a.RawZ), SdMath.Log(a.RawW)); }
        public static Sdouble4 LogFast(Sdouble4 a) { return new Sdouble4(SdMath.LogFast(a.RawX), SdMath.LogFast(a.RawY), SdMath.LogFast(a.RawZ), SdMath.LogFast(a.RawW)); }
        public static Sdouble4 LogFastest(Sdouble4 a) { return new Sdouble4(SdMath.LogFastest(a.RawX), SdMath.LogFastest(a.RawY), SdMath.LogFastest(a.RawZ), SdMath.LogFastest(a.RawW)); }
        public static Sdouble4 Log2(Sdouble4 a) { return new Sdouble4(SdMath.Log2(a.RawX), SdMath.Log2(a.RawY), SdMath.Log2(a.RawZ), SdMath.Log2(a.RawW)); }
        public static Sdouble4 Log2Fast(Sdouble4 a) { return new Sdouble4(SdMath.Log2Fast(a.RawX), SdMath.Log2Fast(a.RawY), SdMath.Log2Fast(a.RawZ), SdMath.Log2Fast(a.RawW)); }
        public static Sdouble4 Log2Fastest(Sdouble4 a) { return new Sdouble4(SdMath.Log2Fastest(a.RawX), SdMath.Log2Fastest(a.RawY), SdMath.Log2Fastest(a.RawZ), SdMath.Log2Fastest(a.RawW)); }
        public static Sdouble4 Sin(Sdouble4 a) { return new Sdouble4(SdMath.Sin(a.RawX), SdMath.Sin(a.RawY), SdMath.Sin(a.RawZ), SdMath.Sin(a.RawW)); }
        public static Sdouble4 SinFast(Sdouble4 a) { return new Sdouble4(SdMath.SinFast(a.RawX), SdMath.SinFast(a.RawY), SdMath.SinFast(a.RawZ), SdMath.SinFast(a.RawW)); }
        public static Sdouble4 SinFastest(Sdouble4 a) { return new Sdouble4(SdMath.SinFastest(a.RawX), SdMath.SinFastest(a.RawY), SdMath.SinFastest(a.RawZ), SdMath.SinFastest(a.RawW)); }
        public static Sdouble4 Cos(Sdouble4 a) { return new Sdouble4(SdMath.Cos(a.RawX), SdMath.Cos(a.RawY), SdMath.Cos(a.RawZ), SdMath.Cos(a.RawW)); }
        public static Sdouble4 CosFast(Sdouble4 a) { return new Sdouble4(SdMath.CosFast(a.RawX), SdMath.CosFast(a.RawY), SdMath.CosFast(a.RawZ), SdMath.CosFast(a.RawW)); }
        public static Sdouble4 CosFastest(Sdouble4 a) { return new Sdouble4(SdMath.CosFastest(a.RawX), SdMath.CosFastest(a.RawY), SdMath.CosFastest(a.RawZ), SdMath.CosFastest(a.RawW)); }

        public static Sdouble4 Pow(Sdouble4 a, Sdouble b) { return new Sdouble4(SdMath.Pow(a.RawX, b.Raw), SdMath.Pow(a.RawY, b.Raw), SdMath.Pow(a.RawZ, b.Raw), SdMath.Pow(a.RawW, b.Raw)); }
        public static Sdouble4 PowFast(Sdouble4 a, Sdouble b) { return new Sdouble4(SdMath.PowFast(a.RawX, b.Raw), SdMath.PowFast(a.RawY, b.Raw), SdMath.PowFast(a.RawZ, b.Raw), SdMath.PowFast(a.RawW, b.Raw)); }
        public static Sdouble4 PowFastest(Sdouble4 a, Sdouble b) { return new Sdouble4(SdMath.PowFastest(a.RawX, b.Raw), SdMath.PowFastest(a.RawY, b.Raw), SdMath.PowFastest(a.RawZ, b.Raw), SdMath.PowFastest(a.RawW, b.Raw)); }
        public static Sdouble4 Pow(Sdouble a, Sdouble4 b) { return new Sdouble4(SdMath.Pow(a.Raw, b.RawX), SdMath.Pow(a.Raw, b.RawY), SdMath.Pow(a.Raw, b.RawZ), SdMath.Pow(a.Raw, b.RawW)); }
        public static Sdouble4 PowFast(Sdouble a, Sdouble4 b) { return new Sdouble4(SdMath.PowFast(a.Raw, b.RawX), SdMath.PowFast(a.Raw, b.RawY), SdMath.PowFast(a.Raw, b.RawZ), SdMath.PowFast(a.Raw, b.RawW)); }
        public static Sdouble4 PowFastest(Sdouble a, Sdouble4 b) { return new Sdouble4(SdMath.PowFastest(a.Raw, b.RawX), SdMath.PowFastest(a.Raw, b.RawY), SdMath.PowFastest(a.Raw, b.RawZ), SdMath.PowFastest(a.Raw, b.RawW)); }
        public static Sdouble4 Pow(Sdouble4 a, Sdouble4 b) { return new Sdouble4(SdMath.Pow(a.RawX, b.RawX), SdMath.Pow(a.RawY, b.RawY), SdMath.Pow(a.RawZ, b.RawZ), SdMath.Pow(a.RawW, b.RawW)); }
        public static Sdouble4 PowFast(Sdouble4 a, Sdouble4 b) { return new Sdouble4(SdMath.PowFast(a.RawX, b.RawX), SdMath.PowFast(a.RawY, b.RawY), SdMath.PowFast(a.RawZ, b.RawZ), SdMath.PowFast(a.RawW, b.RawW)); }
        public static Sdouble4 PowFastest(Sdouble4 a, Sdouble4 b) { return new Sdouble4(SdMath.PowFastest(a.RawX, b.RawX), SdMath.PowFastest(a.RawY, b.RawY), SdMath.PowFastest(a.RawZ, b.RawZ), SdMath.PowFastest(a.RawW, b.RawW)); }

        public static Sdouble Length(Sdouble4 a) { return Sdouble.FromRaw(SdMath.Sqrt(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY) + SdMath.Mul(a.RawZ, a.RawZ) + SdMath.Mul(a.RawW, a.RawW))); }
        public static Sdouble LengthFast(Sdouble4 a) { return Sdouble.FromRaw(SdMath.SqrtFast(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY) + SdMath.Mul(a.RawZ, a.RawZ) + SdMath.Mul(a.RawW, a.RawW))); }
        public static Sdouble LengthFastest(Sdouble4 a) { return Sdouble.FromRaw(SdMath.SqrtFastest(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY) + SdMath.Mul(a.RawZ, a.RawZ) + SdMath.Mul(a.RawW, a.RawW))); }
        public static Sdouble LengthSqr(Sdouble4 a) { return Sdouble.FromRaw(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY) + SdMath.Mul(a.RawZ, a.RawZ) + SdMath.Mul(a.RawW, a.RawW)); }
        public static Sdouble4 Normalize(Sdouble4 a) { Sdouble ooLen = Sdouble.FromRaw(SdMath.RSqrt(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY) + SdMath.Mul(a.RawZ, a.RawZ) + SdMath.Mul(a.RawW, a.RawW))); return ooLen * a; }
        public static Sdouble4 NormalizeFast(Sdouble4 a) { Sdouble ooLen = Sdouble.FromRaw(SdMath.RSqrtFast(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY) + SdMath.Mul(a.RawZ, a.RawZ) + SdMath.Mul(a.RawW, a.RawW))); return ooLen * a; }
        public static Sdouble4 NormalizeFastest(Sdouble4 a) { Sdouble ooLen = Sdouble.FromRaw(SdMath.RSqrtFastest(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY) + SdMath.Mul(a.RawZ, a.RawZ) + SdMath.Mul(a.RawW, a.RawW))); return ooLen * a; }

        public static Sdouble Dot(Sdouble4 a, Sdouble4 b) { return Sdouble.FromRaw(SdMath.Mul(a.RawX, b.RawX) + SdMath.Mul(a.RawY, b.RawY) + SdMath.Mul(a.RawZ, b.RawZ) + SdMath.Mul(a.RawW, b.RawW)); }
        public static Sdouble Distance(Sdouble4 a, Sdouble4 b) { return Length(a - b); }
        public static Sdouble DistanceFast(Sdouble4 a, Sdouble4 b) { return LengthFast(a - b); }
        public static Sdouble DistanceFastest(Sdouble4 a, Sdouble4 b) { return LengthFastest(a - b); }

        public static Sdouble4 Min(Sdouble4 a, Sdouble4 b) { return new Sdouble4(SdMath.Min(a.RawX, b.RawX), SdMath.Min(a.RawY, b.RawY), SdMath.Min(a.RawZ, b.RawZ), SdMath.Min(a.RawW, b.RawW)); }
        public static Sdouble4 Max(Sdouble4 a, Sdouble4 b) { return new Sdouble4(SdMath.Max(a.RawX, b.RawX), SdMath.Max(a.RawY, b.RawY), SdMath.Max(a.RawZ, b.RawZ), SdMath.Max(a.RawW, b.RawW)); }

        public static Sdouble4 Clamp(Sdouble4 a, Sdouble min, Sdouble max)
        {
            return new Sdouble4(
                SdMath.Clamp(a.RawX, min.Raw, max.Raw),
                SdMath.Clamp(a.RawY, min.Raw, max.Raw),
                SdMath.Clamp(a.RawZ, min.Raw, max.Raw),
                SdMath.Clamp(a.RawW, min.Raw, max.Raw));
        }

        public static Sdouble4 Clamp(Sdouble4 a, Sdouble4 min, Sdouble4 max)
        {
            return new Sdouble4(
                SdMath.Clamp(a.RawX, min.RawX, max.RawX),
                SdMath.Clamp(a.RawY, min.RawY, max.RawY),
                SdMath.Clamp(a.RawZ, min.RawZ, max.RawZ),
                SdMath.Clamp(a.RawW, min.RawW, max.RawW));
        }

        public static Sdouble4 Lerp(Sdouble4 a, Sdouble4 b, Sdouble t)
        {
            long tb = t.Raw;
            long ta = SdMath.One - tb;
            return new Sdouble4(
                SdMath.Mul(a.RawX, ta) + SdMath.Mul(b.RawX, tb),
                SdMath.Mul(a.RawY, ta) + SdMath.Mul(b.RawY, tb),
                SdMath.Mul(a.RawZ, ta) + SdMath.Mul(b.RawZ, tb),
                SdMath.Mul(a.RawW, ta) + SdMath.Mul(b.RawW, tb));
        }

        public bool Equals(Sdouble4 other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Sdouble4))
                return false;
            return ((Sdouble4)obj) == this;
        }

        public override string ToString()
        {
            return "(" + SdMath.ToString(RawX) + ", " + SdMath.ToString(RawY) + ", " + SdMath.ToString(RawZ) + ", " + SdMath.ToString(RawW) + ")";
        }

        public override int GetHashCode()
        {
            return RawX.GetHashCode() ^ (RawY.GetHashCode() * 7919) ^ (RawZ.GetHashCode() * 4513) ^ (RawW.GetHashCode() * 8923);
        }
    }
}
