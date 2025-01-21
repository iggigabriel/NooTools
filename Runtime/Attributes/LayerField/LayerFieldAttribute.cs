using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Noo.Tools
{
    public class LayerFieldAttribute : PropertyAttribute
    {
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LayerFieldAttribute))]
    class LayerAttributeEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.LayerField(position, label, property.intValue);
        }
    }
#endif
}
