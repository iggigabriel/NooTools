using System;
using System.Runtime.CompilerServices;
using UnityEngine.Assertions.Must;

namespace Noo.Tools
{
    public readonly struct SfSweepTestHit
    {
        /// <summary>Fraction from 0 to 1 indicating how far along the line the collision occurred</summary>
        public readonly Sfloat t;

        /// <summary>Intersection point of colliders</summary>
        public readonly Sfloat2 point;

        /// <summary>Position of object in time of collision</summary>
        public readonly Sfloat2 centroid;

        /// <summary>Normalized reversed direction of hit</summary>
        public readonly Sfloat2 normal;

        public SfSweepTestHit(Sfloat t, Sfloat2 point, Sfloat2 centroid)
        {
            this.t = t;
            this.point = point;
            this.centroid = centroid;
            var diff = centroid - point;
            normal = diff.IsZeroLength() ? Sfloat2.Right : Sfloat2.NormalizeFast(diff);
        }

        public SfSweepTestHit(Sfloat t, Sfloat2 point, Sfloat2 centroid, Sfloat2 normal)
        {
            this.t = t;
            this.point = point;
            this.centroid = centroid;
            this.normal = normal;
        }
    }

    // Sweep tests
    public static partial class SfGeom
    {
        public static bool SweepCircleVsPoint(SfCircle c1, Sfloat2 c1velocity, Sfloat2 p1)
        {
            var closestVector = ShortestLineBetweenPointAndLine(p1, new SfLine(c1.origin, c1.origin + c1velocity));
            return (c1.radius * c1.radius) >= closestVector.LengthSqr;
        }

