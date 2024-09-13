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

namespace Noo.Tools
{
    /// <summary>
    /// Quaternion struct with signed 32.32 fixed point components.
    /// </summary>
    [Serializable]
    public struct SdQuaternion : IEquatable<SdQuaternion>
    {
        // Constants
        public static SdQuaternion Identity { get { return new SdQuaternion(SdMath.Zero, SdMath.Zero, SdMath.Zero, SdMath.One); } }

        public long RawX;
        public long RawY;
        public long RawZ;
        public long RawW;

#pragma warning disable IDE1006 // Naming Styles to match other Vector structs
        public Sdouble x { readonly get { return Sdouble.FromRaw(RawX); } set { RawX = value.Raw; } }
        public Sdouble y { readonly get { return Sdouble.FromRaw(RawY); } set { RawY = value.Raw; } }
        public Sdouble z { readonly get { return Sdouble.FromRaw(RawZ); } set { RawZ = value.Raw; } }
        public Sdouble w { readonly get { return Sdouble.FromRaw(RawW); } set { RawW = value.Raw; } }
#pragma warning restore IDE1006 // Naming Styles

        public SdQuaternion(Sdouble x, Sdouble y, Sdouble z, Sdouble w)
        {
            RawX = x.Raw;
            RawY = y.Raw;
            RawZ = z.Raw;
            RawW = w.Raw;
        }

        public SdQuaternion(Sdouble3 v, Sdouble w)
        {
            RawX = v.RawX;
            RawY = v.RawY;
            RawZ = v.RawZ;
            RawW = w.Raw;
        }

        private SdQuaternion(long x, long y, long z, long w)
        {
            RawX = x;
            RawY = y;
            RawZ = z;
            RawW = w;
        }

        public static SdQuaternion FromAxisAngle(Sdouble3 axis, Sdouble angle)
        {
            Sdouble half_angle = Sdouble.Div2(angle);
            return new SdQuaternion(axis * Sdouble.SinFastest(half_angle), Sdouble.CosFastest(half_angle));
        }

        public static SdQuaternion FromYawPitchRoll(Sdouble yaw_y, Sdouble pitch_x, Sdouble roll_z)
        {
            //  Roll first, about axis the object is facing, then
            //  pitch upward, then yaw to face into the new heading
            Sdouble half_roll = Sdouble.Div2(roll_z);
            Sdouble sr = Sdouble.SinFastest(half_roll);
            Sdouble cr = Sdouble.CosFastest(half_roll);

            Sdouble half_pitch = Sdouble.Div2(pitch_x);
            Sdouble sp = Sdouble.SinFastest(half_pitch);
            Sdouble cp = Sdouble.CosFastest(half_pitch);

            Sdouble half_yaw = Sdouble.Div2(yaw_y);
            Sdouble sy = Sdouble.SinFastest(half_yaw);
            Sdouble cy = Sdouble.CosFastest(half_yaw);

            return new SdQuaternion(
                cy * sp * cr + sy * cp * sr,
                sy * cp * cr - cy * sp * sr,
                cy * cp * sr - sy * sp * cr,
                cy * cp * cr + sy * sp * sr);
        }

        // Creates a unit quaternion that represents the rotation from a to b. a and b do not need to be normalized.
        public static SdQuaternion FromTwoVectors(Sdouble3 a, Sdouble3 b)
        { // From: http://lolengine.net/blog/2014/02/24/quaternion-from-two-vectors-final
            Sdouble epsilon = Sdouble.Ratio(1, 1000000);

            Sdouble norm_a_norm_b = Sdouble.SqrtFastest(Sdouble3.LengthSqr(a) * Sdouble3.LengthSqr(b));
            Sdouble real_part = norm_a_norm_b + Sdouble3.Dot(a, b);

            Sdouble3 v;

            if (real_part < (epsilon * norm_a_norm_b))
            {
                /* If u and v are exactly opposite, rotate 180 degrees
                 * around an arbitrary orthogonal axis. Axis normalization
                 * can happen later, when we normalize the quaternion. */
                real_part = Sdouble.Zero;
                bool cond = Sdouble.Abs(a.x) > Sdouble.Abs(a.z);
                v = cond ? new Sdouble3(-a.y, a.x, Sdouble.Zero)
                         : new Sdouble3(Sdouble.Zero, -a.z, a.y);
            }
            else
            {
                /* Otherwise, build quaternion the standard way. */
                v = Sdouble3.Cross(a, b);
            }

            return NormalizeFastest(new SdQuaternion(v, real_part));
        }

