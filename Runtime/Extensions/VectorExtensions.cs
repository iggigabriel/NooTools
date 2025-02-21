using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Noo.Tools
{
    public static class Vector2Extensions
    {
        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Vector2 Rotate90(this Vector2 vector) => new(vector.y, -vector.x);

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Vector2 Rotate180(this Vector2 vector) => new(-vector.x, -vector.y);

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Vector2 Rotate270(this Vector2 vector) => new(-vector.y, vector.x);

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Vector2 Rotate90X(this Vector2 vector, int rotations)
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

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Vector2 Abs(this Vector2 vector) => new(Mathf.Abs(vector.x), Mathf.Abs(vector.y));

        public static Vector2 RotateAround(this Vector2 point, Vector2 pivot, float degrees)
        {
            var sin = math.sin(degrees * math.TORADIANS);
            var cos = math.cos(degrees * math.TORADIANS);

            //math.sincos(degrees * Mathf.Deg2Rad, out var sin, out var cos);
            point -= pivot;
            return new Vector2(point.x * cos - point.y * sin + pivot.x, point.x * sin + point.y * cos + pivot.y);
        }

    }

    public static class Vector2IntExtensions
    {
        public static float RoundToNearest(this float value, float spacing)
        {
            return Mathf.RoundToInt(value / spacing) * spacing;
        }

        /// <summary>
        /// Returns true if are touching
        /// </summary>
        public static bool IsNextTo(this Vector2Int target, Vector2Int value)
        {
            var diff = target - value;
            return (diff.x == 0 && math.abs(diff.y) == 1) || (diff.y == 0 && math.abs(diff.x) == 1);
        }

        public static Vector3 ToVector3XY(this Vector2 value)
        {
            return (Vector3)value;
        }

        public static Vector3 ToVector3XY(this Vector2 value, float z)
        {
            return new Vector3(value.x, value.y, z);
        }

        public static Vector3 ToVector3XZ(this Vector2 value)
        {
            return new Vector3(value.x, 0f, value.y);
        }

        public static Vector3 ToVector3XZ(this Vector2 value, float height)
        {
            return new Vector3(value.x, height, value.y);
        }

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Vector2Int Rotate90(this Vector2Int vector) => new(vector.y, -vector.x);

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Vector2Int Rotate180(this Vector2Int vector) => new(-vector.x, -vector.y);

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Vector2Int Rotate270(this Vector2Int vector) => new(-vector.y, vector.x);

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Vector2Int Rotate90X(this Vector2Int vector, int rotations)
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

        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Vector2Int Abs(this Vector2Int vector) => new(Mathf.Abs(vector.x), Mathf.Abs(vector.y));


        [MethodImpl(SfUtil.AggressiveInlining)]
        public static Vector2Int AsYX(this Vector2Int vector) => new(vector.y, vector.x);

        public static Sfloat2 ToSfloat2(this Vector2Int value)
        {
            return Sfloat2.FromInt(value.x, value.y);
        }
    }

    public static class Vector3Extensions
    {
        public static Vector2 ToVector2XY(this Vector3 value)
        {
            return (Vector2)value;
        }

        public static Vector2 ToVector2XZ(this Vector3 value)
        {
            return new Vector2(value.x, value.z);
        }

        public static Vector3 WithX(this Vector3 value, float x) => new(x, value.y, value.z);
        public static Vector3 WithY(this Vector3 value, float y) => new(value.x, y, value.z);
        public static Vector3 WithZ(this Vector3 value, float z) => new(value.x, value.y, z);

        public static float2 ToVector2XY(this float3 value)
        {
            return value.xy;
        }

        public static float2 ToVector2XZ(this float3 value)
        {
            return new float2(value.x, value.z);
        }

        public static float3 WithX(this float3 value, float x) => new(x, value.y, value.z);
        public static float3 WithY(this float3 value, float y) => new(value.x, y, value.z);
        public static float3 WithZ(this float3 value, float z) => new(value.x, value.y, z);
    }
}
