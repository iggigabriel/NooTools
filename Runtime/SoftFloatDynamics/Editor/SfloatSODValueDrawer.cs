#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Noo.Tools.Editor
{
    [CustomPropertyDrawer(typeof(SfloatSODValue<,>), true)]
    [CustomPropertyDrawer(typeof(SODSfloat))]
    [CustomPropertyDrawer(typeof(SODSfloatAngle))]
    [CustomPropertyDrawer(typeof(SODSfloat2))]
    [CustomPropertyDrawer(typeof(SODSfloat3))]
    [CustomPropertyDrawer(typeof(SODSfloat4))]
    public class SfloatSODValueDrawer : PropertyDrawer
    {
        static float LineHeight => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return !property.isExpanded ? EditorGUIUtility.singleLineHeight : LineHeight * 4;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            var labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

            property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, label, true);

            var graphRect = new Rect(rect.x + EditorGUIUtility.labelWidth + 2f, rect.y, rect.width - EditorGUIUtility.labelWidth - 2f, EditorGUIUtility.singleLineHeight);

            var spCurve = property.FindPropertyRelative("curve");
            var spState = property.FindPropertyRelative("state");

            SfloatSODCurveDrawer.DrawProperty(graphRect, spCurve);

            if (property.isExpanded)
            {                
                using (new SfloatSODEditorUtils.LabelWidthScope(EditorGUIUtility.labelWidth - 15f))
                {
                    var spPreviousValue = spState.FindPropertyRelative("previousValue");
                    var spVelocity = spState.FindPropertyRelative("velocity");
                    var spTarget = spState.FindPropertyRelative("target");

                    var propertyRect = new Rect(rect.x + 15f, rect.y, rect.width - 15f, EditorGUIUtility.singleLineHeight);

                    propertyRect.y += LineHeight;
                    EditorGUI.PropertyField(propertyRect, spPreviousValue, new GUIContent("Value"));

                    propertyRect.y += LineHeight;
                    EditorGUI.PropertyField(propertyRect, spTarget);

                    propertyRect.y += LineHeight;
                    EditorGUI.PropertyField(propertyRect, spVelocity);
                }
            }

            spState.FindPropertyRelative("kValues.RawX").intValue = spCurve.FindPropertyRelative("kValues.RawX").intValue;
            spState.FindPropertyRelative("kValues.RawY").intValue = spCurve.FindPropertyRelative("kValues.RawY").intValue;
            spState.FindPropertyRelative("kValues.RawZ").intValue = spCurve.FindPropertyRelative("kValues.RawZ").intValue;

            EditorGUI.EndProperty();
        }
    }
}
#endif