        public static SdQuaternion LookRotation(Sdouble3 dir, Sdouble3 up)
        { // From: https://answers.unity.com/questions/819699/calculate-quaternionlookrotation-manually.html
            if (dir == Sdouble3.Zero)
                return Identity;

            if (up != dir)
            {
                up = Sdouble3.NormalizeFastest(up);
                Sdouble3 v = dir + up * -Sdouble3.Dot(up, dir);
                SdQuaternion q = FromTwoVectors(Sdouble3.AxisZ, v);
                return FromTwoVectors(v, dir) * q;
            }
            else
                return FromTwoVectors(Sdouble3.AxisZ, dir);
        }

        public static SdQuaternion LookAtRotation(Sdouble3 from, Sdouble3 to, Sdouble3 up)
        {
            Sdouble3 dir = Sdouble3.NormalizeFastest(to - from);
            return LookRotation(dir, up);
        }

        // Operators
        public static SdQuaternion operator *(SdQuaternion a, SdQuaternion b) { return Multiply(a, b); }

        public static bool operator ==(SdQuaternion a, SdQuaternion b) { return a.RawX == b.RawX && a.RawY == b.RawY && a.RawZ == b.RawZ && a.RawW == b.RawW; }
        public static bool operator !=(SdQuaternion a, SdQuaternion b) { return a.RawX != b.RawX || a.RawY != b.RawY || a.RawZ != b.RawZ || a.RawW != b.RawW; }

        public static SdQuaternion Negate(SdQuaternion a) { return new SdQuaternion(-a.RawX, -a.RawY, -a.RawZ, -a.RawW); }
        public static SdQuaternion Conjugate(SdQuaternion a) { return new SdQuaternion(-a.RawX, -a.RawY, -a.RawZ, a.RawW); }
        public static SdQuaternion Inverse(SdQuaternion a)
        {
            long inv_norm = Sdouble.Rcp(LengthSqr(a)).Raw;
            return new SdQuaternion(
                -SdMath.Mul(a.RawX, inv_norm),
                -SdMath.Mul(a.RawY, inv_norm),
                -SdMath.Mul(a.RawZ, inv_norm),
                SdMath.Mul(a.RawW, inv_norm));
        }
        // Inverse for unit quaternions
        public static SdQuaternion InverseUnit(SdQuaternion a) { return new SdQuaternion(-a.RawX, -a.RawY, -a.RawZ, a.RawW); }

        public static SdQuaternion Multiply(SdQuaternion value1, SdQuaternion value2)
        {
            Sdouble q1x = value1.x;
            Sdouble q1y = value1.y;
            Sdouble q1z = value1.z;
            Sdouble q1w = value1.w;

            Sdouble q2x = value2.x;
            Sdouble q2y = value2.y;
            Sdouble q2z = value2.z;
            Sdouble q2w = value2.w;

            // cross(av, bv)
            Sdouble cx = q1y * q2z - q1z * q2y;
            Sdouble cy = q1z * q2x - q1x * q2z;
            Sdouble cz = q1x * q2y - q1y * q2x;

            Sdouble dot = q1x * q2x + q1y * q2y + q1z * q2z;

            return new SdQuaternion(
                q1x * q2w + q2x * q1w + cx,
                q1y * q2w + q2y * q1w + cy,
                q1z * q2w + q2z * q1w + cz,
                q1w * q2w - dot);
        }

        public static Sdouble Length(SdQuaternion a) { return Sdouble.Sqrt(LengthSqr(a)); }
        public static Sdouble LengthFast(SdQuaternion a) { return Sdouble.SqrtFast(LengthSqr(a)); }
        public static Sdouble LengthFastest(SdQuaternion a) { return Sdouble.SqrtFastest(LengthSqr(a)); }
        public static Sdouble LengthSqr(SdQuaternion a) { return Sdouble.FromRaw(SdMath.Mul(a.RawX, a.RawX) + SdMath.Mul(a.RawY, a.RawY) + SdMath.Mul(a.RawZ, a.RawZ) + SdMath.Mul(a.RawW, a.RawW)); }
        public static SdQuaternion Normalize(SdQuaternion a)
        {
            long inv_norm = Sdouble.Rcp(Length(a)).Raw;
            return new SdQuaternion(
                SdMath.Mul(a.RawX, inv_norm),
                SdMath.Mul(a.RawY, inv_norm),
                SdMath.Mul(a.RawZ, inv_norm),
                SdMath.Mul(a.RawW, inv_norm));
        }
        public static SdQuaternion NormalizeFast(SdQuaternion a)
        {
            long inv_norm = Sdouble.RcpFast(LengthFast(a)).Raw;
            return new SdQuaternion(
                SdMath.Mul(a.RawX, inv_norm),
                SdMath.Mul(a.RawY, inv_norm),
                SdMath.Mul(a.RawZ, inv_norm),
                SdMath.Mul(a.RawW, inv_norm));
        }
        public static SdQuaternion NormalizeFastest(SdQuaternion a)
        {
            long inv_norm = Sdouble.RcpFastest(LengthFastest(a)).Raw;
            return new SdQuaternion(
                SdMath.Mul(a.RawX, inv_norm),
                SdMath.Mul(a.RawY, inv_norm),
                SdMath.Mul(a.RawZ, inv_norm),
                SdMath.Mul(a.RawW, inv_norm));
        }

