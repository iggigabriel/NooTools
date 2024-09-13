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
    public struct Sfloat4 : IEquatable<Sfloat4>
    {
        public static Sfloat4 Zero      { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat4(SfMath.Zero, SfMath.Zero, SfMath.Zero, SfMath.Zero); } }
        public static Sfloat4 One       { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat4(SfMath.One, SfMath.One, SfMath.One, SfMath.One); } }
        public static Sfloat4 AxisX     { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat4(SfMath.One, SfMath.Zero, SfMath.Zero, SfMath.Zero); } }
        public static Sfloat4 AxisY     { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat4(SfMath.Zero, SfMath.One, SfMath.Zero, SfMath.Zero); } }
        public static Sfloat4 AxisZ     { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat4(SfMath.Zero, SfMath.Zero, SfMath.One, SfMath.Zero); } }
        public static Sfloat4 AxisW     { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat4(SfMath.Zero, SfMath.Zero, SfMath.Zero, SfMath.One); } }

        // Raw components
        public int RawX;
        public int RawY;
        public int RawZ;
        public int RawW;

        // sf accessors
#pragma warning disable IDE1006 // Naming Styles to match other Vector structs
        public Sfloat x { readonly get { return Sfloat.FromRaw(RawX); } set { RawX = value.Raw; } }
        public Sfloat y { readonly get { return Sfloat.FromRaw(RawY); } set { RawY = value.Raw; } }
        public Sfloat z { readonly get { return Sfloat.FromRaw(RawZ); } set { RawZ = value.Raw; } }
        public Sfloat w { readonly get { return Sfloat.FromRaw(RawW); } set { RawW = value.Raw; } }
