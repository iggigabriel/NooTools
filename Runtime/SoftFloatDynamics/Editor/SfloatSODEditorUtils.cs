#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Noo.Tools.Editor
{
    public static class SfloatSODEditorUtils
    {
        const string GraphPreviewTimeKey = "SfloatSODEditorUtils_GraphPreviewTimeKey";

        internal static float GraphPreviewTime
        {
            get => SessionState.GetFloat(GraphPreviewTimeKey, 1f);
            set => SessionState.SetFloat(GraphPreviewTimeKey, value);
        }

        static readonly Vector3[] graphPoints = new Vector3[400];
        static readonly SODSfloat graphCurve = new();

        static readonly Color graphBackgroundColor = new(0.32f, 0.32f, 0.32f, 1f);
        static readonly Color graphEdgeColor = new(0.12f, 0.12f, 0.12f, 1f);
        static readonly Color graphLineColor = new(0.52f, 0.52f, 0.52f, 1f);
        static readonly Color graphGridColor = new(1f, 1f, 1f, 0.06f);

        public static SfloatSODCurve GetSfloatSODCurveValue(this SerializedProperty property) => new
        (
            Sfloat.FromRaw(property.FindPropertyRelative("frequency.Raw").intValue),
            Sfloat.FromRaw(property.FindPropertyRelative("damping.Raw").intValue),
            Sfloat.FromRaw(property.FindPropertyRelative("response.Raw").intValue)
        );

        public static Sfloat GetSfloatValue(this SerializedProperty property) => Sfloat.FromRaw(property.FindPropertyRelative("Raw").intValue);
        public static void SetSfloatValue(this SerializedProperty property, Sfloat value) => property.FindPropertyRelative("Raw").intValue = value.Raw;

        public static void DrawGraph(Rect rect, SfloatSODCurve curve, Color color, bool isPreview)
        {
            EditorGUI.DrawRect(rect, graphBackgroundColor);

            var previewTime = GraphPreviewTime;

            if (!isPreview)
            {
                if (previewTime < 5f)
                {
                    for (var line = 0.1f; line < previewTime; line += 0.1f)
                    {
                        EditorGUI.DrawRect(new Rect(rect.xMin + rect.width * (line / previewTime), rect.yMin, 1f, rect.height), graphGridColor);
                    }
                }

                if (previewTime > 1f)
                {
                    for (var line = 1f; line < previewTime; line += 1f)
                    {
                        EditorGUI.DrawRect(new Rect(rect.xMin + rect.width * (line / previewTime), rect.yMin, 1f, rect.height), graphGridColor);
                    }
                }
            }

            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMin, rect.width, 1f), graphEdgeColor);
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMax - 2f, rect.width, 2f), graphEdgeColor);

            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMin, 1f, rect.height), graphEdgeColor);
            EditorGUI.DrawRect(new Rect(rect.xMax, rect.yMin, 1f, rect.height), graphEdgeColor);

            var innerRect = new Rect(rect.position + new Vector2(1f, 1f), rect.size - new Vector2(2f, 4f));

            var samplePoints = graphPoints.Length;
            if (isPreview) samplePoints /= 4;

            var minValue = 0f;
            var maxValue = 1f;

            var time = 0f;
            var deltaTime = 1f / (samplePoints - 1);

            graphCurve.Curve = curve;
            graphCurve.Reset(Sfloat.Zero);
            graphCurve.Target = Sfloat.One;

            for (int i = 0; i < samplePoints; i++)
            {
                var value = graphCurve.Value;

                graphPoints[i] = new Vector3(time, value.Float, 0f);

                minValue = Mathf.Min(minValue, value.Float);
                maxValue = Mathf.Max(maxValue, value.Float);

                time += deltaTime;
                graphCurve.Update(Sfloat.FromFloat(deltaTime * previewTime));
            }

            for (int i = 0; i < samplePoints; i++)
            {
                graphPoints[i] = new Vector3(
                    innerRect.xMin + innerRect.width * graphPoints[i].x,
                    innerRect.yMin + innerRect.height * Mathf.InverseLerp(maxValue, minValue, graphPoints[i].y),
                    0f
                );
            }

            var minYPos = Mathf.InverseLerp(maxValue, minValue, 0f);
            var maxYPos = Mathf.InverseLerp(maxValue, minValue, 1f);

            if (!isPreview)
            {
                EditorGUI.DrawRect(new Rect(innerRect.xMin, innerRect.yMin + innerRect.height * minYPos, innerRect.width, 1f), graphLineColor);
                EditorGUI.DrawRect(new Rect(innerRect.xMin, innerRect.yMin + innerRect.height * maxYPos, innerRect.width, 1f), graphLineColor);

                EditorGUI.LabelField(new Rect(innerRect.xMin - 12f, innerRect.yMin + innerRect.height * minYPos - 8f, 12f, 16f), "0");
                EditorGUI.LabelField(new Rect(innerRect.xMin - 12f, innerRect.yMin + innerRect.height * maxYPos - 8f, 12f, 16f), "1");
            }

            using var handle = new Handles.DrawingScope(color);
            Handles.DrawAAPolyLine(2f, samplePoints, graphPoints);
        }

        public class LabelWidthScope : IDisposable
        {
            readonly float previousLabelWidth;

            public LabelWidthScope(float labelWidth)
            {
                previousLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = labelWidth;
            }

            public void Dispose()
            {
                EditorGUIUtility.labelWidth = previousLabelWidth;
            }
        }
    }

}
#endif