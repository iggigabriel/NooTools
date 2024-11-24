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
using Unity.Mathematics;

namespace Noo.Tools
{
    /// <summary>
    /// Vector2 struct with signed 16.16 fixed point components.
    /// </summary>
    [Serializable]
    public struct Sfloat2 : IEquatable<Sfloat2>
    {
        // Constants
        public static Sfloat2 Zero { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat2(SfMath.Zero, SfMath.Zero); } }
        public static Sfloat2 One { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat2(SfMath.One, SfMath.One); } }
        public static Sfloat2 Half { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat2(SfMath.Half, SfMath.Half); } }
        public static Sfloat2 Down { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat2(SfMath.Zero, SfMath.NegOne); } }
        public static Sfloat2 Up { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat2(SfMath.Zero, SfMath.One); } }
        public static Sfloat2 Left { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat2(SfMath.NegOne, SfMath.Zero); } }
        public static Sfloat2 Right { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat2(SfMath.One, SfMath.Zero); } }
        public static Sfloat2 AxisX { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat2(SfMath.One, SfMath.Zero); } }
        public static Sfloat2 AxisY { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat2(SfMath.Zero, SfMath.One); } }

        // Raw components
        public int RawX;
        public int RawY;

        // sf accessors
#pragma warning disable IDE1006 // Naming Styles to match other Vector structs
        public Sfloat x { readonly get { return Sfloat.FromRaw(RawX); } set { RawX = value.Raw; } }
        public Sfloat y { readonly get { return Sfloat.FromRaw(RawY); } set { RawY = value.Raw; } }