#pragma warning restore IDE1006 // Naming Styles

        public Sfloat4(Sfloat x, Sfloat y, Sfloat z, Sfloat w)
        {
            RawX = x.Raw;
            RawY = y.Raw;
            RawZ = z.Raw;
            RawW = w.Raw;
        }

        public Sfloat4(int rawX, int rawY, int rawZ, int rawW)
        {
            RawX = rawX;
            RawY = rawY;
            RawZ = rawZ;
            RawW = rawW;
        }

        public static Sfloat4 FromInt(int x, int y, int z, int w) { return new Sfloat4(Sfloat.FromInt(x), Sfloat.FromInt(y), Sfloat.FromInt(z), Sfloat.FromInt(w)); }
        public static Sfloat4 FromFloat(float x, float y, float z, float w) { return new Sfloat4(Sfloat.FromFloat(x), Sfloat.FromFloat(y), Sfloat.FromFloat(z), Sfloat.FromFloat(w)); }
        public static Sfloat4 FromDouble(double x, double y, double z, double w) { return new Sfloat4(Sfloat.FromDouble(x), Sfloat.FromDouble(y), Sfloat.FromDouble(z), Sfloat.FromDouble(w)); }

        public static Sfloat4 operator -(Sfloat4 a) { return new Sfloat4(-a.RawX, -a.RawY, -a.RawZ, -a.RawW); }
        public static Sfloat4 operator +(Sfloat4 a, Sfloat4 b) { return new Sfloat4(a.RawX + b.RawX, a.RawY + b.RawY, a.RawZ + b.RawZ, a.RawW + b.RawW); }
        public static Sfloat4 operator -(Sfloat4 a, Sfloat4 b) { return new Sfloat4(a.RawX - b.RawX, a.RawY - b.RawY, a.RawZ - b.RawZ, a.RawW - b.RawW); }
        public static Sfloat4 operator *(Sfloat4 a, Sfloat4 b) { return new Sfloat4(SfMath.Mul(a.RawX, b.RawX), SfMath.Mul(a.RawY, b.RawY), SfMath.Mul(a.RawZ, b.RawZ), SfMath.Mul(a.RawW, b.RawW)); }
        public static Sfloat4 operator /(Sfloat4 a, Sfloat4 b) { return new Sfloat4(SfMath.DivPrecise(a.RawX, b.RawX), SfMath.DivPrecise(a.RawY, b.RawY), SfMath.DivPrecise(a.RawZ, b.RawZ), SfMath.DivPrecise(a.RawW, b.RawW)); }
        public static Sfloat4 operator %(Sfloat4 a, Sfloat4 b) { return new Sfloat4(a.RawX % b.RawX, a.RawY % b.RawY, a.RawZ % b.RawZ, a.RawW % b.RawW); }

        public static Sfloat4 operator +(Sfloat a, Sfloat4 b) { return new Sfloat4(a.Raw + b.RawX, a.Raw + b.RawY, a.Raw + b.RawZ, a.Raw + b.RawW); }
        public static Sfloat4 operator +(Sfloat4 a, Sfloat b) { return new Sfloat4(a.RawX + b.Raw, a.RawY + b.Raw, a.RawZ + b.Raw, a.RawW + b.Raw); }
        public static Sfloat4 operator -(Sfloat a, Sfloat4 b) { return new Sfloat4(a.Raw - b.RawX, a.Raw - b.RawY, a.Raw - b.RawZ, a.Raw - b.RawW); }
        public static Sfloat4 operator -(Sfloat4 a, Sfloat b) { return new Sfloat4(a.RawX - b.Raw, a.RawY - b.Raw, a.RawZ - b.Raw, a.RawW - b.Raw); }
        public static Sfloat4 operator *(Sfloat a, Sfloat4 b) { return new Sfloat4(SfMath.Mul(a.Raw, b.RawX), SfMath.Mul(a.Raw, b.RawY), SfMath.Mul(a.Raw, b.RawZ), SfMath.Mul(a.Raw, b.RawW)); }
        public static Sfloat4 operator *(Sfloat4 a, Sfloat b) { return new Sfloat4(SfMath.Mul(a.RawX, b.Raw), SfMath.Mul(a.RawY, b.Raw), SfMath.Mul(a.RawZ, b.Raw), SfMath.Mul(a.RawW, b.Raw)); }
        public static Sfloat4 operator /(Sfloat a, Sfloat4 b) { return new Sfloat4(SfMath.DivPrecise(a.Raw, b.RawX), SfMath.DivPrecise(a.Raw, b.RawY), SfMath.DivPrecise(a.Raw, b.RawZ), SfMath.DivPrecise(a.Raw, b.RawW)); }
        public static Sfloat4 operator /(Sfloat4 a, Sfloat b) { return new Sfloat4(SfMath.DivPrecise(a.RawX, b.Raw), SfMath.DivPrecise(a.RawY, b.Raw), SfMath.DivPrecise(a.RawZ, b.Raw), SfMath.DivPrecise(a.RawW, b.Raw)); }
        public static Sfloat4 operator %(Sfloat a, Sfloat4 b) { return new Sfloat4(a.Raw % b.RawX, a.Raw % b.RawY, a.Raw % b.RawZ, a.Raw % b.RawW); }
        public static Sfloat4 operator %(Sfloat4 a, Sfloat b) { return new Sfloat4(a.RawX % b.Raw, a.RawY % b.Raw, a.RawZ % b.Raw, a.RawW % b.Raw); }

        public static bool operator ==(Sfloat4 a, Sfloat4 b) { return a.RawX == b.RawX && a.RawY == b.RawY && a.RawZ == b.RawZ && a.RawW == b.RawW; }
        public static bool operator !=(Sfloat4 a, Sfloat4 b) { return a.RawX != b.RawX || a.RawY != b.RawY || a.RawZ != b.RawZ || a.RawW != b.RawW; }

        public static Sfloat4 Div(Sfloat4 a, Sfloat b) { int oob = SfMath.Rcp(b.Raw); return new Sfloat4(SfMath.Mul(a.RawX, oob), SfMath.Mul(a.RawY, oob), SfMath.Mul(a.RawZ, oob), SfMath.Mul(a.RawW, oob)); }
        public static Sfloat4 DivFast(Sfloat4 a, Sfloat b) { int oob = SfMath.RcpFast(b.Raw); return new Sfloat4(SfMath.Mul(a.RawX, oob), SfMath.Mul(a.RawY, oob), SfMath.Mul(a.RawZ, oob), SfMath.Mul(a.RawW, oob)); }
        public static Sfloat4 DivFastest(Sfloat4 a, Sfloat b) { int oob = SfMath.RcpFastest(b.Raw); return new Sfloat4(SfMath.Mul(a.RawX, oob), SfMath.Mul(a.RawY, oob), SfMath.Mul(a.RawZ, oob), SfMath.Mul(a.RawW, oob)); }
        public static Sfloat4 Div(Sfloat4 a, Sfloat4 b) { return new Sfloat4(SfMath.Div(a.RawX, b.RawX), SfMath.Div(a.RawY, b.RawY), SfMath.Div(a.RawZ, b.RawZ), SfMath.Div(a.RawW, b.RawW)); }
        public static Sfloat4 DivFast(Sfloat4 a, Sfloat4 b) { return new Sfloat4(SfMath.DivFast(a.RawX, b.RawX), SfMath.DivFast(a.RawY, b.RawY), SfMath.DivFast(a.RawZ, b.RawZ), SfMath.DivFast(a.RawW, b.RawW)); }
        public static Sfloat4 DivFastest(Sfloat4 a, Sfloat4 b) { return new Sfloat4(SfMath.DivFastest(a.RawX, b.RawX), SfMath.DivFastest(a.RawY, b.RawY), SfMath.DivFastest(a.RawZ, b.RawZ), SfMath.DivFastest(a.RawW, b.RawW)); }
        public static Sfloat4 SqrtPrecise(Sfloat4 a) { return new Sfloat4(SfMath.SqrtPrecise(a.RawX), SfMath.SqrtPrecise(a.RawY), SfMath.SqrtPrecise(a.RawZ), SfMath.SqrtPrecise(a.RawW)); }
        public static Sfloat4 Sqrt(Sfloat4 a) { return new Sfloat4(SfMath.Sqrt(a.RawX), SfMath.Sqrt(a.RawY), SfMath.Sqrt(a.RawZ), SfMath.Sqrt(a.RawW)); }
        public static Sfloat4 SqrtFast(Sfloat4 a) { return new Sfloat4(SfMath.SqrtFast(a.RawX), SfMath.SqrtFast(a.RawY), SfMath.SqrtFast(a.RawZ), SfMath.SqrtFast(a.RawW)); }
        public static Sfloat4 SqrtFastest(Sfloat4 a) { return new Sfloat4(SfMath.SqrtFastest(a.RawX), SfMath.SqrtFastest(a.RawY), SfMath.SqrtFastest(a.RawZ), SfMath.SqrtFastest(a.RawW)); }
        public static Sfloat4 RSqrt(Sfloat4 a) { return new Sfloat4(SfMath.RSqrt(a.RawX), SfMath.RSqrt(a.RawY), SfMath.RSqrt(a.RawZ), SfMath.RSqrt(a.RawW)); }
        public static Sfloat4 RSqrtFast(Sfloat4 a) { return new Sfloat4(SfMath.RSqrtFast(a.RawX), SfMath.RSqrtFast(a.RawY), SfMath.RSqrtFast(a.RawZ), SfMath.RSqrtFast(a.RawW)); }
        public static Sfloat4 RSqrtFastest(Sfloat4 a) { return new Sfloat4(SfMath.RSqrtFastest(a.RawX), SfMath.RSqrtFastest(a.RawY), SfMath.RSqrtFastest(a.RawZ), SfMath.RSqrtFastest(a.RawW)); }
        public static Sfloat4 Rcp(Sfloat4 a) { return new Sfloat4(SfMath.Rcp(a.RawX), SfMath.Rcp(a.RawY), SfMath.Rcp(a.RawZ), SfMath.Rcp(a.RawW)); }
        public static Sfloat4 RcpFast(Sfloat4 a) { return new Sfloat4(SfMath.RcpFast(a.RawX), SfMath.RcpFast(a.RawY), SfMath.RcpFast(a.RawZ), SfMath.RcpFast(a.RawW)); }
        public static Sfloat4 RcpFastest(Sfloat4 a) { return new Sfloat4(SfMath.RcpFastest(a.RawX), SfMath.RcpFastest(a.RawY), SfMath.RcpFastest(a.RawZ), SfMath.RcpFastest(a.RawW)); }
        public static Sfloat4 Exp(Sfloat4 a) { return new Sfloat4(SfMath.Exp(a.RawX), SfMath.Exp(a.RawY), SfMath.Exp(a.RawZ), SfMath.Exp(a.RawW)); }
        public static Sfloat4 ExpFast(Sfloat4 a) { return new Sfloat4(SfMath.ExpFast(a.RawX), SfMath.ExpFast(a.RawY), SfMath.ExpFast(a.RawZ), SfMath.ExpFast(a.RawW)); }
        public static Sfloat4 ExpFastest(Sfloat4 a) { return new Sfloat4(SfMath.ExpFastest(a.RawX), SfMath.ExpFastest(a.RawY), SfMath.ExpFastest(a.RawZ), SfMath.ExpFastest(a.RawW)); }
        public static Sfloat4 Exp2(Sfloat4 a) { return new Sfloat4(SfMath.Exp2(a.RawX), SfMath.Exp2(a.RawY), SfMath.Exp2(a.RawZ), SfMath.Exp2(a.RawW)); }
        public static Sfloat4 Exp2Fast(Sfloat4 a) { return new Sfloat4(SfMath.Exp2Fast(a.RawX), SfMath.Exp2Fast(a.RawY), SfMath.Exp2Fast(a.RawZ), SfMath.Exp2Fast(a.RawW)); }
        public static Sfloat4 Exp2Fastest(Sfloat4 a) { return new Sfloat4(SfMath.Exp2Fastest(a.RawX), SfMath.Exp2Fastest(a.RawY), SfMath.Exp2Fastest(a.RawZ), SfMath.Exp2Fastest(a.RawW)); }
        public static Sfloat4 Log(Sfloat4 a) { return new Sfloat4(SfMath.Log(a.RawX), SfMath.Log(a.RawY), SfMath.Log(a.RawZ), SfMath.Log(a.RawW)); }
        public static Sfloat4 LogFast(Sfloat4 a) { return new Sfloat4(SfMath.LogFast(a.RawX), SfMath.LogFast(a.RawY), SfMath.LogFast(a.RawZ), SfMath.LogFast(a.RawW)); }
        public static Sfloat4 LogFastest(Sfloat4 a) { return new Sfloat4(SfMath.LogFastest(a.RawX), SfMath.LogFastest(a.RawY), SfMath.LogFastest(a.RawZ), SfMath.LogFastest(a.RawW)); }
        public static Sfloat4 Log2(Sfloat4 a) { return new Sfloat4(SfMath.Log2(a.RawX), SfMath.Log2(a.RawY), SfMath.Log2(a.RawZ), SfMath.Log2(a.RawW)); }
        public static Sfloat4 Log2Fast(Sfloat4 a) { return new Sfloat4(SfMath.Log2Fast(a.RawX), SfMath.Log2Fast(a.RawY), SfMath.Log2Fast(a.RawZ), SfMath.Log2Fast(a.RawW)); }
        public static Sfloat4 Log2Fastest(Sfloat4 a) { return new Sfloat4(SfMath.Log2Fastest(a.RawX), SfMath.Log2Fastest(a.RawY), SfMath.Log2Fastest(a.RawZ), SfMath.Log2Fastest(a.RawW)); }
        public static Sfloat4 Sin(Sfloat4 a) { return new Sfloat4(SfMath.Sin(a.RawX), SfMath.Sin(a.RawY), SfMath.Sin(a.RawZ), SfMath.Sin(a.RawW)); }
        public static Sfloat4 SinFast(Sfloat4 a) { return new Sfloat4(SfMath.SinFast(a.RawX), SfMath.SinFast(a.RawY), SfMath.SinFast(a.RawZ), SfMath.SinFast(a.RawW)); }
        public static Sfloat4 SinFastest(Sfloat4 a) { return new Sfloat4(SfMath.SinFastest(a.RawX), SfMath.SinFastest(a.RawY), SfMath.SinFastest(a.RawZ), SfMath.SinFastest(a.RawW)); }
        public static Sfloat4 Cos(Sfloat4 a) { return new Sfloat4(SfMath.Cos(a.RawX), SfMath.Cos(a.RawY), SfMath.Cos(a.RawZ), SfMath.Cos(a.RawW)); }
        public static Sfloat4 CosFast(Sfloat4 a) { return new Sfloat4(SfMath.CosFast(a.RawX), SfMath.CosFast(a.RawY), SfMath.CosFast(a.RawZ), SfMath.CosFast(a.RawW)); }
        public static Sfloat4 CosFastest(Sfloat4 a) { return new Sfloat4(SfMath.CosFastest(a.RawX), SfMath.CosFastest(a.RawY), SfMath.CosFastest(a.RawZ), SfMath.CosFastest(a.RawW)); }

        public static Sfloat4 Pow(Sfloat4 a, Sfloat b) { return new Sfloat4(SfMath.Pow(a.RawX, b.Raw), SfMath.Pow(a.RawY, b.Raw), SfMath.Pow(a.RawZ, b.Raw), SfMath.Pow(a.RawW, b.Raw)); }
        public static Sfloat4 PowFast(Sfloat4 a, Sfloat b) { return new Sfloat4(SfMath.PowFast(a.RawX, b.Raw), SfMath.PowFast(a.RawY, b.Raw), SfMath.PowFast(a.RawZ, b.Raw), SfMath.PowFast(a.RawW, b.Raw)); }
        public static Sfloat4 PowFastest(Sfloat4 a, Sfloat b) { return new Sfloat4(SfMath.PowFastest(a.RawX, b.Raw), SfMath.PowFastest(a.RawY, b.Raw), SfMath.PowFastest(a.RawZ, b.Raw), SfMath.PowFastest(a.RawW, b.Raw)); }
        public static Sfloat4 Pow(Sfloat a, Sfloat4 b) { return new Sfloat4(SfMath.Pow(a.Raw, b.RawX), SfMath.Pow(a.Raw, b.RawY), SfMath.Pow(a.Raw, b.RawZ), SfMath.Pow(a.Raw, b.RawW)); }
        public static Sfloat4 PowFast(Sfloat a, Sfloat4 b) { return new Sfloat4(SfMath.PowFast(a.Raw, b.RawX), SfMath.PowFast(a.Raw, b.RawY), SfMath.PowFast(a.Raw, b.RawZ), SfMath.PowFast(a.Raw, b.RawW)); }
        public static Sfloat4 PowFastest(Sfloat a, Sfloat4 b) { return new Sfloat4(SfMath.PowFastest(a.Raw, b.RawX), SfMath.PowFastest(a.Raw, b.RawY), SfMath.PowFastest(a.Raw, b.RawZ), SfMath.PowFastest(a.Raw, b.RawW)); }
        public static Sfloat4 Pow(Sfloat4 a, Sfloat4 b) { return new Sfloat4(SfMath.Pow(a.RawX, b.RawX), SfMath.Pow(a.RawY, b.RawY), SfMath.Pow(a.RawZ, b.RawZ), SfMath.Pow(a.RawW, b.RawW)); }
        public static Sfloat4 PowFast(Sfloat4 a, Sfloat4 b) { return new Sfloat4(SfMath.PowFast(a.RawX, b.RawX), SfMath.PowFast(a.RawY, b.RawY), SfMath.PowFast(a.RawZ, b.RawZ), SfMath.PowFast(a.RawW, b.RawW)); }
        public static Sfloat4 PowFastest(Sfloat4 a, Sfloat4 b) { return new Sfloat4(SfMath.PowFastest(a.RawX, b.RawX), SfMath.PowFastest(a.RawY, b.RawY), SfMath.PowFastest(a.RawZ, b.RawZ), SfMath.PowFastest(a.RawW, b.RawW)); }

        public static Sfloat Length(Sfloat4 a) { return Sfloat.FromRaw(SfMath.Sqrt(SfMath.Mul(a.RawX, a.RawX) + SfMath.Mul(a.RawY, a.RawY) + SfMath.Mul(a.RawZ, a.RawZ) + SfMath.Mul(a.RawW, a.RawW))); }
        public static Sfloat LengthFast(Sfloat4 a) { return Sfloat.FromRaw(SfMath.SqrtFast(SfMath.Mul(a.RawX, a.RawX) + SfMath.Mul(a.RawY, a.RawY) + SfMath.Mul(a.RawZ, a.RawZ) + SfMath.Mul(a.RawW, a.RawW))); }
        public static Sfloat LengthFastest(Sfloat4 a) { return Sfloat.FromRaw(SfMath.SqrtFastest(SfMath.Mul(a.RawX, a.RawX) + SfMath.Mul(a.RawY, a.RawY) + SfMath.Mul(a.RawZ, a.RawZ) + SfMath.Mul(a.RawW, a.RawW))); }
        public static Sfloat LengthSqr(Sfloat4 a) { return Sfloat.FromRaw(SfMath.Mul(a.RawX, a.RawX) + SfMath.Mul(a.RawY, a.RawY) + SfMath.Mul(a.RawZ, a.RawZ) + SfMath.Mul(a.RawW, a.RawW)); }
        public static Sfloat4 Normalize(Sfloat4 a) { Sfloat ooLen = Sfloat.FromRaw(SfMath.RSqrt(SfMath.Mul(a.RawX, a.RawX) + SfMath.Mul(a.RawY, a.RawY) + SfMath.Mul(a.RawZ, a.RawZ) + SfMath.Mul(a.RawW, a.RawW))); return ooLen * a; }
        public static Sfloat4 NormalizeFast(Sfloat4 a) { Sfloat ooLen = Sfloat.FromRaw(SfMath.RSqrtFast(SfMath.Mul(a.RawX, a.RawX) + SfMath.Mul(a.RawY, a.RawY) + SfMath.Mul(a.RawZ, a.RawZ) + SfMath.Mul(a.RawW, a.RawW))); return ooLen * a; }
        public static Sfloat4 NormalizeFastest(Sfloat4 a) { Sfloat ooLen = Sfloat.FromRaw(SfMath.RSqrtFastest(SfMath.Mul(a.RawX, a.RawX) + SfMath.Mul(a.RawY, a.RawY) + SfMath.Mul(a.RawZ, a.RawZ) + SfMath.Mul(a.RawW, a.RawW))); return ooLen * a; }

        public static Sfloat Dot(Sfloat4 a, Sfloat4 b) { return Sfloat.FromRaw(SfMath.Mul(a.RawX, b.RawX) + SfMath.Mul(a.RawY, b.RawY) + SfMath.Mul(a.RawZ, b.RawZ) + SfMath.Mul(a.RawW, b.RawW)); }
        public static Sfloat Distance(Sfloat4 a, Sfloat4 b) { return Length(a - b); }
        public static Sfloat DistanceFast(Sfloat4 a, Sfloat4 b) { return LengthFast(a - b); }
        public static Sfloat DistanceFastest(Sfloat4 a, Sfloat4 b) { return LengthFastest(a - b); }

        public static Sfloat4 Min(Sfloat4 a, Sfloat4 b) { return new Sfloat4(SfMath.Min(a.RawX, b.RawX), SfMath.Min(a.RawY, b.RawY), SfMath.Min(a.RawZ, b.RawZ), SfMath.Min(a.RawW, b.RawW)); }
        public static Sfloat4 Max(Sfloat4 a, Sfloat4 b) { return new Sfloat4(SfMath.Max(a.RawX, b.RawX), SfMath.Max(a.RawY, b.RawY), SfMath.Max(a.RawZ, b.RawZ), SfMath.Max(a.RawW, b.RawW)); }

        public static Sfloat4 Clamp(Sfloat4 a, Sfloat min, Sfloat max)
        {
            return new Sfloat4(
                SfMath.Clamp(a.RawX, min.Raw, max.Raw),
                SfMath.Clamp(a.RawY, min.Raw, max.Raw),
                SfMath.Clamp(a.RawZ, min.Raw, max.Raw),
                SfMath.Clamp(a.RawW, min.Raw, max.Raw));
        }

        public static Sfloat4 Clamp(Sfloat4 a, Sfloat4 min, Sfloat4 max)
        {
            return new Sfloat4(
                SfMath.Clamp(a.RawX, min.RawX, max.RawX),
                SfMath.Clamp(a.RawY, min.RawY, max.RawY),
                SfMath.Clamp(a.RawZ, min.RawZ, max.RawZ),
                SfMath.Clamp(a.RawW, min.RawW, max.RawW));
        }

        public static Sfloat4 Lerp(Sfloat4 a, Sfloat4 b, Sfloat t)
        {
            int tb = t.Raw;
            int ta = SfMath.One - tb;
            return new Sfloat4(
                SfMath.Mul(a.RawX, ta) + SfMath.Mul(b.RawX, tb),
                SfMath.Mul(a.RawY, ta) + SfMath.Mul(b.RawY, tb),
                SfMath.Mul(a.RawZ, ta) + SfMath.Mul(b.RawZ, tb),
                SfMath.Mul(a.RawW, ta) + SfMath.Mul(b.RawW, tb));
        }

        public bool Equals(Sfloat4 other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Sfloat4))
                return false;
            return ((Sfloat4)obj) == this;
        }

        public readonly override string ToString()
        {
            return $"({SfMath.ToString(RawX)}, {SfMath.ToString(RawY)}, {SfMath.ToString(RawZ)}, {SfMath.ToString(RawW)})";
        }

        public readonly string ToString(string format)
        {
            return $"({SfMath.ToString(RawX, format)}, {SfMath.ToString(RawY, format)}, {SfMath.ToString(RawZ, format)}, {SfMath.ToString(RawW, format)})";
        }

        public override int GetHashCode()
        {
            return RawX.GetHashCode() ^ (RawY.GetHashCode() * 7919) ^ (RawZ.GetHashCode() * 4513) ^ (RawW.GetHashCode() * 8923);
        }
    }
}
