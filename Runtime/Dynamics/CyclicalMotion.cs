using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Noo.Tools
{
    [HideMonoScript]
    public sealed class CyclicalMotion : MonoBehaviour
    {
        [Serializable]
        [InlineProperty]
        public struct Cycle
        {
            [HorizontalGroup(74f, marginRight: 12), LabelText(SdfIconType.Clock, Text = ""), LabelWidth(24f)]
            public float frequency;

            [HorizontalGroup(Width = 64), LabelText("X"), LabelWidth(16f)]
            public float dirX;

            [HorizontalGroup(Width = 64), LabelText("Y"), LabelWidth(16f)]
            public float dirY;

            [HorizontalGroup(Width = 64), LabelText("R"), LabelWidth(16f), Unit(Units.Degree), Tooltip("Rotation")]
            public float rotation;

            [HorizontalGroup(Width = 64), LabelText("S"), LabelWidth(16f), Tooltip("Scale")]
            public float scale;

            public readonly Vector4 GetOffset(float time)
            {
                return Mathf.Sin(time * frequency) * new Vector4(dirX, dirY, rotation, scale);
            }
        }

        public Cycle[] cycles;

        private Vector3 initialPosition;
        private float initialRotation;
        private Vector3 initialScale;

        [HorizontalGroup("fRange")]
        public float fMin = 1f;
        [HorizontalGroup("fRange")]
        public float fMax = 1f;
        private float f;

        [HorizontalGroup("dRange")]
        public float dMin = 1f;
        [HorizontalGroup("dRange")]
        public float dMax = 1f;
        private float d;

        public float tOffset;

        private float time;

        private void OnEnable()
        {
            initialPosition = transform.localPosition;
            initialRotation = transform.localEulerAngles.z;
            initialScale = transform.localScale;

            f = UnityEngine.Random.Range(fMin, fMax);
            d = UnityEngine.Random.Range(dMin, dMax);

            time = 0f;
        }

        private void OnDisable()
        {
            transform.localPosition = initialPosition;
            transform.localEulerAngles = new Vector3(0f, 0f, initialRotation);
            transform.localScale = initialScale;
        }

        private void Update()
        {
            time += Time.deltaTime;

            var t = (time + tOffset) * (2f * Mathf.PI) * f - f;

            var pos = initialPosition;
            var rot = initialRotation;
            var scale = initialScale;

            foreach (var cycle in cycles)
            {
                var offset = cycle.GetOffset(t) * d;

                pos += (Vector3)(Vector2)offset;
                rot += offset.z;
                scale *= (offset.w + 1f);
            }

            transform.SetLocalPositionAndRotation(pos, Quaternion.Euler(new Vector3(0f, 0f, rot)));
            transform.localScale = scale;
        }
    }
}
