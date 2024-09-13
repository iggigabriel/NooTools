#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Noo.Tools
{
    public static class NooHandles
    {
        public readonly struct ColorScope : IDisposable
        {
            public readonly Color previousColor;
            public readonly Color? color;

            public ColorScope(Color? color)
            {
                previousColor = Handles.color;
                this.color = color;
                if (color.HasValue) Handles.color = color.Value;
            }

            public void Dispose()
            {
                if (color.HasValue) Handles.color = previousColor;
            }
        }

        public static bool EditPoint2D(ref Vector2 point, float snap = 0f, Transform relativeToTransform = null, Color? color = null)
        {
            using var _ = new ColorScope(color);

            var pos = relativeToTransform ? relativeToTransform.TransformPoint(point) : point.ToVector3XY();
            var size = HandleUtility.GetHandleSize(pos) / 20f;
            var newPos = Handles.FreeMoveHandle(pos, size, snap * Vector3.one, Handles.DotHandleCap);

            point = relativeToTransform ? relativeToTransform.InverseTransformPoint(newPos) : newPos;

            return pos != newPos;
        }
    }
}
#endif
