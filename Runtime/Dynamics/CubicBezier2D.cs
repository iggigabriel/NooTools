using System;
using UnityEngine;

namespace Noo.Tools
{
    [Serializable]
    public struct CubicBezier2D
    {
        public Vector2 p0;
        public Vector2 p1;
        public Vector2 p2;
        public Vector2 p3;

        public CubicBezier2D(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }

        /// <summary>
        /// Evaluate the cubic Bezier at parameter t (0..1).
        /// </summary>
        public readonly Vector2 Evaluate(float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            return (uuu * p0) +
                   (3 * uu * t * p1) +
                   (3 * u * tt * p2) +
                   (ttt * p3);
        }

        /// <summary>
        /// Get the derivative (tangent vector) at parameter t.
        /// </summary>
        public readonly Vector2 Derivative(float t)
        {
            float u = 1 - t;
            return (3 * u * u * (p1 - p0)) +
                   (6 * u * t * (p2 - p1)) +
                   (3 * t * t * (p3 - p2));
        }

        public static CubicBezier2D FromTo(Vector2 from, Vector2 to)
        {
            var vector = to - from;
            return new CubicBezier2D(from, from + vector * 0.3333f, from + vector * 0.6666f, to);
        }
    }
}
