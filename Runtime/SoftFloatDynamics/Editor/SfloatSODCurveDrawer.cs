#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Noo.Tools.Editor
{
    [CustomPropertyDrawer(typeof(SfloatSODCurve))]
    public class SfloatSODCurveDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            rect = EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive), label);

            DrawProperty(rect, property);

            EditorGUI.EndProperty();
        }

        public static void DrawProperty(Rect rect, SerializedProperty property)
        {
            var graphRect = new Rect(rect.position + Vector2.one, rect.size - Vector2.one * 2);

            SfloatSODEditorUtils.DrawGraph(rect, property.GetSfloatSODCurveValue(), Color.green, true);

            if (property.hasMultipleDifferentValues)
            {
                EditorGUI.DrawRect(new Rect(rect.center.x - 7f, rect.center.y, 14f, 1f), Color.grey);
            }
            else
            {
                if ((Event.current.button == 0) && (Event.current.type == EventType.MouseDown) && graphRect.Contains(Event.current.mousePosition))
                {
                    SfloatSODCurveEditor.Open(graphRect, property);
                }
            }
        }
    }
}
#endif