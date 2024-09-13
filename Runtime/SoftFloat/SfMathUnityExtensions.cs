using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Noo.Tools
{
    public static class SfMathUnityExtensions 
    {
        public static Vector2 ToVector2(this Sfloat2 value)
        {
            return new Vector2(SfMath.ToFloat(value.RawX), SfMath.ToFloat(value.RawY));
        }

        public static Vector3 ToVector3XY(this Sfloat2 value)
        {
            return new Vector3(SfMath.ToFloat(value.RawX), SfMath.ToFloat(value.RawY), 0f);
        }

        public static Vector3 ToVector3XZ(this Sfloat2 value)
        {
            return new Vector3(SfMath.ToFloat(value.RawX), 0f, SfMath.ToFloat(value.RawY));
        }

        public static Vector3 ToVector3(this Sfloat3 value)
        {
            return new Vector3(SfMath.ToFloat(value.RawX), SfMath.ToFloat(value.RawY), SfMath.ToFloat(value.RawZ));
        }

        public static Vector4 ToVector4(this Sfloat4 value)
        {
            return new Vector4(SfMath.ToFloat(value.RawX), SfMath.ToFloat(value.RawY), SfMath.ToFloat(value.RawZ), SfMath.ToFloat(value.RawW));
        }

        public static float2 ToFloat2(this Sfloat2 value)
        {
            return new float2(SfMath.ToFloat(value.RawX), SfMath.ToFloat(value.RawY));
        }

        public static float3 ToFloat3XY(this Sfloat2 value)
        {
            return new float3(SfMath.ToFloat(value.RawX), SfMath.ToFloat(value.RawY), 0f);
        }

        public static float3 ToFloat3XZ(this Sfloat2 value)
        {
            return new float3(SfMath.ToFloat(value.RawX), 0f, SfMath.ToFloat(value.RawY));
        }

        public static float3 ToFloat3(this Sfloat3 value)
        {
            return new float3(SfMath.ToFloat(value.RawX), SfMath.ToFloat(value.RawY), SfMath.ToFloat(value.RawZ));
        }

        public static float4 ToFloat4(this Sfloat4 value)
        {
            return new float4(SfMath.ToFloat(value.RawX), SfMath.ToFloat(value.RawY), SfMath.ToFloat(value.RawZ), SfMath.ToFloat(value.RawW));
        }

        public static Quaternion ToQuaternionFromEuler(this Sfloat3 value)
        {
            return Quaternion.Euler(SfMath.ToFloat(value.RawX), SfMath.ToFloat(value.RawY), SfMath.ToFloat(value.RawZ));
        }

        public static Sfloat ToSfloat(this float value)
        {
            return new Sfloat(SfMath.FromFloat(value));
        }

        public static Sfloat2 ToSfloat2(this Vector2 value)
        {
            return new Sfloat2(SfMath.FromFloat(value.x), SfMath.FromFloat(value.y));
        }

        public static Sfloat3 ToSfloat3(this Vector3 value)
        {
            return new Sfloat3(SfMath.FromFloat(value.x), SfMath.FromFloat(value.y), SfMath.FromFloat(value.z));
        }

        public static Sfloat2 ToSfloat2XY(this Vector3 value)
        {
            return new Sfloat2(SfMath.FromFloat(value.x), SfMath.FromFloat(value.y));
        }

        public static Sfloat2 ToSfloat2XZ(this Vector3 value)
        {
            return new Sfloat2(SfMath.FromFloat(value.x), SfMath.FromFloat(value.z));
        }

        public static Sfloat4 ToSfloat4(this Vector4 value)
        {
            return new Sfloat4(SfMath.FromFloat(value.x), SfMath.FromFloat(value.y), SfMath.FromFloat(value.z), SfMath.FromFloat(value.w));
        }

        public static Sfloat2 ToSfloat2(this float2 value)
        {
            return new Sfloat2(SfMath.FromFloat(value.x), SfMath.FromFloat(value.y));
        }

        public static Sfloat3 ToSfloat3(this float3 value)
        {
            return new Sfloat3(SfMath.FromFloat(value.x), SfMath.FromFloat(value.y), SfMath.FromFloat(value.z));
        }

        public static Sfloat2 ToSfloat2XY(this float3 value)
        {
            return new Sfloat2(SfMath.FromFloat(value.x), SfMath.FromFloat(value.y));
        }

        public static Sfloat2 ToSfloat2XZ(this float3 value)
        {
            return new Sfloat2(SfMath.FromFloat(value.x), SfMath.FromFloat(value.z));
        }

        public static Sfloat4 ToSfloat4(this float4 value)
        {
            return new Sfloat4(SfMath.FromFloat(value.x), SfMath.FromFloat(value.y), SfMath.FromFloat(value.z), SfMath.FromFloat(value.w));
        }

        public static Sfloat3 ToSfloat3EulerAngles(this Quaternion value)
        {
            var euler = value.eulerAngles;
            return new Sfloat3(SfMath.FromFloat(euler.x), SfMath.FromFloat(euler.y), SfMath.FromFloat(euler.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int RoundToInt(this Sfloat2 point)
        {
            return new Vector2Int(SfMath.RoundToInt(point.RawX), SfMath.RoundToInt(point.RawY));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int CielToInt(this Sfloat2 point)
        {
            return new Vector2Int(SfMath.CeilToInt(point.RawX), SfMath.CeilToInt(point.RawY));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int FloorToInt(this Sfloat2 point)
        {
            return new Vector2Int(SfMath.FloorToInt(point.RawX), SfMath.FloorToInt(point.RawY));
        }
    }
}
