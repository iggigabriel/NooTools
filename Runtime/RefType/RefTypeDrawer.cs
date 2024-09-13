#if UNITY_EDITOR

using System;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Noo.Tools
{
    [CustomPropertyDrawer(typeof(RefType))]
    public class RefTypeDrawer : PropertyDrawer
    {
        Type[] types;
        GUIContent[] typeDisplayNames;
        string[] typeFullNames;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            AssertTypes(property);

            string selectedType = property.managedReferenceFullTypename;

            int selectedIndex = Array.FindIndex(typeFullNames, (x) => !string.IsNullOrEmpty(x) && selectedType.EndsWith(x));
            if (selectedIndex < 0) selectedIndex = 0;

            var popupRect = rect;
            popupRect.height = EditorGUIUtility.singleLineHeight;

            int newSelectedIndex = EditorGUI.Popup(popupRect, label, selectedIndex, typeDisplayNames);

            if (newSelectedIndex != selectedIndex)
            {
                property.managedReferenceValue = newSelectedIndex == 0 ? null : Activator.CreateInstance(types[newSelectedIndex]);
            }

            EditorGUI.PropertyField(rect, property, true);
        }

        private void AssertTypes(SerializedProperty property)
        {
            if (types == null)
            {
                var type = GetTypeFromManagedReferenceFullTypeName(property.managedReferenceFieldTypename);

                types = new Type[] { null }.Concat(TypeCache.GetTypesDerivedFrom(type))
                    .Where(x => x == null || !x.IsAbstract)
                    .ToArray();

                typeFullNames = types.Select(x => x == null ? "" : x.FullName).ToArray();

                typeDisplayNames = types
                    .Select(x => x == null ? "Null" : ObjectNames.NicifyVariableName(x.Name))
                    .Select(x => new GUIContent(x))
                    .ToArray();
            }
        }

        static Type GetTypeFromManagedReferenceFullTypeName(string managedReferenceFullTypename)
        {
            var splitIndex = managedReferenceFullTypename.IndexOf(' ');

            if (splitIndex > 0)
            {
                var assemblyPart = managedReferenceFullTypename.Substring(0, splitIndex);
                var nsClassnamePart = managedReferenceFullTypename.Substring(splitIndex);
                return Type.GetType($"{nsClassnamePart}, {assemblyPart}");
            }

            return null;
        }

        [MenuItem("GameObject/Clear ScriptableObject References with Missing Types")]
        static public void ClearMissingTypesOnScriptableObjects()
        {
            var report = new StringBuilder();

            foreach (var obj in Selection.objects)
            {
                if (obj is not ScriptableObject) continue;

                if (SerializationUtility.ClearAllManagedReferencesWithMissingTypes(obj))
                {
                    report.Append("Cleared missing types from ").Append(obj.name).AppendLine();
                }
                else
                {
                    report.Append("No missing types to clear on ").Append(obj.name).AppendLine();
                }
            }

            Debug.Log(report.ToString());
        }
    }

    
}

#endif