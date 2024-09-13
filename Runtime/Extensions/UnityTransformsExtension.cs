namespace Noo.Tools
{
    using Unity.Mathematics;
    using UnityEngine;

    public static class UnityTransformExtensions
    {
        public static void DestroyChildrenOfType<T>(this GameObject t, bool includeInactive = true, bool destroyImmediate = false, bool allowDestroyingAssets = false) where T : Component
        {
            var children = t.GetComponentsInChildren<T>(includeInactive);

            for (int q = 0; q < children.Length; q++)
            {
                if (!children[q] || children[q].gameObject == t) continue;

                if (destroyImmediate)
                {
                    Object.DestroyImmediate(children[q].gameObject, allowDestroyingAssets);
                }
                else
                {
                    Object.Destroy(children[q].gameObject);
                }
            }
        }

        public static void DestroyChildrenOfType<T>(this Component t, bool includeInactive = true, bool destroyImmediate = false, bool allowDestroyingAssets = false) where T : Component
        {
            DestroyChildrenOfType<T>(t.gameObject, includeInactive, destroyImmediate, allowDestroyingAssets);
        }

        public static Rect TransformRect(this Transform transform, Rect rect)
        {
            if (!transform) return rect;

            var matrix = transform.localToWorldMatrix;

            float x = rect.x;
            float y = rect.y;
            float xMax = rect.xMax;
            float yMax = rect.yMax;
            var a = matrix.MultiplyPoint3x4(new Vector3(x, y, 0f));
            var b = matrix.MultiplyPoint3x4(new Vector3(x, yMax, 0f));
            var c = matrix.MultiplyPoint3x4(new Vector3(xMax, yMax, 0f));
            var d = matrix.MultiplyPoint3x4(new Vector3(xMax, y, 0f));
            var min = math.min(a, math.min(b, math.min(c, d))).xy;
            var max = math.max(a, math.max(b, math.max(c, d))).xy;
            return new Rect(min, max - min);
        }
    }
}