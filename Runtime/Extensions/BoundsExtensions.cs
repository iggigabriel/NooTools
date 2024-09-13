using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Noo.Tools
{
    public static class BoundsExtensions
    {
        public static Bounds ToBounds(this BoundsInt bounds) => new Bounds(bounds.center, bounds.size);

        /// <summary>
        /// Make sure line points will be inside the bounds
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>Returns true if line intersects the bounds</returns>
        public static bool ClampLine(this Bounds bounds, ref Vector3 start, ref Vector3 end)
        {
            var startRay = new Ray(start, end - start);
            var endRay = new Ray(end, -startRay.direction);

            if (!bounds.IntersectRay(startRay, out float startDistance)) return false;
            if (!bounds.IntersectRay(endRay, out float endDistance)) return false;

            if (startDistance > 0f) start = startRay.GetPoint(startDistance);
            if (endDistance > 0f) end = endRay.GetPoint(endDistance);

            return true;
        }

        /// <summary>
        /// Find the line intersection of a ray inside the bounds
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>Returns true if line intersects the bounds</returns>
        public static bool ClampRay(this Bounds bounds, Ray ray, out Vector3 start, out Vector3 end)
        {
            start = end = ray.origin;

            if (!bounds.IntersectRay(ray, out float startDistance)) return false;
            if (!bounds.IntersectRay(ray, out float endDistance)) return false;

            start = ray.GetPoint(startDistance);
            end = ray.GetPoint(endDistance);

            return true;
        }

        public static BoundsInt Encapsulate(this BoundsInt bounds, Vector3Int position)
        {
            if (bounds.Contains(position))
                return bounds;

            if (position.x < bounds.position.x)
            {
                var increase = bounds.x - position.x;
                bounds.position = new Vector3Int(position.x, bounds.y, bounds.z);
                bounds.size = new Vector3Int(bounds.size.x + increase, bounds.size.y, bounds.size.z);
            }
            if (position.x >= bounds.xMax)
            {
                var increase = position.x - bounds.xMax + 1;
                bounds.size = new Vector3Int(bounds.size.x + increase, bounds.size.y, bounds.size.z);
            }
            if (position.y < bounds.position.y)
            {
                var increase = bounds.y - position.y;
                bounds.position = new Vector3Int(bounds.x, position.y, bounds.z);
                bounds.size = new Vector3Int(bounds.size.x, bounds.size.y + increase, bounds.size.z);
            }
            if (position.y >= bounds.yMax)
            {
                var increase = position.y - bounds.yMax + 1;
                bounds.size = new Vector3Int(bounds.size.x, bounds.size.y + increase, bounds.size.z);
            }
            if (position.z < bounds.position.z)
            {
                var increase = bounds.z - position.z;
                bounds.position = new Vector3Int(bounds.x, bounds.y, position.z);
                bounds.size = new Vector3Int(bounds.size.x, bounds.size.y, bounds.size.z + increase);
            }
            if (position.z >= bounds.zMax)
            {
                var increase = position.z - bounds.zMax + 1;
                bounds.size = new Vector3Int(bounds.size.x, bounds.size.y, bounds.size.z + increase);
            }

            return bounds;
        }
    }
}
