using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

[assembly: InternalsVisibleTo("Noo.DevToolkit", AllInternalsVisible = true)]
namespace Noo.Nui
{
    internal static class NuiUtility
    {
        public static string ClassCombine(string classBase, string classWith) => $"{classBase}-{classWith}";
        public static string ClassCombine(string classBase, string classModifier, string classWith) => $"{classBase}--{classModifier}-{classWith}";

        public static T WithClass<T>(this T element, string className) where T : VisualElement
        {
            element.AddToClassList(className);
            return element;
        }

        public static T WithoutClass<T>(this T element, string className) where T : VisualElement
        {
            element.RemoveFromClassList(className);
            return element;
        }

        public static T WithClass<T>(this T element, string className1, string className2) where T : VisualElement
        {
            element.AddToClassList(className1);
            element.AddToClassList(className2);
            return element;
        }

        public static T WithoutClass<T>(this T element, string className1, string className2) where T : VisualElement
        {
            element.RemoveFromClassList(className1);
            element.RemoveFromClassList(className2);
            return element;
        }

        public static T WithClass<T>(this T element, string className1, string className2, string className3) where T : VisualElement
        {
            element.AddToClassList(className1);
            element.AddToClassList(className2);
            element.AddToClassList(className3);
            return element;
        }

        public static T WithClass<T>(this T element, string className1, string className2, string className3, params string[] extraClasses) where T : VisualElement
        {
            element.AddToClassList(className1);
            element.AddToClassList(className2);
            element.AddToClassList(className3);
            foreach (var c in extraClasses) element.AddToClassList(c);
            return element;
        }

        public static T WithName<T>(this T element, string name) where T : VisualElement
        {
            element.name = name;
            return element;
        }

        public static T AppendTo<T>(this T element, VisualElement parent) where T : VisualElement
        {
            parent.Add(element);
            return element;
        }

        public static T AppendToHierarchy<T>(this T element, VisualElement parent) where T : VisualElement
        {
            parent.hierarchy.Add(element);
            return element;
        }

        public static T PrependTo<T>(this T element, VisualElement parent) where T : VisualElement
        {
            parent.Add(element);
            element.SendToBack();
            return element;
        }

        public static T PrependToToHierarchy<T>(this T element, VisualElement parent) where T : VisualElement
        {
            parent.hierarchy.Add(element);
            element.SendToBack();
            return element;
        }

        public static T Closest<T>(this VisualElement element) where T : VisualElement
        {
            for (var parent = element.hierarchy.parent; parent != null; parent = parent.hierarchy.parent)
            {
                if (parent is T result) return result;
            }

            return null;
        }

        public static VisualElement Closest(this VisualElement element, string className)
        {
            for (var parent = element.hierarchy.parent; parent != null; parent = parent.hierarchy.parent)
            {
                if (parent.ClassListContains(className)) return parent;
            }

            return null;
        }

        public static bool IsChildOf(this VisualElement element, VisualElement parent)
        {
            return element.parent == parent;
        }

        public static bool IsParentOf(this VisualElement element, VisualElement child)
        {
            return child.parent == element;
        }

        public static bool IsDescendantOf(this VisualElement element, VisualElement ancestor)
        {
            if (ancestor == null) return false;

            for (var parent = element.hierarchy.parent; parent != null; parent = parent.hierarchy.parent)
            {
                if (parent == ancestor) return true;
            }

            return false;
        }

        public static bool IsAncestorOf(this VisualElement element, VisualElement descendant)
        {
            if (descendant == null) return false;

            for (var parent = descendant.hierarchy.parent; parent != null; parent = parent.hierarchy.parent)
            {
                if (parent == element) return true;
            }

            return false;
        }

        public static VisualElement FirstChild(this VisualElement element)
        {
            if (element.childCount == 0) return null;
            return element[0];
        }

        public static T FirstChild<T>(this VisualElement element) where T : VisualElement
        {
            if (element.childCount == 0) return null;
            var child = element[0];
            return child is T childAsT ? childAsT : null;
        }

        public static void LogException(Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }

        public static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError(message);
        }

        public static TempList<VisualElement> ChildrenAsTempList(this VisualElement element)
        {
            return element.Children().ToTempList();
        }

        public static bool TryGetNuiSystem<T>(this IPanel panel, out T system) where T : NuiSystem
        {
            return NuiSystem.TryFind(panel, out system);
        }

        public static string[] ParseSearchQuery(string query)
        {
            // TODO OPTIMIZE THIS NO GC
            return query.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim().ToLowerInvariant()).ToArray();
        }

        public static bool AreEqual<T>(T item1, T item2)
        {
            if (item1 == null && item2 == null) return true;
            if (item1 is IEquatable<T> eItem1) return eItem1.Equals(item2);
            if (item1 is Enum enum1 && item2 is Enum enum2) return enum1 == enum2;
            return item1.Equals(item2);
        }

        public static bool AreEqual<T>(IReadOnlyList<T> item1, IReadOnlyList<T> item2)
        {
            if (item1 == null && item2 == null) return true;
            if (item1 == null && item2.Count == 0) return true;
            if (item2 == null && item1.Count == 0) return true;
            if (item1.Count != item2.Count) return false;

            for (int i = 0; i < item1.Count; i++)
            {
                if (!AreEqual(item1[i], item2[i])) return false;
            }

            return true;
        }

        public static ChildrenEnumerator GetDescendants(this VisualElement element)
        {
            return new ChildrenEnumerator(element);
        }

        public static VisualElement GetFirstFocusableChild(this VisualElement element)
        {
            foreach (var child in element.GetDescendants())
            {
                if (child.focusable) return child;
            }

            return null;
        }

        public static VisualElement GetFirstDescendantsWithClass(this VisualElement element, string className)
        {
            foreach (var child in element.GetDescendants())
            {
                if (child.ClassListContains(className)) return child;
            }

            return null;
        }
    }
}
