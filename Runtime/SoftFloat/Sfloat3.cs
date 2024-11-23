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
    /// Vector3 struct with signed 16.16 fixed point components.
    /// </summary>
    [Serializable]
    public struct Sfloat3 : IEquatable<Sfloat3>
    {
        // Constants
        public static Sfloat3 Zero { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat3(SfMath.Zero, SfMath.Zero, SfMath.Zero); } }
        public static Sfloat3 One { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat3(SfMath.One, SfMath.One, SfMath.One); } }
        public static Sfloat3 Down { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat3(SfMath.Zero, SfMath.NegOne, SfMath.Zero); } }
        public static Sfloat3 Up { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat3(SfMath.Zero, SfMath.One, SfMath.Zero); } }
        public static Sfloat3 Left { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat3(SfMath.NegOne, SfMath.Zero, SfMath.Zero); } }
        public static Sfloat3 Right { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat3(SfMath.One, SfMath.Zero, SfMath.Zero); } }
        public static Sfloat3 Forward { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat3(SfMath.Zero, SfMath.Zero, SfMath.One); } }
        public static Sfloat3 Back { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat3(SfMath.Zero, SfMath.Zero, SfMath.NegOne); } }
        public static Sfloat3 AxisX { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat3(SfMath.One, SfMath.Zero, SfMath.Zero); } }
        public static Sfloat3 AxisY { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat3(SfMath.Zero, SfMath.One, SfMath.Zero); } }
        public static Sfloat3 AxisZ { [MethodImpl(SfUtil.AggressiveInlining)] get { return new Sfloat3(SfMath.Zero, SfMath.Zero, SfMath.One); } }

        // Raw components
        public int RawX;
        public int RawY;
        public int RawZ;

        // sf accessors
#pragma warning disable IDE1006 // Naming Styles to match other Vector structs
        public Sfloat x { readonly get { return Sfloat.FromRaw(RawX); } set { RawX = value.Raw; } }
        public Sfloat y { readonly get { return Sfloat.FromRaw(RawY); } set { RawY = value.Raw; } }
        public Sfloat z { readonly get { return Sfloat.FromRaw(RawZ); } set { RawZ = value.Raw; } }

        public Sfloat2 xy { readonly get { return new(RawX, RawY); } set { RawX = value.x.Raw; RawY = value.y.Raw; } }
