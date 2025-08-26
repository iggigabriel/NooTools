using System.Runtime.CompilerServices;

namespace Noo.Tools
{
    public static partial class SfGeom
    {
        public static Sfloat RadToDeg { [MethodImpl(SfUtil.AggressiveInlining)] get { return Sfloat.FromRaw(0x394BBF); } }
        public static Sfloat DegToRad { [MethodImpl(SfUtil.AggressiveInlining)] get { return Sfloat.FromRaw(0x477); } }

        public static Sfloat Deg90 { [MethodImpl(SfUtil.AggressiveInlining)] get { return Sfloat.FromInt(90); } }
        public static Sfloat DegNeg90 { [MethodImpl(SfUtil.AggressiveInlining)] get { return Sfloat.FromInt(-90); } }
        public static Sfloat Deg180 { [MethodImpl(SfUtil.AggressiveInlining)] get { return Sfloat.FromInt(180); } }
        public static Sfloat DegNeg180 { [MethodImpl(SfUtil.AggressiveInlining)] get { return Sfloat.FromInt(-180); } }
        public static Sfloat Deg270 { [MethodImpl(SfUtil.AggressiveInlining)] get { return Sfloat.FromInt(270); } }
        public static Sfloat Deg360 { [MethodImpl(SfUtil.AggressiveInlining)] get { return Sfloat.FromInt(360); } }

