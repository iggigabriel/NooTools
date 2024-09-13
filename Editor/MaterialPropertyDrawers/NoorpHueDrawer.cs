using UnityEditor;
using UnityEngine;

namespace Noo.Tools.Editor
{
    public class NoorpHueDrawer : MaterialPropertyDrawer
    {
        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            var value = prop.vectorValue;

            var color = Color.HSVToRGB((SfGeom.VectorToAngle(Sfloat2.FromFloat(value.x, value.y)).Float / 360f + 0.5f) % 1f, 1f, 1f);

            EditorGUI.BeginChangeCheck();

            EditorGUI.showMixedValue = prop.hasMixedValue;

            color = EditorGUI.ColorField(position, new GUIContent(label), color, true, false, false);

            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                Color.RGBToHSV(color, out var hue, out _, out _);

                var colorVector = SfGeom.AngleToVector(Sfloat.FromFloat(hue) * SfGeom.Deg360 - SfGeom.Deg180);
                value.x = colorVector.x.Float;
                value.y = colorVector.y.Float;

                prop.vectorValue = value;
            }
        }
    }
}
