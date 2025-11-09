using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Noo.Tools
{
    public static class UnityGameObjectExtensions
    {
        public static GameObject Clone(this GameObject gameObject, Transform parent, bool worldPositionStays = false, string name = default)
        {
            var clone = Object.Instantiate(gameObject, parent, worldPositionStays);
            if (!string.IsNullOrEmpty(name)) clone.name = name;
            return clone;
        }

        public static T GetOrAddComponent<T>(this GameObject target) where T : Component
        {
            if (!target) throw default;
            return target.TryGetComponent(out T component) ? component : target.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component target) where T : Component
        {
            if (!target) return default;
            return target.gameObject.TryGetComponent(out T component) ? component : target.gameObject.AddComponent<T>();
        }

        public static T GetComponentCached<T>(this Component target, ref T reference) where T : Component
        {
            if (!reference) reference = target.GetComponent<T>();
            return reference;
        }

        public static T GetComponentInParentCached<T>(this Component target, ref T reference, bool includeInactive = false) where T : Component
        {
            if (!reference) reference = target.GetComponentInParent<T>(includeInactive);
            return reference;
        }

#pragma warning disable UNT0014 // Invalid type for call to GetComponent
        public static bool Is<T>(this GameObject gameObject, out T component) => gameObject.TryGetComponent(out component);
#pragma warning restore UNT0014 // Invalid type for call to GetComponent

        public static void DestroySafe(this GameObject target)
        {
            if (Application.isPlaying) Object.Destroy(target);
            else if (target) Object.DestroyImmediate(target, true);
        }

        public static void DestroySafe<T>(this T target) where T : Component
        {
            if (target && target.gameObject) DestroySafe(target.gameObject);
        }
    }
}