        public static SdQuaternion Slerp(SdQuaternion q1, SdQuaternion q2, Sdouble t)
        {
            Sdouble epsilon = Sdouble.Ratio(1, 1000000);
            Sdouble cos_omega = q1.x * q2.x + q1.y * q2.y + q1.z * q2.z + q1.w * q2.w;

            bool flip = false;

            if (cos_omega < 0)
            {
                flip = true;
                cos_omega = -cos_omega;
            }

            Sdouble s1, s2;
            if (cos_omega > (Sdouble.One - epsilon))
            {
                // Too close, do straight linear interpolation.
                s1 = Sdouble.One - t;
                s2 = (flip) ? -t : t;
            }
            else
            {
                Sdouble omega = Sdouble.AcosFastest(cos_omega);
                Sdouble inv_sin_omega = Sdouble.RcpFastest(Sdouble.SinFastest(omega));

                s1 = Sdouble.SinFastest((Sdouble.One - t) * omega) * inv_sin_omega;
                s2 = (flip)
                    ? -Sdouble.SinFastest(t * omega) * inv_sin_omega
                    : Sdouble.SinFastest(t * omega) * inv_sin_omega;
            }

            return new SdQuaternion(
                s1 * q1.x + s2 * q2.x,
                s1 * q1.y + s2 * q2.y,
                s1 * q1.z + s2 * q2.z,
                s1 * q1.w + s2 * q2.w);
        }

        public static SdQuaternion Lerp(SdQuaternion q1, SdQuaternion q2, Sdouble t)
        {
            Sdouble t1 = Sdouble.One - t;
            Sdouble dot = q1.x * q2.x + q1.y * q2.y + q1.z * q2.z + q1.w * q2.w;

            SdQuaternion r;
            if (dot >= 0)
                r = new SdQuaternion(
                    t1 * q1.x + t * q2.x,
                    t1 * q1.y + t * q2.y,
                    t1 * q1.z + t * q2.z,
                    t1 * q1.w + t * q2.w);
            else
                r = new SdQuaternion(
                    t1 * q1.x - t * q2.x,
                    t1 * q1.y - t * q2.y,
                    t1 * q1.z - t * q2.z,
                    t1 * q1.w - t * q2.w);

            return NormalizeFastest(r);
        }

        // Concatenates two Quaternions; the result represents the value1 rotation followed by the value2 rotation.
        public static SdQuaternion Concatenate(SdQuaternion value1, SdQuaternion value2)
        {
            // Concatenate rotation is actually q2 * q1 instead of q1 * q2.
            // So that's why value2 goes q1 and value1 goes q2.
            return Multiply(value2, value1);
        }

        // Rotates a vector by the unit quaternion.
        public static Sdouble3 RotateVector(SdQuaternion rot, Sdouble3 v)
        { // From https://gamedev.stackexchange.com/questions/28395/rotating-vector3-by-a-quaternion
            Sdouble3 u = new (rot.x, rot.y, rot.z);
            Sdouble s = rot.w;

            return
                (Sdouble.Two * Sdouble3.Dot(u, v)) * u +
                (s * s - Sdouble3.Dot(u, u)) * v +
                (Sdouble.Two * s) * Sdouble3.Cross(u, v);
        }

        public bool Equals(SdQuaternion other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            if (obj is not SdQuaternion)
                return false;
            return ((SdQuaternion)obj) == this;
        }

        public override string ToString()
        {
            return "(" + SdMath.ToString(RawX) + ", " + SdMath.ToString(RawY) + ", " + SdMath.ToString(RawZ) + ", " + SdMath.ToString(RawW) + ")";
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() * 7919) ^ (z.GetHashCode() * 4513) ^ (w.GetHashCode() * 8923);
        }
    }
}