#pragma warning restore IDE1006 // Naming Styles


        public Sfloat3(Sfloat x, Sfloat y, Sfloat z)
        {
            RawX = x.Raw;
            RawY = y.Raw;
            RawZ = z.Raw;
        }

        public Sfloat3(int rawX, int rawY, int rawZ)
        {
            RawX = rawX;
            RawY = rawY;
            RawZ = rawZ;
        }

        public readonly Sfloat3 Normalized
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

        public static Sfloat3 FromRaw(int rawX, int rawY, int rawZ) { return new Sfloat3(rawX, rawY, rawZ); }
        public static Sfloat3 FromInt(int x, int y, int z) { return new Sfloat3(Sfloat.FromInt(x), Sfloat.FromInt(y), Sfloat.FromInt(z)); }
        public static Sfloat3 FromFloat(float x, float y, float z) { return new Sfloat3(Sfloat.FromFloat(x), Sfloat.FromFloat(y), Sfloat.FromFloat(z)); }
        public static Sfloat3 FromDouble(double x, double y, double z) { return new Sfloat3(Sfloat.FromDouble(x), Sfloat.FromDouble(y), Sfloat.FromDouble(z)); }
        public static Sfloat3 FromVector3(UnityEngine.Vector3 vector) { return new Sfloat3(Sfloat.FromFloat(vector.x), Sfloat.FromFloat(vector.y), Sfloat.FromFloat(vector.z)); }
        public static Sfloat3 FromFloat3(float3 vector) { return new Sfloat3(Sfloat.FromFloat(vector.x), Sfloat.FromFloat(vector.y), Sfloat.FromFloat(vector.z)); }

        public static Sfloat3 operator -(Sfloat3 a) { return new Sfloat3(-a.RawX, -a.RawY, -a.RawZ); }
        public static Sfloat3 operator +(Sfloat3 a, Sfloat3 b) { return new Sfloat3(a.RawX + b.RawX, a.RawY + b.RawY, a.RawZ + b.RawZ); }
        public static Sfloat3 operator -(Sfloat3 a, Sfloat3 b) { return new Sfloat3(a.RawX - b.RawX, a.RawY - b.RawY, a.RawZ - b.RawZ); }
        public static Sfloat3 operator *(Sfloat3 a, Sfloat3 b) { return new Sfloat3(SfMath.Mul(a.RawX, b.RawX), SfMath.Mul(a.RawY, b.RawY), SfMath.Mul(a.RawZ, b.RawZ)); }
        public static Sfloat3 operator /(Sfloat3 a, Sfloat3 b) { return new Sfloat3(SfMath.DivPrecise(a.RawX, b.RawX), SfMath.DivPrecise(a.RawY, b.RawY), SfMath.DivPrecise(a.RawZ, b.RawZ)); }
        public static Sfloat3 operator %(Sfloat3 a, Sfloat3 b) { return new Sfloat3(a.RawX % b.RawX, a.RawY % b.RawY, a.RawZ % b.RawZ); }

        public static Sfloat3 operator +(Sfloat a, Sfloat3 b) { return new Sfloat3(a.Raw + b.RawX, a.Raw + b.RawY, a.Raw + b.RawZ); }
        public static Sfloat3 operator +(Sfloat3 a, Sfloat b) { return new Sfloat3(a.RawX + b.Raw, a.RawY + b.Raw, a.RawZ + b.Raw); }
        public static Sfloat3 operator -(Sfloat a, Sfloat3 b) { return new Sfloat3(a.Raw - b.RawX, a.Raw - b.RawY, a.Raw - b.RawZ); }
        public static Sfloat3 operator -(Sfloat3 a, Sfloat b) { return new Sfloat3(a.RawX - b.Raw, a.RawY - b.Raw, a.RawZ - b.Raw); }
        public static Sfloat3 operator *(Sfloat a, Sfloat3 b) { return new Sfloat3(SfMath.Mul(a.Raw, b.RawX), SfMath.Mul(a.Raw, b.RawY), SfMath.Mul(a.Raw, b.RawZ)); }
        public static Sfloat3 operator *(Sfloat3 a, Sfloat b) { return new Sfloat3(SfMath.Mul(a.RawX, b.Raw), SfMath.Mul(a.RawY, b.Raw), SfMath.Mul(a.RawZ, b.Raw)); }
        public static Sfloat3 operator /(Sfloat a, Sfloat3 b) { return new Sfloat3(SfMath.DivPrecise(a.Raw, b.RawX), SfMath.DivPrecise(a.Raw, b.RawY), SfMath.DivPrecise(a.Raw, b.RawZ)); }
        public static Sfloat3 operator /(Sfloat3 a, Sfloat b) { return new Sfloat3(SfMath.DivPrecise(a.RawX, b.Raw), SfMath.DivPrecise(a.RawY, b.Raw), SfMath.DivPrecise(a.RawZ, b.Raw)); }
        public static Sfloat3 operator %(Sfloat a, Sfloat3 b) { return new Sfloat3(a.Raw % b.RawX, a.Raw % b.RawY, a.Raw % b.RawZ); }
        public static Sfloat3 operator %(Sfloat3 a, Sfloat b) { return new Sfloat3(a.RawX % b.Raw, a.RawY % b.Raw, a.RawZ % b.Raw); }

        public static bool operator ==(Sfloat3 a, Sfloat3 b) { return a.RawX == b.RawX && a.RawY == b.RawY && a.RawZ == b.RawZ; }
        public static bool operator !=(Sfloat3 a, Sfloat3 b) { return a.RawX != b.RawX || a.RawY != b.RawY || a.RawZ != b.RawZ; }

        public static Sfloat3 Div(Sfloat3 a, Sfloat b) { int oob = SfMath.Rcp(b.Raw); return new Sfloat3(SfMath.Mul(a.RawX, oob), SfMath.Mul(a.RawY, oob), SfMath.Mul(a.RawZ, oob)); }
        public static Sfloat3 DivFast(Sfloat3 a, Sfloat b) { int oob = SfMath.RcpFast(b.Raw); return new Sfloat3(SfMath.Mul(a.RawX, oob), SfMath.Mul(a.RawY, oob), SfMath.Mul(a.RawZ, oob)); }
        public static Sfloat3 DivFastest(Sfloat3 a, Sfloat b) { int oob = SfMath.RcpFastest(b.Raw); return new Sfloat3(SfMath.Mul(a.RawX, oob), SfMath.Mul(a.RawY, oob), SfMath.Mul(a.RawZ, oob)); }
        public static Sfloat3 Div(Sfloat3 a, Sfloat3 b) { return new Sfloat3(SfMath.Div(a.RawX, b.RawX), SfMath.Div(a.RawY, b.RawY), SfMath.Div(a.RawZ, b.RawZ)); }
        public static Sfloat3 DivFast(Sfloat3 a, Sfloat3 b) { return new Sfloat3(SfMath.DivFast(a.RawX, b.RawX), SfMath.DivFast(a.RawY, b.RawY), SfMath.DivFast(a.RawZ, b.RawZ)); }
        public static Sfloat3 DivFastest(Sfloat3 a, Sfloat3 b) { return new Sfloat3(SfMath.DivFastest(a.RawX, b.RawX), SfMath.DivFastest(a.RawY, b.RawY), SfMath.DivFastest(a.RawZ, b.RawZ)); }
        public static Sfloat3 SqrtPrecise(Sfloat3 a) { return new Sfloat3(SfMath.SqrtPrecise(a.RawX), SfMath.SqrtPrecise(a.RawY), SfMath.SqrtPrecise(a.RawZ)); }
        public static Sfloat3 Sqrt(Sfloat3 a) { return new Sfloat3(SfMath.Sqrt(a.RawX), SfMath.Sqrt(a.RawY), SfMath.Sqrt(a.RawZ)); }
        public static Sfloat3 SqrtFast(Sfloat3 a) { return new Sfloat3(SfMath.SqrtFast(a.RawX), SfMath.SqrtFast(a.RawY), SfMath.SqrtFast(a.RawZ)); }
        public static Sfloat3 SqrtFastest(Sfloat3 a) { return new Sfloat3(SfMath.SqrtFastest(a.RawX), SfMath.SqrtFastest(a.RawY), SfMath.SqrtFastest(a.RawZ)); }
        public static Sfloat3 RSqrt(Sfloat3 a) { return new Sfloat3(SfMath.RSqrt(a.RawX), SfMath.RSqrt(a.RawY), SfMath.RSqrt(a.RawZ)); }
        public static Sfloat3 RSqrtFast(Sfloat3 a) { return new Sfloat3(SfMath.RSqrtFast(a.RawX), SfMath.RSqrtFast(a.RawY), SfMath.RSqrtFast(a.RawZ)); }
        public static Sfloat3 RSqrtFastest(Sfloat3 a) { return new Sfloat3(SfMath.RSqrtFastest(a.RawX), SfMath.RSqrtFastest(a.RawY), SfMath.RSqrtFastest(a.RawZ)); }
        public static Sfloat3 Rcp(Sfloat3 a) { return new Sfloat3(SfMath.Rcp(a.RawX), SfMath.Rcp(a.RawY), SfMath.Rcp(a.RawZ)); }
        public static Sfloat3 RcpFast(Sfloat3 a) { return new Sfloat3(SfMath.RcpFast(a.RawX), SfMath.RcpFast(a.RawY), SfMath.RcpFast(a.RawZ)); }
        public static Sfloat3 RcpFastest(Sfloat3 a) { return new Sfloat3(SfMath.RcpFastest(a.RawX), SfMath.RcpFastest(a.RawY), SfMath.RcpFastest(a.RawZ)); }
        public static Sfloat3 Exp(Sfloat3 a) { return new Sfloat3(SfMath.Exp(a.RawX), SfMath.Exp(a.RawY), SfMath.Exp(a.RawZ)); }
        public static Sfloat3 ExpFast(Sfloat3 a) { return new Sfloat3(SfMath.ExpFast(a.RawX), SfMath.ExpFast(a.RawY), SfMath.ExpFast(a.RawZ)); }
        public static Sfloat3 ExpFastest(Sfloat3 a) { return new Sfloat3(SfMath.ExpFastest(a.RawX), SfMath.ExpFastest(a.RawY), SfMath.ExpFastest(a.RawZ)); }
        public static Sfloat3 Exp2(Sfloat3 a) { return new Sfloat3(SfMath.Exp2(a.RawX), SfMath.Exp2(a.RawY), SfMath.Exp2(a.RawZ)); }
        public static Sfloat3 Exp2Fast(Sfloat3 a) { return new Sfloat3(SfMath.Exp2Fast(a.RawX), SfMath.Exp2Fast(a.RawY), SfMath.Exp2Fast(a.RawZ)); }
        public static Sfloat3 Exp2Fastest(Sfloat3 a) { return new Sfloat3(SfMath.Exp2Fastest(a.RawX), SfMath.Exp2Fastest(a.RawY), SfMath.Exp2Fastest(a.RawZ)); }
        public static Sfloat3 Log(Sfloat3 a) { return new Sfloat3(SfMath.Log(a.RawX), SfMath.Log(a.RawY), SfMath.Log(a.RawZ)); }
        public static Sfloat3 LogFast(Sfloat3 a) { return new Sfloat3(SfMath.LogFast(a.RawX), SfMath.LogFast(a.RawY), SfMath.LogFast(a.RawZ)); }
        public static Sfloat3 LogFastest(Sfloat3 a) { return new Sfloat3(SfMath.LogFastest(a.RawX), SfMath.LogFastest(a.RawY), SfMath.LogFastest(a.RawZ)); }
        public static Sfloat3 Log2(Sfloat3 a) { return new Sfloat3(SfMath.Log2(a.RawX), SfMath.Log2(a.RawY), SfMath.Log2(a.RawZ)); }
        public static Sfloat3 Log2Fast(Sfloat3 a) { return new Sfloat3(SfMath.Log2Fast(a.RawX), SfMath.Log2Fast(a.RawY), SfMath.Log2Fast(a.RawZ)); }
        public static Sfloat3 Log2Fastest(Sfloat3 a) { return new Sfloat3(SfMath.Log2Fastest(a.RawX), SfMath.Log2Fastest(a.RawY), SfMath.Log2Fastest(a.RawZ)); }
        public static Sfloat3 Sin(Sfloat3 a) { return new Sfloat3(SfMath.Sin(a.RawX), SfMath.Sin(a.RawY), SfMath.Sin(a.RawZ)); }
        public static Sfloat3 SinFast(Sfloat3 a) { return new Sfloat3(SfMath.SinFast(a.RawX), SfMath.SinFast(a.RawY), SfMath.SinFast(a.RawZ)); }
        public static Sfloat3 SinFastest(Sfloat3 a) { return new Sfloat3(SfMath.SinFastest(a.RawX), SfMath.SinFastest(a.RawY), SfMath.SinFastest(a.RawZ)); }
        public static Sfloat3 Cos(Sfloat3 a) { return new Sfloat3(SfMath.Cos(a.RawX), SfMath.Cos(a.RawY), SfMath.Cos(a.RawZ)); }
        public static Sfloat3 CosFast(Sfloat3 a) { return new Sfloat3(SfMath.CosFast(a.RawX), SfMath.CosFast(a.RawY), SfMath.CosFast(a.RawZ)); }
        public static Sfloat3 CosFastest(Sfloat3 a) { return new Sfloat3(SfMath.CosFastest(a.RawX), SfMath.CosFastest(a.RawY), SfMath.CosFastest(a.RawZ)); }

        public static Sfloat3 Pow(Sfloat3 a, Sfloat b) { return new Sfloat3(SfMath.Pow(a.RawX, b.Raw), SfMath.Pow(a.RawY, b.Raw), SfMath.Pow(a.RawZ, b.Raw)); }
        public static Sfloat3 PowFast(Sfloat3 a, Sfloat b) { return new Sfloat3(SfMath.PowFast(a.RawX, b.Raw), SfMath.PowFast(a.RawY, b.Raw), SfMath.PowFast(a.RawZ, b.Raw)); }
        public static Sfloat3 PowFastest(Sfloat3 a, Sfloat b) { return new Sfloat3(SfMath.PowFastest(a.RawX, b.Raw), SfMath.PowFastest(a.RawY, b.Raw), SfMath.PowFastest(a.RawZ, b.Raw)); }
        public static Sfloat3 Pow(Sfloat a, Sfloat3 b) { return new Sfloat3(SfMath.Pow(a.Raw, b.RawX), SfMath.Pow(a.Raw, b.RawY), SfMath.Pow(a.Raw, b.RawZ)); }
        public static Sfloat3 PowFast(Sfloat a, Sfloat3 b) { return new Sfloat3(SfMath.PowFast(a.Raw, b.RawX), SfMath.PowFast(a.Raw, b.RawY), SfMath.PowFast(a.Raw, b.RawZ)); }
        public static Sfloat3 PowFastest(Sfloat a, Sfloat3 b) { return new Sfloat3(SfMath.PowFastest(a.Raw, b.RawX), SfMath.PowFastest(a.Raw, b.RawY), SfMath.PowFastest(a.Raw, b.RawZ)); }
        public static Sfloat3 Pow(Sfloat3 a, Sfloat3 b) { return new Sfloat3(SfMath.Pow(a.RawX, b.RawX), SfMath.Pow(a.RawY, b.RawY), SfMath.Pow(a.RawZ, b.RawZ)); }
        public static Sfloat3 PowFast(Sfloat3 a, Sfloat3 b) { return new Sfloat3(SfMath.PowFast(a.RawX, b.RawX), SfMath.PowFast(a.RawY, b.RawY), SfMath.PowFast(a.RawZ, b.RawZ)); }
        public static Sfloat3 PowFastest(Sfloat3 a, Sfloat3 b) { return new Sfloat3(SfMath.PowFastest(a.RawX, b.RawX), SfMath.PowFastest(a.RawY, b.RawY), SfMath.PowFastest(a.RawZ, b.RawZ)); }

        public static Sfloat Length(Sfloat3 a) { return Sfloat.FromRaw((int)(SdMath.Sqrt((long)a.RawX * (long)a.RawX + (long)a.RawY * (long)a.RawY + (long)a.RawZ * (long)a.RawZ) >> 16)); }
        public static Sfloat LengthFast(Sfloat3 a) { return Sfloat.FromRaw((int)(SdMath.SqrtFast((long)a.RawX * (long)a.RawX + (long)a.RawY * (long)a.RawY + (long)a.RawZ * (long)a.RawZ) >> 16)); }
        public static Sfloat LengthFastest(Sfloat3 a) { return Sfloat.FromRaw((int)(SdMath.SqrtFastest((long)a.RawX * (long)a.RawX + (long)a.RawY * (long)a.RawY + (long)a.RawZ * (long)a.RawZ) >> 16)); }
        public static Sdouble LengthSqr(Sfloat3 a) { return Sdouble.FromRaw((long)a.RawX * (long)a.RawX + (long)a.RawY * (long)a.RawY + (long)a.RawZ * (long)a.RawZ); }
        public static Sfloat3 Normalize(Sfloat3 a) { Sfloat ooLen = Sfloat.FromRaw((int)(SdMath.RSqrt((long)a.RawX * (long)a.RawX + (long)a.RawY * (long)a.RawY + (long)a.RawZ * (long)a.RawZ) >> 16)); return ooLen * a; }
        public static Sfloat3 NormalizeFast(Sfloat3 a) { Sfloat ooLen = Sfloat.FromRaw((int)(SdMath.RSqrtFast((long)a.RawX * (long)a.RawX + (long)a.RawY * (long)a.RawY + (long)a.RawZ * (long)a.RawZ) >> 16)); return ooLen * a; }
        public static Sfloat3 NormalizeFastest(Sfloat3 a) { Sfloat ooLen = Sfloat.FromRaw((int)(SdMath.RSqrtFastest((long)a.RawX * (long)a.RawX + (long)a.RawY * (long)a.RawY + (long)a.RawZ * (long)a.RawZ) >> 16)); return ooLen * a; }

        public static Sfloat Dot(Sfloat3 a, Sfloat3 b) { return Sfloat.FromRaw(SfMath.Mul(a.RawX, b.RawX) + SfMath.Mul(a.RawY, b.RawY) + SfMath.Mul(a.RawZ, b.RawZ)); }
        public static Sfloat Distance(Sfloat3 a, Sfloat3 b) { return Length(a - b); }
        public static Sfloat DistanceFast(Sfloat3 a, Sfloat3 b) { return LengthFast(a - b); }
        public static Sfloat DistanceFastest(Sfloat3 a, Sfloat3 b) { return LengthFastest(a - b); }

        public static Sfloat3 Min(Sfloat3 a, Sfloat3 b) { return new Sfloat3(SfMath.Min(a.RawX, b.RawX), SfMath.Min(a.RawY, b.RawY), SfMath.Min(a.RawZ, b.RawZ)); }
        public static Sfloat3 Max(Sfloat3 a, Sfloat3 b) { return new Sfloat3(SfMath.Max(a.RawX, b.RawX), SfMath.Max(a.RawY, b.RawY), SfMath.Max(a.RawZ, b.RawZ)); }

        public static Sfloat3 Clamp(Sfloat3 a, Sfloat min, Sfloat max)
        {
            return new Sfloat3(
                SfMath.Clamp(a.RawX, min.Raw, max.Raw),
                SfMath.Clamp(a.RawY, min.Raw, max.Raw),
                SfMath.Clamp(a.RawZ, min.Raw, max.Raw));
        }

        public static Sfloat3 Clamp(Sfloat3 a, Sfloat3 min, Sfloat3 max)
        {
            return new Sfloat3(
                SfMath.Clamp(a.RawX, min.RawX, max.RawX),
                SfMath.Clamp(a.RawY, min.RawY, max.RawY),
                SfMath.Clamp(a.RawZ, min.RawZ, max.RawZ));
        }

        public static Sfloat3 Lerp(Sfloat3 a, Sfloat3 b, Sfloat t)
        {
            int tb = t.Raw;
            int ta = SfMath.One - tb;
            return new Sfloat3(
                SfMath.Mul(a.RawX, ta) + SfMath.Mul(b.RawX, tb),
                SfMath.Mul(a.RawY, ta) + SfMath.Mul(b.RawY, tb),
                SfMath.Mul(a.RawZ, ta) + SfMath.Mul(b.RawZ, tb));
        }

        public static Sfloat3 Cross(Sfloat3 a, Sfloat3 b)
        {
            return new Sfloat3(
                SfMath.Mul(a.RawY, b.RawZ) - SfMath.Mul(a.RawZ, b.RawY),
                SfMath.Mul(a.RawZ, b.RawX) - SfMath.Mul(a.RawX, b.RawZ),
                SfMath.Mul(a.RawX, b.RawY) - SfMath.Mul(a.RawY, b.RawX));
        }

        public bool Equals(Sfloat3 other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Sfloat3))
                return false;
            return ((Sfloat3)obj) == this;
        }

        public readonly override string ToString()
        {
            return $"({SfMath.ToString(RawX)}, {SfMath.ToString(RawY)}, {SfMath.ToString(RawZ)})";
        }

        public readonly string ToString(string format)
        {
            return $"({SfMath.ToString(RawX, format)}, {SfMath.ToString(RawY, format)}, {SfMath.ToString(RawZ, format)})";
        }

        public override readonly int GetHashCode()
        {
            return RawX.GetHashCode() ^ RawY.GetHashCode() * 7919 ^ RawZ.GetHashCode() * 4513;
        }
    }
}