#pragma warning restore IDE1006 // Naming Styles

        public Sfloat2(Sfloat x, Sfloat y)
        {
            RawX = x.Raw;
            RawY = y.Raw;
        }

        public Sfloat2(int rawX, int rawY)
        {
            RawX = rawX;
            RawY = rawY;
        }

        public readonly Sfloat2 Normalized
        {
            [MethodImpl(SfUtil.AggressiveInlining)]
            get
            {
                return NormalizeFast(this);
            }
        }

        public readonly Sfloat Magnitude
        {
            [MethodImpl(SfUtil.AggressiveInlining)]
            get
            {
                return LengthFast(this);
            }
        }

        public readonly Sfloat MagnitudeSqr
        {
            [MethodImpl(SfUtil.AggressiveInlining)]
            get
            {
                return LengthSqr(this).Sfloat;
            }
        }

        public readonly float2 Float2 { [MethodImpl(SfUtil.AggressiveInlining)] get { return new float2(x.Float, y.Float); } }

        public static Sfloat2 FromRaw(int rawX, int rawY) { return new Sfloat2(rawX, rawY); }
        public static Sfloat2 FromInt(int x, int y) { return new Sfloat2(SfMath.FromInt(x), SfMath.FromInt(y)); }
        public static Sfloat2 FromFloat(float x, float y) { return new Sfloat2(SfMath.FromFloat(x), SfMath.FromFloat(y)); }
        public static Sfloat2 FromDouble(double x, double y) { return new Sfloat2(SfMath.FromDouble(x), SfMath.FromDouble(y)); }

        [MethodImpl(SfUtil.AggressiveInlining)] public readonly Sfloat2 WithX(Sfloat x) => new(x.Raw, RawY);
        [MethodImpl(SfUtil.AggressiveInlining)] public readonly Sfloat2 WithY(Sfloat y) => new(RawX, y.Raw);

        public static Sfloat2 operator -(Sfloat2 a) { return new Sfloat2(-a.RawX, -a.RawY); }
        public static Sfloat2 operator +(Sfloat2 a, Sfloat2 b) { return new Sfloat2(a.RawX + b.RawX, a.RawY + b.RawY); }
        public static Sfloat2 operator -(Sfloat2 a, Sfloat2 b) { return new Sfloat2(a.RawX - b.RawX, a.RawY - b.RawY); }
        public static Sfloat2 operator *(Sfloat2 a, Sfloat2 b) { return new Sfloat2(SfMath.Mul(a.RawX, b.RawX), SfMath.Mul(a.RawY, b.RawY)); }
        public static Sfloat2 operator /(Sfloat2 a, Sfloat2 b) { return new Sfloat2(SfMath.DivPrecise(a.RawX, b.RawX), SfMath.DivPrecise(a.RawY, b.RawY)); }
        public static Sfloat2 operator %(Sfloat2 a, Sfloat2 b) { return new Sfloat2(a.RawX % b.RawX, a.RawY % b.RawY); }

        public static Sfloat2 operator +(Sfloat a, Sfloat2 b) { return new Sfloat2(a.Raw + b.RawX, a.Raw + b.RawY); }
        public static Sfloat2 operator +(Sfloat2 a, Sfloat b) { return new Sfloat2(a.RawX + b.Raw, a.RawY + b.Raw); }
        public static Sfloat2 operator -(Sfloat a, Sfloat2 b) { return new Sfloat2(a.Raw - b.RawX, a.Raw - b.RawY); }
        public static Sfloat2 operator -(Sfloat2 a, Sfloat b) { return new Sfloat2(a.RawX - b.Raw, a.RawY - b.Raw); }
        public static Sfloat2 operator *(Sfloat a, Sfloat2 b) { return new Sfloat2(SfMath.Mul(a.Raw, b.RawX), SfMath.Mul(a.Raw, b.RawY)); }
        public static Sfloat2 operator *(Sfloat2 a, Sfloat b) { return new Sfloat2(SfMath.Mul(a.RawX, b.Raw), SfMath.Mul(a.RawY, b.Raw)); }
        public static Sfloat2 operator /(Sfloat a, Sfloat2 b) { return new Sfloat2(SfMath.DivPrecise(a.Raw, b.RawX), SfMath.DivPrecise(a.Raw, b.RawY)); }
        public static Sfloat2 operator /(Sfloat2 a, Sfloat b) { return new Sfloat2(SfMath.DivPrecise(a.RawX, b.Raw), SfMath.DivPrecise(a.RawY, b.Raw)); }
        public static Sfloat2 operator %(Sfloat a, Sfloat2 b) { return new Sfloat2(a.Raw % b.RawX, a.Raw % b.RawY); }
        public static Sfloat2 operator %(Sfloat2 a, Sfloat b) { return new Sfloat2(a.RawX % b.Raw, a.RawY % b.Raw); }

        public static bool operator ==(Sfloat2 a, Sfloat2 b) { return a.RawX == b.RawX && a.RawY == b.RawY; }
        public static bool operator !=(Sfloat2 a, Sfloat2 b) { return a.RawX != b.RawX || a.RawY != b.RawY; }

        public static Sfloat2 operator >>(Sfloat2 a, int b) { return new Sfloat2(a.RawX >> b, a.RawY >> b); }
        public static Sfloat2 operator <<(Sfloat2 a, int b) { return new Sfloat2(a.RawX << b, a.RawY << b); }

        public readonly Sfloat2 Sign() { return new Sfloat2(Sfloat.FromInt(SfMath.Sign(RawX)), Sfloat.FromInt(SfMath.Sign(RawY))); }
        public static Sfloat2 Abs(Sfloat2 a) { return FromRaw(SfMath.Abs(a.RawX), SfMath.Abs(a.RawY)); }
        public static Sfloat2 Div(Sfloat2 a, Sfloat b) { int oob = SfMath.Rcp(b.Raw); return new Sfloat2(SfMath.Mul(a.RawX, oob), SfMath.Mul(a.RawY, oob)); }
        public static Sfloat2 DivFast(Sfloat2 a, Sfloat b) { int oob = SfMath.RcpFast(b.Raw); return new Sfloat2(SfMath.Mul(a.RawX, oob), SfMath.Mul(a.RawY, oob)); }
        public static Sfloat2 DivFastest(Sfloat2 a, Sfloat b) { int oob = SfMath.RcpFastest(b.Raw); return new Sfloat2(SfMath.Mul(a.RawX, oob), SfMath.Mul(a.RawY, oob)); }
        public static Sfloat2 Div(Sfloat2 a, Sfloat2 b) { return new Sfloat2(SfMath.Div(a.RawX, b.RawX), SfMath.Div(a.RawY, b.RawY)); }
        public static Sfloat2 DivFast(Sfloat2 a, Sfloat2 b) { return new Sfloat2(SfMath.DivFast(a.RawX, b.RawX), SfMath.DivFast(a.RawY, b.RawY)); }
        public static Sfloat2 DivFastest(Sfloat2 a, Sfloat2 b) { return new Sfloat2(SfMath.DivFastest(a.RawX, b.RawX), SfMath.DivFastest(a.RawY, b.RawY)); }
        public static Sfloat2 SqrtPrecise(Sfloat2 a) { return new Sfloat2(SfMath.SqrtPrecise(a.RawX), SfMath.SqrtPrecise(a.RawY)); }
        public static Sfloat2 Sqrt(Sfloat2 a) { return new Sfloat2(SfMath.Sqrt(a.RawX), SfMath.Sqrt(a.RawY)); }
        public static Sfloat2 SqrtFast(Sfloat2 a) { return new Sfloat2(SfMath.SqrtFast(a.RawX), SfMath.SqrtFast(a.RawY)); }
        public static Sfloat2 SqrtFastest(Sfloat2 a) { return new Sfloat2(SfMath.SqrtFastest(a.RawX), SfMath.SqrtFastest(a.RawY)); }
        public static Sfloat2 RSqrt(Sfloat2 a) { return new Sfloat2(SfMath.RSqrt(a.RawX), SfMath.RSqrt(a.RawY)); }
        public static Sfloat2 RSqrtFast(Sfloat2 a) { return new Sfloat2(SfMath.RSqrtFast(a.RawX), SfMath.RSqrtFast(a.RawY)); }
        public static Sfloat2 RSqrtFastest(Sfloat2 a) { return new Sfloat2(SfMath.RSqrtFastest(a.RawX), SfMath.RSqrtFastest(a.RawY)); }
        public static Sfloat2 Rcp(Sfloat2 a) { return new Sfloat2(SfMath.Rcp(a.RawX), SfMath.Rcp(a.RawY)); }
        public static Sfloat2 RcpFast(Sfloat2 a) { return new Sfloat2(SfMath.RcpFast(a.RawX), SfMath.RcpFast(a.RawY)); }
        public static Sfloat2 RcpFastest(Sfloat2 a) { return new Sfloat2(SfMath.RcpFastest(a.RawX), SfMath.RcpFastest(a.RawY)); }
        public static Sfloat2 Exp(Sfloat2 a) { return new Sfloat2(SfMath.Exp(a.RawX), SfMath.Exp(a.RawY)); }
        public static Sfloat2 ExpFast(Sfloat2 a) { return new Sfloat2(SfMath.ExpFast(a.RawX), SfMath.ExpFast(a.RawY)); }
        public static Sfloat2 ExpFastest(Sfloat2 a) { return new Sfloat2(SfMath.ExpFastest(a.RawX), SfMath.ExpFastest(a.RawY)); }
        public static Sfloat2 Exp2(Sfloat2 a) { return new Sfloat2(SfMath.Exp2(a.RawX), SfMath.Exp2(a.RawY)); }
        public static Sfloat2 Exp2Fast(Sfloat2 a) { return new Sfloat2(SfMath.Exp2Fast(a.RawX), SfMath.Exp2Fast(a.RawY)); }
        public static Sfloat2 Exp2Fastest(Sfloat2 a) { return new Sfloat2(SfMath.Exp2Fastest(a.RawX), SfMath.Exp2Fastest(a.RawY)); }
        public static Sfloat2 Log(Sfloat2 a) { return new Sfloat2(SfMath.Log(a.RawX), SfMath.Log(a.RawY)); }
        public static Sfloat2 LogFast(Sfloat2 a) { return new Sfloat2(SfMath.LogFast(a.RawX), SfMath.LogFast(a.RawY)); }
        public static Sfloat2 LogFastest(Sfloat2 a) { return new Sfloat2(SfMath.LogFastest(a.RawX), SfMath.LogFastest(a.RawY)); }
        public static Sfloat2 Log2(Sfloat2 a) { return new Sfloat2(SfMath.Log2(a.RawX), SfMath.Log2(a.RawY)); }
        public static Sfloat2 Log2Fast(Sfloat2 a) { return new Sfloat2(SfMath.Log2Fast(a.RawX), SfMath.Log2Fast(a.RawY)); }
        public static Sfloat2 Log2Fastest(Sfloat2 a) { return new Sfloat2(SfMath.Log2Fastest(a.RawX), SfMath.Log2Fastest(a.RawY)); }
        public static Sfloat2 Sin(Sfloat2 a) { return new Sfloat2(SfMath.Sin(a.RawX), SfMath.Sin(a.RawY)); }
        public static Sfloat2 SinFast(Sfloat2 a) { return new Sfloat2(SfMath.SinFast(a.RawX), SfMath.SinFast(a.RawY)); }
        public static Sfloat2 SinFastest(Sfloat2 a) { return new Sfloat2(SfMath.SinFastest(a.RawX), SfMath.SinFastest(a.RawY)); }
        public static Sfloat2 Cos(Sfloat2 a) { return new Sfloat2(SfMath.Cos(a.RawX), SfMath.Cos(a.RawY)); }
        public static Sfloat2 CosFast(Sfloat2 a) { return new Sfloat2(SfMath.CosFast(a.RawX), SfMath.CosFast(a.RawY)); }
        public static Sfloat2 CosFastest(Sfloat2 a) { return new Sfloat2(SfMath.CosFastest(a.RawX), SfMath.CosFastest(a.RawY)); }

        public static Sfloat2 Pow(Sfloat2 a, Sfloat b) { return new Sfloat2(SfMath.Pow(a.RawX, b.Raw), SfMath.Pow(a.RawY, b.Raw)); }
        public static Sfloat2 PowFast(Sfloat2 a, Sfloat b) { return new Sfloat2(SfMath.PowFast(a.RawX, b.Raw), SfMath.PowFast(a.RawY, b.Raw)); }
        public static Sfloat2 PowFastest(Sfloat2 a, Sfloat b) { return new Sfloat2(SfMath.PowFastest(a.RawX, b.Raw), SfMath.PowFastest(a.RawY, b.Raw)); }
        public static Sfloat2 Pow(Sfloat a, Sfloat2 b) { return new Sfloat2(SfMath.Pow(a.Raw, b.RawX), SfMath.Pow(a.Raw, b.RawY)); }
        public static Sfloat2 PowFast(Sfloat a, Sfloat2 b) { return new Sfloat2(SfMath.PowFast(a.Raw, b.RawX), SfMath.PowFast(a.Raw, b.RawY)); }
        public static Sfloat2 PowFastest(Sfloat a, Sfloat2 b) { return new Sfloat2(SfMath.PowFastest(a.Raw, b.RawX), SfMath.PowFastest(a.Raw, b.RawY)); }
        public static Sfloat2 Pow(Sfloat2 a, Sfloat2 b) { return new Sfloat2(SfMath.Pow(a.RawX, b.RawX), SfMath.Pow(a.RawY, b.RawY)); }
        public static Sfloat2 PowFast(Sfloat2 a, Sfloat2 b) { return new Sfloat2(SfMath.PowFast(a.RawX, b.RawX), SfMath.PowFast(a.RawY, b.RawY)); }
        public static Sfloat2 PowFastest(Sfloat2 a, Sfloat2 b) { return new Sfloat2(SfMath.PowFastest(a.RawX, b.RawX), SfMath.PowFastest(a.RawY, b.RawY)); }

        public static Sfloat Length(Sfloat2 a) { return Sfloat.FromRaw((int)(SdMath.Sqrt((long)a.RawX * (long)a.RawX + (long)a.RawY * (long)a.RawY) >> 16)); }
        public static Sfloat LengthFast(Sfloat2 a) { return Sfloat.FromRaw((int)(SdMath.SqrtFast((long)a.RawX * (long)a.RawX + (long)a.RawY * (long)a.RawY) >> 16)); }
        public static Sfloat LengthFastest(Sfloat2 a) { return Sfloat.FromRaw((int)(SdMath.SqrtFastest((long)a.RawX * (long)a.RawX + (long)a.RawY * (long)a.RawY) >> 16)); }
        public static Sdouble LengthSqr(Sfloat2 a) { return Sdouble.FromRaw((long)a.RawX * (long)a.RawX + (long)a.RawY * (long)a.RawY); }
        public static Sfloat2 Normalize(Sfloat2 a) { Sfloat ooLen = Sfloat.FromRaw((int)(SdMath.RSqrt((long)a.RawX * (long)a.RawX + (long)a.RawY * (long)a.RawY) >> 16)); return ooLen * a; }
        public static Sfloat2 NormalizeFast(Sfloat2 a) { Sfloat ooLen = Sfloat.FromRaw((int)(SdMath.RSqrtFast((long)a.RawX * (long)a.RawX + (long)a.RawY * (long)a.RawY) >> 16)); return ooLen * a; }
        public static Sfloat2 NormalizeFastest(Sfloat2 a) { Sfloat ooLen = Sfloat.FromRaw((int)(SdMath.RSqrtFastest((long)a.RawX * (long)a.RawX + (long)a.RawY * (long)a.RawY) >> 16)); return ooLen * a; }

        public static Sfloat Dot(Sfloat2 a, Sfloat2 b) { return Sfloat.FromRaw(SfMath.Mul(a.RawX, b.RawX) + SfMath.Mul(a.RawY, b.RawY)); }
        public static Sfloat Cross(Sfloat2 a, Sfloat2 b) { return Sfloat.FromRaw(SfMath.Mul(a.RawX, b.RawY) - SfMath.Mul(b.RawX, a.RawY)); }
        public static Sfloat Distance(Sfloat2 a, Sfloat2 b) { return Length(a - b); }
        public static Sfloat DistanceFast(Sfloat2 a, Sfloat2 b) { return LengthFast(a - b); }
        public static Sfloat DistanceFastest(Sfloat2 a, Sfloat2 b) { return LengthFastest(a - b); }

        public static Sfloat2 Min(Sfloat2 a, Sfloat2 b) { return new Sfloat2(SfMath.Min(a.RawX, b.RawX), SfMath.Min(a.RawY, b.RawY)); }
        public static Sfloat2 Max(Sfloat2 a, Sfloat2 b) { return new Sfloat2(SfMath.Max(a.RawX, b.RawX), SfMath.Max(a.RawY, b.RawY)); }

        public static Sfloat2 Ceil(Sfloat2 a) { return FromRaw(SfMath.Ceil(a.x.Raw), SfMath.Ceil(a.y.Raw)); }
        public static Sfloat2 Floor(Sfloat2 a) { return FromRaw(SfMath.Floor(a.x.Raw), SfMath.Floor(a.y.Raw)); }
        public static Sfloat2 Round(Sfloat2 a) { return FromRaw(SfMath.Round(a.x.Raw), SfMath.Round(a.y.Raw)); }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public readonly Sfloat2 Div2() { return new Sfloat2(RawX >> 1, RawY >> 1); }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public readonly Sfloat2 Mul2() { return new Sfloat2(RawX << 1, RawY << 1); }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int2 FloorToInt(Sfloat2 a) { return new int2(SfMath.FloorToInt(a.x.Raw), SfMath.FloorToInt(a.y.Raw)); }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int2 CeilToInt(Sfloat2 a) { return new int2(SfMath.CeilToInt(a.x.Raw), SfMath.CeilToInt(a.y.Raw)); }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static int2 RoundToInt(Sfloat2 a) { return new int2(SfMath.RoundToInt(a.x.Raw), SfMath.RoundToInt(a.y.Raw)); }

        public static Sfloat2 Clamp(Sfloat2 a, Sfloat min, Sfloat max)
        {
            return new Sfloat2(
                SfMath.Clamp(a.RawX, min.Raw, max.Raw),
                SfMath.Clamp(a.RawY, min.Raw, max.Raw));
        }

        public static Sfloat2 Clamp(Sfloat2 a, Sfloat2 min, Sfloat2 max)
        {
            return new Sfloat2(
                SfMath.Clamp(a.RawX, min.RawX, max.RawX),
                SfMath.Clamp(a.RawY, min.RawY, max.RawY));
        }

        public static Sfloat2 ClampLength(Sfloat2 a, Sfloat maxLength)
        {
            if (Length(a) > maxLength) return Normalize(a) * maxLength;
            else return a;
        }

        public static Sfloat2 ClampLengthFast(Sfloat2 a, Sfloat maxLength)
        {
            if (LengthFast(a) > maxLength) return NormalizeFast(a) * maxLength;
            else return a;
        }

        public static Sfloat2 Lerp(Sfloat2 a, Sfloat2 b, Sfloat t)
        {
            int tb = t.Raw;
            int ta = SfMath.One - tb;
            return new Sfloat2(
                SfMath.Mul(a.RawX, ta) + SfMath.Mul(b.RawX, tb),
                SfMath.Mul(a.RawY, ta) + SfMath.Mul(b.RawY, tb));
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Sfloat2 Reflect(Sfloat2 vector, Sfloat2 normal)
        {
            return vector - (Dot(vector, normal) << 1) * normal;
        }

        public readonly Sfloat3 ToSfloat3(Sfloat z = default) => new(RawX, RawY, z.Raw);

        [MethodImpl(SfUtil.AggressiveInlining)]
        public readonly bool IsZeroLength()
        {
            return RawX == 0 && RawY == 0;
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public readonly bool Equals(Sfloat2 other)
        {
            return (this == other);
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public override readonly bool Equals(object obj)
        {
            return obj is Sfloat2 sfloat2 && sfloat2 == this;
        }

        public override readonly string ToString()
        {
            return $"({SfMath.ToString(RawX)}, {SfMath.ToString(RawY)})";
        }

        public readonly string ToString(string format)
        {
            return $"({SfMath.ToString(RawX, format)}, {SfMath.ToString(RawY, format)})";
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public override readonly int GetHashCode()
        {
            return RawX.GetHashCode() ^ RawY.GetHashCode() * 7919;
        }
    }
}
