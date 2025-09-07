#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Noo.Tools
{
    [CustomPropertyDrawer(typeof(Sfloat))]
    public class SfPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            var rawProperty = property.FindPropertyRelative("Raw");

            var rawValue = ToRoundedValue(rawProperty.intValue);

            EditorGUI.BeginChangeCheck();

            rawValue = EditorGUI.FloatField(position, label, rawValue);

            if (EditorGUI.EndChangeCheck()) rawProperty.intValue = SfMath.FromFloat(rawValue);

            EditorGUI.EndProperty();
        }

        public static float ToRoundedValue(int rawValue) => Mathf.Round(SfMath.ToFloat(rawValue) * 10000f) / 10000f;

        public static Sfloat SfloatField(Rect position, GUIContent label, Sfloat value)
        {
            var rawValue = ToRoundedValue(value.Raw);
            rawValue = EditorGUI.FloatField(position, label, rawValue);
            return Sfloat.FromFloat(rawValue);
        }

        public static Sfloat SfloatField(Rect position, Sfloat value)
        {
            var rawValue = ToRoundedValue(value.Raw);
            rawValue = EditorGUI.FloatField(position, rawValue);
            return Sfloat.FromFloat(rawValue);
        }
    }

    [CustomPropertyDrawer(typeof(Sdouble))]
    public class SdPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            var rawProperty = property.FindPropertyRelative("Raw");

            var rawValue = ToRoundedValue(rawProperty.longValue);

            EditorGUI.BeginChangeCheck();

            rawValue = EditorGUI.DoubleField(position, label, rawValue);

            if (EditorGUI.EndChangeCheck()) rawProperty.longValue = SdMath.FromDouble(rawValue);

            EditorGUI.EndProperty();
        }

        public static double ToRoundedValue(long rawValue) => System.Math.Round(SdMath.ToDouble(rawValue) * 100000.0) / 100000.0;
    }

    internal abstract class VecPropertyDrawer : PropertyDrawer
    {
        protected abstract GUIContent[] Labels { get; }
        protected abstract float[] Values { get; }

        protected abstract float FromRaw(SerializedProperty property);
        protected abstract void ToRaw(SerializedProperty property, float value);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.wideMode ? 18f : 40f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            for (int i = 0; i < Labels.Length; i++)
            {
                Values[i] = FromRaw(property.FindPropertyRelative($"Raw{Labels[i]}"));
            }

            EditorGUI.BeginChangeCheck();

            EditorGUI.MultiFloatField(position, label, Labels, Values);

            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < Labels.Length; i++)
                {
                    ToRaw(property.FindPropertyRelative($"Raw{Labels[i]}"), Values[i]);
                }
            }

            EditorGUI.EndProperty();
        }
    }

    internal abstract class SfVecPropertyDrawer : VecPropertyDrawer
    {
        protected override float FromRaw(SerializedProperty property) => SfPropertyDrawer.ToRoundedValue(property.intValue);
        protected override void ToRaw(SerializedProperty property, float value) => property.intValue = SfMath.FromFloat(value);
    }

    internal abstract class SdVecPropertyDrawer : VecPropertyDrawer
    {
        protected override float FromRaw(SerializedProperty property) => (float)SdPropertyDrawer.ToRoundedValue(property.longValue);
        protected override void ToRaw(SerializedProperty property, float value) => property.longValue = SdMath.FromFloat(value);
    }

    [CustomPropertyDrawer(typeof(Sfloat2))]
    internal class Sf2PropertyDrawer : SfVecPropertyDrawer
    {
        static readonly GUIContent[] labels = new GUIContent[] {
            new("X"),
            new("Y"),
        };

        static readonly float[] values = new float[2];

        protected override GUIContent[] Labels => labels;
        protected override float[] Values => values;
    }

    [CustomPropertyDrawer(typeof(Sfloat3))]
    internal class Sf3PropertyDrawer : SfVecPropertyDrawer
    {
        static readonly GUIContent[] labels = new GUIContent[] {
            new("X"),
            new("Y"),
            new("Z"),
        };

        static readonly float[] values = new float[3];

        protected override GUIContent[] Labels => labels;
        protected override float[] Values => values;
    }

    [CustomPropertyDrawer(typeof(Sfloat4))]
    internal class Sf4PropertyDrawer : SfVecPropertyDrawer
    {
        static readonly GUIContent[] labels = new GUIContent[] {
            new("X"),
            new("Y"),
            new("Z"),
            new("W"),
        };

        static readonly float[] values = new float[4];

        protected override GUIContent[] Labels => labels;
        protected override float[] Values => values;
    }


    [CustomPropertyDrawer(typeof(Sdouble2))]
    internal class Sd2PropertyDrawer : SdVecPropertyDrawer
    {
        static readonly GUIContent[] labels = new GUIContent[] {
            new("X"),
            new("Y"),
        };

        static readonly float[] values = new float[2];

        protected override GUIContent[] Labels => labels;
        protected override float[] Values => values;
    }

    [CustomPropertyDrawer(typeof(Sdouble3))]
    internal class Sd3PropertyDrawer : SdVecPropertyDrawer
    {
        static readonly GUIContent[] labels = new GUIContent[] {
            new("X"),
            new("Y"),
            new("Z"),
        };

        static readonly float[] values = new float[3];

        protected override GUIContent[] Labels => labels;
        protected override float[] Values => values;
    }

    [CustomPropertyDrawer(typeof(Sdouble4))]
    internal class Sd4PropertyDrawer : SdVecPropertyDrawer
    {
        static readonly GUIContent[] labels = new GUIContent[] {
            new("X"),
            new("Y"),
            new("Z"),
            new("W"),
        };

        static readonly float[] values = new float[4];

        protected override GUIContent[] Labels => labels;
        protected override float[] Values => values;
    }
}

#endif