        ///<summary>Returns angle in degrees -180 to 180</summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Sfloat VectorToAngle(Sfloat2 a)
        {
            return Sfloat.Atan2Fast(a.y, a.x) * RadToDeg;
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Sfloat VectorToAngleSafe(Sfloat2 a)
        {
            if (a.RawX == 0 && a.RawY == 0) return default;
            return Sfloat.Atan2Fast(a.y, a.x) * RadToDeg;
        }

        /// <summary>Angle in degrees</summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Sfloat2 AngleToVector(Sfloat angle)
        {
            angle *= DegToRad;

            return new(Sfloat.CosFast(angle), Sfloat.SinFast(angle));
        }

        /// <summary>Angle in degrees</summary>
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Sfloat3 AngleToVector3D(Sfloat angle, Sfloat pitch)
        {
            angle *= DegToRad;
            pitch *= DegToRad;

            var xyLen = Sfloat.CosFast(pitch);
            return new(Sfloat.CosFast(angle) * xyLen, Sfloat.SinFast(angle) * xyLen, Sfloat.Sin(pitch));
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Sfloat Vector3DToPitch(Sfloat3 direction)
        {
            return Sfloat.AsinFast(direction.z) * RadToDeg;
        }

        public static Sfloat2 Rotate(Sfloat2 point, Sfloat degrees)
        {
            var radians = degrees * DegToRad;
            var sin = Sfloat.SinFast(radians);
            var cos = Sfloat.CosFast(radians);
            return new Sfloat2(point.x * cos - point.y * sin, point.x * sin + point.y * cos);
        }

        public static Sfloat2 RotateAround(Sfloat2 point, Sfloat degrees, Sfloat2 pivot)
        {
            var radians = degrees * DegToRad;
            var sin = Sfloat.SinFast(radians);
            var cos = Sfloat.CosFast(radians);
            point -= pivot;
            return new Sfloat2(point.x * cos - point.y * sin + pivot.x, point.x * sin + point.y * cos + pivot.y);
        }

        public static Sfloat Lerp(Sfloat a, Sfloat b, Sfloat t)
        {
            int tb = t.Raw;
            int ta = SfMath.One - tb;
            return Sfloat.FromRaw(SfMath.Mul(a.Raw, ta) + SfMath.Mul(b.Raw, tb));
        }

        // https://gist.github.com/shaunlebron/8832585
        /// <summary>Angle in degrees (returns -180 to +180 degrees)</summary>
        public static Sfloat DeltaAngle(Sfloat a, Sfloat b)
        {
            var diff = NormalizeAngle(b - a);
            return NormalizeAngle(diff << 1) - diff;
        }

        // https://gist.github.com/shaunlebron/8832585
        /// <summary>Angle in degrees</summary>
        public static Sfloat LerpAngle(Sfloat a, Sfloat b, Sfloat t)
        {
            return a + DeltaAngle(a, b) * t;
        }

        /// <summary>Normalizes angle from 0 to 360 degrees</summary>
        public static Sfloat NormalizeAngle(Sfloat angle)
        {
            return Sfloat.Clamp(angle - Sfloat.Floor(angle / Deg360) * Deg360, Sfloat.Zero, Deg360);
        }

        /// <summary>Normalizes angle from -180 to +180 around pivot angle</summary>
        public static Sfloat NormalizeAngleSignedAroundPivot(Sfloat angle, Sfloat pivot)
        {
            angle = angle - pivot + Deg180;
            return Sfloat.Clamp(angle - Sfloat.Floor(angle / Deg360) * Deg360, Sfloat.Zero, Deg360) - Deg180 + pivot;
        }

        /// <summary>Normalizes angle from -180 to +180</summary>
        public static Sfloat NormalizeAngleSigned(Sfloat angle)
        {
            angle += Deg180;
            return Sfloat.Clamp(angle - Sfloat.Floor(angle / Deg360) * Deg360, Sfloat.Zero, Deg360) - Deg180;
        }

        // https://math.stackexchange.com/questions/383321/rotating-x-y-points-45-degrees
        // [cos(45), -sin(45)]
        // [sin(45),  cos(45)]
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Sfloat2 Rotate45(this Sfloat2 vector)
        {
            const int sqrt2rcp = 0xB504; // cos(45) = 1.0/sqrt(2.0) = 0.7071

            return new Sfloat2
            (
                SfMath.Mul(vector.x.Raw + vector.y.Raw, sqrt2rcp),
                SfMath.Mul(vector.y.Raw - vector.x.Raw, sqrt2rcp)
            );
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Sfloat2 Rotate90(this Sfloat2 vector) => new(vector.y, -vector.x);

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Sfloat2 Rotate180(this Sfloat2 vector) => new(-vector.x, -vector.y);

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Sfloat2 Rotate270(this Sfloat2 vector) => new(-vector.y, vector.x);

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Sfloat2 Rotate90X(this Sfloat2 vector, int rotations)
        {
            return (rotations % 4) switch
            {
                -3 => Rotate90(vector),
                -2 => Rotate180(vector),
                -1 => Rotate270(vector),
                0 => vector,
                1 => Rotate90(vector),
                2 => Rotate180(vector),
                3 => Rotate270(vector),
                _ => vector,
            };
        }

        public static bool Overlaps(SfCircle a, SfCircle b)
        {
            var rad = a.radius + b.radius;
            var distanceSqr = (a.origin - b.origin).MagnitudeSqr;
            return distanceSqr < (rad * rad);
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static bool Overlaps(SfRay a, SfRay b)
        {
            return Sfloat2.Cross(a.Direction, b.Direction).Raw == SfMath.Zero;
        }

        public static bool Overlaps(SfRay a, SfRay b, out Sfloat2 intersection, out Sfloat t)
        {
            var adbd = Sfloat2.Cross(a.Direction, b.Direction);

            if (adbd.Raw == SfMath.Zero)
            {
                t = default;
                intersection = default;
                return false;
            }

            t = Sfloat2.Cross(b.origin - a.origin, b.Direction) / adbd;
            intersection = a.origin + t * a.Direction;
            return true;
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static bool Overlaps(SfRay a, SfRay b, out Sfloat2 intersection) => Overlaps(a, b, out intersection, out _);

        public static bool Overlaps(SfLine a, SfLine b, out Sfloat2 intersection)
        {
            var ad = a.Vector;
            var bd = b.Vector;

            var denom = bd.y * ad.x - bd.x * ad.y;

            if (denom.Raw == SfMath.Zero)
            {
                intersection = default;
                return false;
            }

            var y13 = a.p1.y - b.p1.y;
            var x13 = a.p1.x - b.p1.x;

            var uaNum = bd.x * y13 - bd.y * x13;
            var ubNum = ad.x * y13 - ad.y * x13;

            var ua = uaNum / denom;
            var ub = ubNum / denom;

            if (ua.Raw < SfMath.Zero || ua.Raw > SfMath.One || ub.Raw < SfMath.Zero || ub.Raw > SfMath.One)
            {
                intersection = default;
                return false;
            }
            else
            {
                intersection = a.p1 + ad * ua;
                return true;
            }
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static bool Overlaps(SfLine a, SfLine b) => Overlaps(a, b, out _);

        public static bool Overlaps(SfRect rect, SfLine line, out Sfloat t)
        {
            var lineVector = line.Vector;

            var scale = Sfloat2.RcpFast(lineVector);
            var sign = new Sfloat2(Sfloat.FromInt(Sfloat.Sign(scale.x)), Sfloat.FromInt(Sfloat.Sign(scale.y)));

            var center = rect.Center;
            var halfSize = rect.size.Div2();

            var nearTime = (center - sign * halfSize - line.p1) * scale;
            var farTime = (center + sign * halfSize - line.p1) * scale;

            if (nearTime.x > farTime.y || nearTime.y > farTime.x)
            {
                t = default;
                return false;
            }

            var nearTimeMax = nearTime.x > nearTime.y ? nearTime.x : nearTime.y;
            var farTimeMax = farTime.x < farTime.y ? farTime.x : farTime.y;

            if (nearTimeMax >= Sfloat.One || farTimeMax <= Sfloat.Zero)
            {
                t = default;
                return false;
            }

            t = Sfloat.Clamp01(nearTimeMax);

            return true;
        }

        public static bool Overlaps(SfRect r1, SfRect r2)
        {
            return r1.Overlaps(r2);
        }

        public static bool Overlaps(SfRect r1, SfRect r2, out Sfloat2 delta)
        {
            if (!r1.Overlaps(r2))
            {
                delta = default;
                return false;
            }
            else
            {
                var d1 = Sfloat2.Abs(r1.Max - r2.min);
                var d2 = Sfloat2.Abs(r2.Max - r1.min);

                delta = new Sfloat2(d1.x < d2.x ? -d1.x : d2.x, d1.y < d2.y ? -d1.y : d2.y);
                return true;
            }
        }

        public static bool Overlaps(SfRect rect, Sfloat2 point, out Sfloat2 delta)
        {
            if (!rect.Contains(point))
            {
                delta = default;
                return false;
            }
            else
            {
                var d1 = Sfloat2.Abs(point - rect.Max);
                var d2 = Sfloat2.Abs(point - rect.min);

                delta = new Sfloat2(d1.x < d2.x ? d1.x : -d2.x, d1.y < d2.y ? d1.y : -d2.y);
                return true;
            }
        }

        public static bool Overlaps(SfCircle circle, SfRect rect)
        {
            var rectCenter = rect.Center;
            var rectHalfSize = rect.size >> 1;

            var diff = circle.origin - rectCenter;
            var diffAbs = Sfloat2.Abs(diff);

            if (diffAbs.x > (rectHalfSize.x + circle.radius)) return false;
            if (diffAbs.y > (rectHalfSize.y + circle.radius)) return false;

            if (diffAbs.x <= rectHalfSize.x) return true;
            if (diffAbs.y <= rectHalfSize.y) return true;

            var distanceSqr = (diffAbs.x - rectHalfSize.x) * (diffAbs.x - rectHalfSize.x) + (diffAbs.y - rectHalfSize.y) * (diffAbs.y - rectHalfSize.y);

            return distanceSqr <= (circle.radius * circle.radius);
        }

        public static Sfloat2 NearestPointOnRect(Sfloat2 origin, SfRect rect)
        {
            var rectMin = rect.min;
            var rectMax = rect.Max;

            if (origin.x < rectMin.x)
            {
                if (origin.y < rectMin.y) return rectMin;
                else if (origin.y > rectMax.y) return new Sfloat2(rectMin.x, rectMax.y);
                else return new Sfloat2(rectMin.x, origin.y);
            }
            else if (origin.x > rectMax.x)
            {
                if (origin.y < rectMin.y) return new Sfloat2(rectMax.x, rectMin.y);
                else if (origin.y > rectMax.y) return rectMax;
                else return new Sfloat2(rectMax.x, origin.y);
            }
            else
            {
                if (origin.y < rectMin.y) return new Sfloat2(origin.x, rectMin.y);
                else if (origin.y > rectMax.y) return new Sfloat2(origin.x, rectMax.y);
                else
                {
                    var diff = origin - rect.Center;
                    var diffAbs = (rect.size >> 1) - Sfloat2.Abs(diff);

                    if (diffAbs.x < diffAbs.y)
                    {
                        if (diff.x.Raw < 0) return new Sfloat2(rectMin.x, origin.y);
                        else return new Sfloat2(rectMax.x, origin.y);
                    }
                    else
                    {
                        if (diff.y.Raw < 0) return new Sfloat2(origin.x, rectMin.y);
                        else return new Sfloat2(origin.x, rectMax.y);
                    }
                }
            }
        }

        public static Sfloat2 NearestPointOnLine(Sfloat2 origin, SfLine line, out Sfloat t)
        {
            var lineVector = line.Vector;
            t = Sfloat2.Dot(origin - line.p1, lineVector) / lineVector.MagnitudeSqr;
            if (t.Raw < SfMath.Zero) return line.p1;
            else if (t.Raw > SfMath.One) return line.p2;
            else return line.p1 + t * lineVector;
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Sfloat2 NearestPointOnLine(Sfloat2 origin, SfLine line) => NearestPointOnLine(origin, line, out _);

        public static Sfloat2 NearestPointOnRay(Sfloat2 origin, SfRay ray, out Sfloat t)
        {
            t = Sfloat2.Dot(origin - ray.origin, ray.Direction);
            return ray.origin + t * ray.Direction;
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Sfloat2 NearestPointOnRay(Sfloat2 origin, SfRay ray) => NearestPointOnRay(origin, ray, out _);

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static SfLine ShortestLineBetweenPointAndLine(Sfloat2 point, SfLine line)
        {
            return new SfLine(point, NearestPointOnLine(point, line));
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static SfLine ShortestLineBetweenPointAndRay(Sfloat2 point, SfRay ray)
        {
            return new SfLine(point, NearestPointOnRay(point, ray));
        }

        public static SfLine ShortestLineBetweenTwoLines(SfLine a, SfLine b)
        {
            if (Overlaps(a, b, out var intersection)) return new SfLine(intersection, intersection);

            var ap1 = ShortestLineBetweenPointAndLine(a.p1, b);
            var ap2 = ShortestLineBetweenPointAndLine(a.p2, b);
            var bp1 = ShortestLineBetweenPointAndLine(b.p1, a);
            var bp2 = ShortestLineBetweenPointAndLine(b.p2, a);

            var ap1len = ap1.LengthSqr;
            var ap2len = ap2.LengthSqr;
            var bp1len = bp1.LengthSqr;
            var bp2len = bp2.LengthSqr;

            var closestDistance = ap1len;
            var closestLine = ap1;

            if (ap2len < closestDistance)
            {
                closestDistance = ap2len;
                closestLine = ap2;
            }

            if (bp1len < closestDistance)
            {
                closestDistance = bp1len;
                closestLine = bp1.Reversed;
            }

            if (bp2len < closestDistance)
            {
                closestLine = bp2.Reversed;
            }

            return closestLine;
        }

        public static Sfloat ShortestDistanceBetweenTwoLines(SfLine a, SfLine b)
        {
            if (Overlaps(a, b)) return Sfloat.Zero;

            var ap1 = ShortestLineBetweenPointAndLine(a.p1, b).LengthSqr;
            var ap2 = ShortestLineBetweenPointAndLine(a.p2, b).LengthSqr;
            var bp1 = ShortestLineBetweenPointAndLine(b.p1, a).LengthSqr;
            var bp2 = ShortestLineBetweenPointAndLine(b.p2, a).LengthSqr;

            return Sfloat.SqrtFast(Sfloat.Min(Sfloat.Min(Sfloat.Min(ap1, ap2), bp1), bp2));
        }


    }
}
