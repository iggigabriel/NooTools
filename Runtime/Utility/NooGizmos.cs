using System;
using UnityEngine;

namespace Noo.Tools
{
    public static class NooGizmos
    {
        public readonly struct ColorScope : IDisposable
        {
            public readonly Color previousColor;
            public readonly Color? color;

            public ColorScope(Color? color)
            {
                previousColor = Gizmos.color;
                this.color = color;
                if (color.HasValue) Gizmos.color = color.Value;
            }

            public void Dispose()
            {
                if (color.HasValue) Gizmos.color = previousColor;
            }
        }

        static readonly Vector3[] gizmoHelper = new Vector3[1024];

        public static void DrawLine(Vector3 p1, Vector3 p2, Transform relativeToTransform = null, Color? color = null)
        {
            using var _ = new ColorScope(color);

            if (relativeToTransform)
            {
                p1 = relativeToTransform.TransformPoint(p1);
                p2 = relativeToTransform.TransformPoint(p2);
            }

            gizmoHelper[0] = p1;
            gizmoHelper[1] = p2;

            Gizmos.DrawLineList(new ReadOnlySpan<Vector3>(gizmoHelper, 0, 2));
        }

        public static void DrawPolygon(Vector3 p1, Vector3 p2, Vector3 p3, Transform relativeToTransform = null, Color? color = null)
        {
            using var _ = new ColorScope(color);

            if (relativeToTransform)
            {
                p1 = relativeToTransform.TransformPoint(p1);
                p2 = relativeToTransform.TransformPoint(p2);
                p3 = relativeToTransform.TransformPoint(p3);
            }

            gizmoHelper[0] = p1;
            gizmoHelper[1] = p2;
            gizmoHelper[2] = p2;
            gizmoHelper[3] = p3;
            gizmoHelper[4] = p3;
            gizmoHelper[5] = p1;

            Gizmos.DrawLineList(new ReadOnlySpan<Vector3>(gizmoHelper, 0, 6));
        }

        public static void DrawPolygon(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Transform relativeToTransform = null, Color? color = null)
        {
            using var _ = new ColorScope(color);

            if (relativeToTransform)
            {
                p1 = relativeToTransform.TransformPoint(p1);
                p2 = relativeToTransform.TransformPoint(p2);
                p3 = relativeToTransform.TransformPoint(p3);
                p4 = relativeToTransform.TransformPoint(p4);
            }

            gizmoHelper[0] = p1;
            gizmoHelper[1] = p2;
            gizmoHelper[2] = p2;
            gizmoHelper[3] = p3;
            gizmoHelper[4] = p3;
            gizmoHelper[5] = p4;
            gizmoHelper[6] = p4;
            gizmoHelper[7] = p1;

            Gizmos.DrawLineList(new ReadOnlySpan<Vector3>(gizmoHelper, 0, 8));
        }

        public static void DrawRect(Rect rect, Transform relativeToTransform = null, Color? color = null)
        {
            DrawPolygon(
                new Vector3(rect.xMin, rect.yMin, 0f),
                new Vector3(rect.xMin, rect.yMax, 0f),
                new Vector3(rect.xMax, rect.yMax, 0f),
                new Vector3(rect.xMax, rect.yMin, 0f),
                relativeToTransform,
                color
            );
        }

        public static void DrawBone2D(Vector2 from, Vector2 to, Transform relativeToTransform = null, Color? color = null)
        {
            var delta = to - from;
            var deltaLength = delta.magnitude;
            var deltaNormalized = delta / deltaLength;

            if (deltaLength < 0.0001f) return;

            var p1 = from + deltaNormalized * (deltaLength * 0.1f) + deltaNormalized.Rotate90() * (deltaLength * 0.1f);
            var p2 = from + deltaNormalized * (deltaLength * 0.1f) - deltaNormalized.Rotate90() * (deltaLength * 0.1f);

            DrawLine(from, to, relativeToTransform, color);
            DrawLine(p1, p2, relativeToTransform, color);
            DrawPolygon(from, p1, to, p2, relativeToTransform, color);
        }
    }
}
