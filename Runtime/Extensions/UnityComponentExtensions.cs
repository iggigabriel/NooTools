using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Noo.Tools
{
    public static class UnityComponentExtensions
    {
        public static T CloneGameObject<T>(this T component, Transform parent, bool worldPositionStays = false, string name = default) where T : Component
        {
            var clone = Object.Instantiate(component.gameObject, parent, worldPositionStays);
            if (!string.IsNullOrEmpty(name)) clone.name = name;
            return clone.GetComponent<T>();
        }
    }
}