        public static bool SweepCircleVsPoint(SfCircle c1, Sfloat2 c1velocity, Sfloat2 p1, out SfSweepTestHit hit)
        {
            var c1RadiusSqr = c1.radius * c1.radius;
            var c1p1 = c1.origin - p1;

            // Already interesecting
            if (Sfloat2.LengthSqr(c1p1) <= c1RadiusSqr)
            {
                hit = new SfSweepTestHit(Sfloat.Zero, p1, c1.origin);
                return true;
            }

            var c1VelLen = Sfloat2.LengthFast(c1velocity);
            var c1VelRay = new SfRay(c1.origin, c1velocity / c1VelLen, true);
            var p2 = NearestPointOnRay(p1, c1VelRay); // Closest point on velocity ray
            var p1p2 = p2 - p1;

            var p1p2LenSqr = Sfloat2.LengthSqr(p1p2).Sfloat;

            // far away or behind starting point
            if (p1p2LenSqr > c1RadiusSqr || Sfloat2.Dot(c1velocity, c1p1) > Sfloat.Zero)
            {
                hit = default;
                return false;
            }

            var backDistance = Sfloat.SqrtFast(c1RadiusSqr - p1p2LenSqr);
            var hitCentroid = p2 - c1VelRay.Direction * backDistance;
            var t = Sfloat2.LengthFast(hitCentroid - c1.origin) / c1VelLen;

            // behind last point
            if (t.Raw > SfMath.One)
            {
                hit = default;
                return false;
            }

            hit = new SfSweepTestHit(t, p1, hitCentroid);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SweepCircleVsCircle(SfCircle c1, Sfloat2 c1velocity, SfCircle c2)
        {
            return SweepCircleVsPoint(new SfCircle(c1.origin, c1.radius + c2.radius), c1velocity, c2.origin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SweepCircleVsCircle(SfCircle c1, Sfloat2 c1velocity, SfCircle c2, out SfSweepTestHit hit)
        {
            if (SweepCircleVsPoint(new SfCircle(c1.origin, c1.radius + c2.radius), c1velocity, c2.origin, out hit))
            {
                hit = new SfSweepTestHit(hit.t, hit.centroid - hit.normal * c1.radius, hit.centroid);
                return true;
            }
            else
            {
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SweepCircleVsSweepCircle(SfCircle c1, Sfloat2 c1velocity, SfCircle c2, Sfloat2 c2velocity)
        {
            return SweepCircleVsPoint(new SfCircle(c1.origin, c1.radius + c2.radius), c1velocity - c2velocity, c2.origin);
        }

        public static bool SweepCircleVsSweepCircle(SfCircle c1, Sfloat2 c1velocity, SfCircle c2, Sfloat2 c2velocity, out SfSweepTestHit hit)
        {
            if (SweepCircleVsPoint(new SfCircle(c1.origin, c1.radius + c2.radius), c1velocity - c2velocity, c2.origin, out hit))
            {
                var centroid = hit.centroid + c2velocity * hit.t;
                hit = new SfSweepTestHit(hit.t, centroid - hit.normal * c1.radius, centroid);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool SweepCircleVsLine(SfCircle c1, Sfloat2 c1velocity, SfLine l1, out SfSweepTestHit hit)
        {
            hit = default;
            var hitFound = false;

            var c1RadiusSqr = c1.radius * c1.radius;

            var c1VelocityLine = new SfLine(c1.origin, c1.origin + c1velocity);
            var l1Ray = l1.ToRay(out var l1Length);

            var c1l1NearestOnLine = NearestPointOnLine(c1.origin, l1, out var c1l1nearestT);

            // Already interesecting
            if (Sfloat2.LengthSqr(c1l1NearestOnLine - c1.origin) <= c1RadiusSqr)
            {
                hit = new SfSweepTestHit(Sfloat.Zero, c1l1NearestOnLine, c1.origin);
                return true;
            }

            var c1VelocityRay = c1VelocityLine.ToRay(out var c1VelocityLength);
            var c1l1 = new SfLine(c1.origin, l1.p1 + l1.Vector * c1l1nearestT);

            // With segment
            if (Overlaps(c1VelocityRay, l1Ray, out var _, out var l1c1Distance))
            {
                var ratio = (Sfloat.One - c1.radius / c1l1.Length) * l1c1Distance;
                var hitT = ratio / c1VelocityLength;

                if (hitT.Raw >= SfMath.Zero && hitT.Raw <= SfMath.One)
                {
                    var hitCentroid = c1VelocityRay.GetPoint(ratio);
                    var hitPoint = hitCentroid + c1l1.Direction * c1.radius;
                    var l1HitDistance = l1Ray.GetDistance(hitPoint);

                    if (l1HitDistance.Raw >= SfMath.Zero && l1HitDistance <= l1Length)
                    {
                        hit = new SfSweepTestHit(hitT, hitPoint, hitCentroid);
                        hitFound = true;
                    }
                }
            }

            // Endpoint p1
            var c1l1p1 = c1.origin - l1.p1;
            var c1l1q1 = NearestPointOnRay(l1.p1, c1VelocityRay);
            var l1p1q1 = c1l1q1 - l1.p1;
            var l1p1q1LenghtSqr = l1p1q1.MagnitudeSqr;

            if (l1p1q1LenghtSqr <= c1RadiusSqr && Sfloat2.Dot(c1velocity, c1l1p1).Raw <= SfMath.Zero)
            {
                var l1p1BackDistance = Sfloat.SqrtFast(c1RadiusSqr - l1p1q1LenghtSqr);
                var hitCentroid = c1l1q1 - c1VelocityRay.Direction * l1p1BackDistance;
                var hitT = Sfloat2.LengthFast(hitCentroid - c1.origin) / c1VelocityLength;

                if (hitT.Raw <= SfMath.One && (!hitFound || hit.t > hitT))
                {
                    hitFound = true;
                    hit = new SfSweepTestHit(hitT, l1.p1, hitCentroid);
                }
            }

            // Endpoint p2
            var c1l1p2 = c1.origin - l1.p2;
            var c1l1q2 = NearestPointOnRay(l1.p2, c1VelocityRay);
            var l1p2q2 = c1l1q2 - l1.p2;
            var l1p2q2LenghtSqr = l1p2q2.MagnitudeSqr;

            if (l1p2q2LenghtSqr <= c1RadiusSqr && Sfloat2.Dot(c1velocity, c1l1p2).Raw <= SfMath.Zero)
            {
                var l1p2BackDistance = Sfloat.SqrtFast(c1RadiusSqr - l1p2q2LenghtSqr);
                var hitCentroid = c1l1q2 - c1VelocityRay.Direction * l1p2BackDistance;
                var hitT = Sfloat2.LengthFast(hitCentroid - c1.origin) / c1VelocityLength;

                if (hitT.Raw <= SfMath.One && (!hitFound || hit.t > hitT))
                {
                    hitFound = true;
                    hit = new SfSweepTestHit(hitT, l1.p2, hitCentroid);
                }
            }

            return hitFound;
        }

        public static bool SweepCircleVsRay(SfCircle c1, Sfloat2 c1velocity, SfRay r1, out SfSweepTestHit hit)
        {
            var c1RadiusSqr = c1.radius * c1.radius;

            var c1VelocityLine = new SfLine(c1.origin, c1.origin + c1velocity);
            var c1l1Nearest = NearestPointOnRay(c1.origin, r1);

            // Already interesecting
            if (Sfloat2.LengthSqr(c1l1Nearest - c1.origin) <= c1RadiusSqr)
            {
                hit = new SfSweepTestHit(Sfloat.Zero, c1l1Nearest, c1.origin);
                return true;
            }

            var c1VelocityRay = c1VelocityLine.ToRay(out var c1VelocityLength);

            // With segment
            if (Overlaps(c1VelocityRay, r1, out var _, out var l1c1Distance))
            {
                var c1r1 = c1.origin - c1l1Nearest;
                var c1r1Length = Sfloat2.LengthFast(c1r1);
                var ratio = (Sfloat.One - c1.radius / c1r1Length) * l1c1Distance;
                var hitT = ratio / c1VelocityLength;

                if (hitT.Raw >= SfMath.Zero && hitT.Raw <= SfMath.One)
                {
                    var hitCentroid = c1VelocityRay.GetPoint(ratio);
                    var hitPoint = hitCentroid - (c1r1 / c1r1Length) * c1.radius;

                    hit = new SfSweepTestHit(hitT, hitPoint, hitCentroid);
                    return true;
                }
            }

            hit = default;
            return false;
        }

        public static bool SweepPointVsRect(Sfloat2 point, Sfloat2 velocity, SfRect rect)
        {
            var lineVector = velocity;

            // Special case when velocities are 0
            if (lineVector.RawX == 0 || lineVector.RawY == 0)
            {
                if (rect.Contains(point)) return true;
                if (lineVector.IsZeroLength()) return false;

                var pointDst = point + lineVector;

                if (lineVector.RawX == 0)
                {
                    if (lineVector.RawY > 0 && point.y <= rect.min.y && pointDst.y >= rect.min.y) return true;
                    else if (lineVector.RawY < 0 && point.y >= rect.Max.y && pointDst.y <= rect.Max.y) return true;
                    else return false;
                }

                if (lineVector.RawY == 0)
                {
                    if (lineVector.RawX > 0 && point.x <= rect.min.x && pointDst.x >= rect.min.x) return true;
                    else if (lineVector.RawX < 0 && point.x >= rect.Max.x && pointDst.x <= rect.Max.x) return true;
                    else return false;
                }
            }

            var scale = Sfloat2.RcpFast(lineVector);
            var sign = new Sfloat2(Sfloat.FromInt(Sfloat.Sign(scale.x)), Sfloat.FromInt(Sfloat.Sign(scale.y)));

            var center = rect.Center;
            var halfSize = rect.size.Div2();

            var nearTime = (center - sign * halfSize - point) * scale;
            var farTime = (center + sign * halfSize - point) * scale;

            if (nearTime.x > farTime.y || nearTime.y > farTime.x) return false;

            var nearTimeMax = nearTime.x > nearTime.y ? nearTime.x : nearTime.y;
            var farTimeMax = farTime.x < farTime.y ? farTime.x : farTime.y;

            if (nearTimeMax >= Sfloat.One || farTimeMax <= Sfloat.Zero) return false;

            return true;
        }

        public static bool SweepPointVsRect(Sfloat2 point, Sfloat2 velocity, SfRect rect, out SfSweepTestHit hit)
        {
            var lineVector = velocity;

            // Special case when velocities are 0
            if (lineVector.RawX == 0 || lineVector.RawY == 0)
            {
                if (Overlaps(rect, point, out var delta))
                {
                    var overlapNormal = Sfloat.Abs(delta.x) < Sfloat.Abs(delta.y) ? 
                        (delta.RawX < 0 ? Sfloat2.Left : Sfloat2.Right) :
                        (delta.RawY < 0 ? Sfloat2.Down : Sfloat2.Up);

                    hit = new SfSweepTestHit(Sfloat.Zero, point, point, overlapNormal);
                    return true;
                }

                if (lineVector.IsZeroLength()) // Zero velocity and outside of rect
                {
                    hit = default;
                    return false;
                }

                var pointDst = point + lineVector;

                if (lineVector.RawX == 0 && point.x >= rect.min.x && point.x <= rect.Max.x)
                {
                    if (lineVector.RawY > 0 && point.y <= rect.min.y && pointDst.y >= rect.min.y)
                    {
                        hit = new SfSweepTestHit(Sfloat.LerpInverse(point.y, pointDst.y, rect.min.y), new Sfloat2(point.x, rect.min.y), new Sfloat2(point.x, rect.min.y), Sfloat2.Down);
                        return true;
                    }
                    else if (lineVector.RawY < 0 && point.y >= rect.Max.y && pointDst.y <= rect.Max.y)
                    {
                        hit = new SfSweepTestHit(Sfloat.One - Sfloat.LerpInverse(pointDst.y, point.y, rect.Max.y), new Sfloat2(point.x, rect.Max.y), new Sfloat2(point.x, rect.Max.y), Sfloat2.Up);
                        return true;
                    }
                }

                if (lineVector.RawY == 0 && point.y >= rect.min.y && point.y <= rect.Max.y)
                {
                    if (lineVector.RawX > 0 && point.x <= rect.min.x && pointDst.x >= rect.min.x)
                    {
                        hit = new SfSweepTestHit(Sfloat.LerpInverse(point.x, pointDst.x, rect.min.x), new Sfloat2(rect.min.x, point.y), new Sfloat2(rect.min.x, point.y), Sfloat2.Left);
                        return true;
                    }
                    else if (lineVector.RawX < 0 && point.x >= rect.Max.x && pointDst.x <= rect.Max.x)
                    {
                        hit = new SfSweepTestHit(Sfloat.One - Sfloat.LerpInverse(pointDst.x, point.x, rect.Max.x), new Sfloat2(rect.Max.x, point.y), new Sfloat2(rect.Max.x, point.y), Sfloat2.Right);
                        return true;
                    }
                }

                hit = default;
                return false;
            }

            var scale = Sfloat2.RcpFast(lineVector);
            var sign = new Sfloat2(Sfloat.FromInt(Sfloat.Sign(scale.x)), Sfloat.FromInt(Sfloat.Sign(scale.y)));

            var center = rect.Center;
            var halfSize = rect.size.Div2();

            var nearTime = (center - sign * halfSize - point) * scale;
            var farTime = (center + sign * halfSize - point) * scale;

            if (nearTime.x > farTime.y || nearTime.y > farTime.x)
            {
                hit = default;
                return false;
            }

            var nearTimeMax = nearTime.x > nearTime.y ? nearTime.x : nearTime.y;
            var farTimeMax = farTime.x < farTime.y ? farTime.x : farTime.y;

            if (nearTimeMax >= Sfloat.One || farTimeMax <= Sfloat.Zero)
            {
                hit = default;
                return false;
            }

            var t = Sfloat.Clamp01(nearTimeMax);
            var hitPoint = point + velocity * t;
            var hitNormal = nearTime.x > nearTime.y ? new Sfloat2(-sign.x, Sfloat.Zero) : new Sfloat2(Sfloat.Zero, -sign.y);

            hit = new SfSweepTestHit(t, hitPoint, hitPoint, hitNormal);

            return true;
        }

        public static bool SweepRectVsRect(SfRect r1, Sfloat2 r1Velocity, SfRect r2)
        {
            var halfSize = r1.size >> 1;
            return SweepPointVsRect(r1.min + halfSize, r1Velocity, SfRect.Expand(r2, halfSize));
        }

        public static bool SweepRectVsRect(SfRect r1, Sfloat2 r1Velocity, SfRect r2, out SfSweepTestHit hit)
        {
            var halfSize = r1.size >> 1;

            if (SweepPointVsRect(r1.min + halfSize, r1Velocity, SfRect.Expand(r2, halfSize), out hit))
            {
                if (hit.t.Raw == 0) return true;

                var delta = r1Velocity * hit.t;

                if (hit.normal.y.Raw < 0)
                {
                    var midPoint = (Sfloat.Max(r2.min.x, r1.min.x + delta.x) + Sfloat.Min(r2.Max.x, r1.Max.x + delta.x)) >> 1;
                    var hitPoint = new Sfloat2(midPoint, r2.min.y);

                    hit = new SfSweepTestHit(hit.t, hitPoint, hit.centroid, hit.normal);
                }
                else if (hit.normal.y.Raw > 0)
                {
                    var midPoint = (Sfloat.Max(r2.min.x, r1.min.x + delta.x) + Sfloat.Min(r2.Max.x, r1.Max.x + delta.x)) >> 1;
                    var hitPoint = new Sfloat2(midPoint, r2.Max.y);

                    hit = new SfSweepTestHit(hit.t, hitPoint, hit.centroid, hit.normal);
                }
                else if (hit.normal.x.Raw < 0)
                {
                    var midPoint = (Sfloat.Max(r2.min.y, r1.min.y + delta.y) + Sfloat.Min(r2.Max.y, r1.Max.y + delta.y)) >> 1;
                    var hitPoint = new Sfloat2(r2.min.x, midPoint);

                    hit = new SfSweepTestHit(hit.t, hitPoint, hit.centroid, hit.normal);
                }
                else
                {
                    var midPoint = (Sfloat.Max(r2.min.y, r1.min.y + delta.y) + Sfloat.Min(r2.Max.y, r1.Max.y + delta.y)) >> 1;
                    var hitPoint = new Sfloat2(r2.Max.x, midPoint);

                    hit = new SfSweepTestHit(hit.t, hitPoint, hit.centroid, hit.normal);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool SweepPointVsCapsule(Sfloat2 point, Sfloat2 velocity, SfCapsule capsule, out SfSweepTestHit hit)
        {
            if (velocity.IsZeroLength())
            {
                if (capsule.Contains(point))
                {
                    hit = new SfSweepTestHit(default, point, point, capsule.Line.Normal); //todo normal flip
                    return true;
                }
                else
                {
                    hit = default;
                    return false;
                }
            }

            var pointLine = new SfLine(point, point + velocity);
            var capsuleLine = capsule.Line;

            var nearestLine = ShortestLineBetweenTwoLines(pointLine, capsuleLine);
            var nearestLineLength = nearestLine.Length;

            if (nearestLineLength > capsule.radius)
            {
                hit = default;
                return false;
            }
            else
            {
                var closestLine = ShortestLineBetweenPointAndLine(point, capsuleLine);
                var closestLineLength = closestLine.Length;

                var hitT = default(Sfloat);

                Sfloat2 hitPoint, hitNormal;

                if (closestLineLength.IsZero)
                {
                    hitNormal = capsuleLine.Normal;
                    hitPoint = capsuleLine.p1;
                }
                else if (closestLineLength < capsule.radius)
                {
                    hitNormal = -closestLine.Vector.Normalized;
                    hitPoint = point;
                }
                else
                {
                    var capsuleRay = capsuleLine.ToRay();
                    var capsuleLineLength = capsuleLine.Length;

                    var projectedPoint = NearestPointOnRay(point, capsuleRay, out var projectedT);
                    var projectedLine = new SfLine(point, projectedPoint);
                    var projectedLineLength = projectedLine.Length;

                    hitNormal = -closestLine.Vector.Normalized;

                    if (projectedT < Sfloat.Zero || projectedT > capsuleLineLength)
                    {
                        hitPoint = capsuleLine.p1; // TODO FIX THIS CASE
                        // idea is to convert pointLine to ray, and find closest point on that ray from capsule points (depends on projectedT which line to use)
                        // then use pythagoras with radius and that new length to find the final point of intersection with circle
                    }
                    else
                    {
                        var projectedLine2 = ShortestLineBetweenPointAndRay(pointLine.p2, capsuleRay);
                        var totalProjectedDistnace = (projectedLine.Vector - projectedLine2.Vector).Magnitude;
                        hitT = (projectedLineLength - capsule.radius) / totalProjectedDistnace;
                        hitPoint = pointLine.GetPoint(hitT);
                    }
                }

                hit = new SfSweepTestHit(hitT, hitPoint, hitPoint, hitNormal);
                return true;
            }
        }
    }
}
