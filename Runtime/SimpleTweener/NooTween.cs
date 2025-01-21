using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Noo.Tools.NooTween
{
    [Serializable]
    public class NooTween
    {
        public enum RepeatBehaviour
        {
            None,
            Repeat,
        }

        [SerializeField]
        public RepeatBehaviour repeat;

        [SerializeReference, ListDrawerSettings(ShowFoldout = false), PolymorphicDrawerSettings(ShowBaseType = false), InlineProperty]
        public List<NooTweenTrack> sequence = new();

        public NooTweenPlayer PlayOn(GameObject gameObject)
        {
            return NooTweenPlayer.GetNew(this, gameObject);
        }
    }

    [Serializable]
    public abstract class NooTweenTrack
    {
        [HorizontalGroup]
        public float delay;

        [HorizontalGroup]
        public float duration;

        [InlineProperty, HideLabel]
        public NooTweenAnimationEasing easing;

        public float TotalDuration => delay + duration;

        public virtual void Init(NooTweenPlayer player) { }
        public virtual void Start(NooTweenPlayer player) { }
        public virtual void Clear(NooTweenPlayer player) { }
        public virtual void Evaluate(NooTweenPlayer player, float normalizedTime) { }

        public float GetNormalizedTime(float time)
        {
            if (duration <= 0f) return 1f;
            if (time <= delay) return 0f;

            var t = Mathf.Clamp01((time - delay) / duration);

            return easing.easingType switch
            {
                NooTweenAnimationEasing.Type.Easing => Easing.Evaluate(easing.easingStyle, t),
                NooTweenAnimationEasing.Type.AnimationCurve => easing.animationCurve.Evaluate(t),
                NooTweenAnimationEasing.Type.SecondOrderDynamics => easing.EvaluateSODCurveTime(t),
                _ => 1f,
            };
        }
    }

    public enum NooTweenTargetType
    {
        Initial,
        Current,
        Fixed
    }

    [Serializable]
    public class NooTweenAnimationEasing
    {
        public enum Type
        {
            Easing,
            AnimationCurve,
            SecondOrderDynamics
        }

        public Type easingType;

        [ShowIf("@this.easingType == Type.Easing"), Indent, LabelText(" ")]
        public Easing.Type easingStyle = Easing.Type.Linear;

        [ShowIf("@this.easingType == Type.AnimationCurve"), Indent, LabelText(" ")]
        public AnimationCurve animationCurve;

        [ShowIf("@this.easingType == Type.SecondOrderDynamics"), Indent, LabelText(" ")]
        public SODCurve sodCurve;

        static readonly SODFloat sodEvaluator = new();

        public float EvaluateSODCurveTime(float time)
        {
            sodEvaluator.Reset(0f);
            sodEvaluator.Curve = sodCurve;
            sodEvaluator.Update(time);
            return sodEvaluator.Value;
        }
    }

    [Serializable]
    public abstract class NooTweenTrack<T> : NooTweenTrack
    {
        protected virtual bool ShowDefaultDrawers => true;

        [PropertyOrder(1)]
        public NooTweenTargetType from = NooTweenTargetType.Fixed;

        [PropertyOrder(2)]
        [ShowIf("@from == NooTweenTargetType.Fixed && ShowDefaultDrawers"), LabelText(" ")]
        public T fixedFromValue;

        [PropertyOrder(3)]
        public NooTweenTargetType to = NooTweenTargetType.Fixed;

        [PropertyOrder(4)]
        [ShowIf("@to == NooTweenTargetType.Fixed && ShowDefaultDrawers"), LabelText(" ")]
        public T fixedToValue;

        public override void Init(NooTweenPlayer player)
        {
            base.Init(player);

            player.SetCustomData(this, "initialData", GetValue(player));
        }

        public override void Clear(NooTweenPlayer player)
        {
            base.Clear(player);

            SetValue(player, player.GetCustomData<T>(this, "initialData"));
        }

        public override void Start(NooTweenPlayer player)
        {
            base.Start(player);

            player.SetCustomData(this, "currentData", GetValue(player));
        }

        T GetFromValue(NooTweenPlayer player)
        {
            return from switch
            {
                NooTweenTargetType.Initial => player.GetCustomData<T>(this, "initialData"),
                NooTweenTargetType.Current => player.GetCustomData<T>(this, "currentData"),
                NooTweenTargetType.Fixed => fixedFromValue,
                _ => default,
            };
        }

        T GetToValue(NooTweenPlayer player)
        {
            return to switch
            {
                NooTweenTargetType.Initial => player.GetCustomData<T>(this, "initialData"),
                NooTweenTargetType.Current => player.GetCustomData<T>(this, "currentData"),
                NooTweenTargetType.Fixed => fixedToValue,
                _ => default,
            };
        }

        public override void Evaluate(NooTweenPlayer player, float normalizedTime)
        {
            base.Evaluate(player, normalizedTime);

            SetValue(player, Lerp(GetFromValue(player), GetToValue(player), normalizedTime));
        }

        protected abstract T GetValue(NooTweenPlayer player);
        protected abstract void SetValue(NooTweenPlayer player, T value);
        protected abstract T Lerp(T from, T to, float t);
    }

    public abstract class NooTweenTrackFloat : NooTweenTrack<float>
    {
        protected override float Lerp(float from, float to, float t) => Mathf.LerpUnclamped(from, to, t);
    }

    public abstract class NooTweenTrackVector2 : NooTweenTrack<Vector2>
    {
        protected override Vector2 Lerp(Vector2 from, Vector2 to, float t) => Vector2.LerpUnclamped(from, to, t);
    }

    public abstract class NooTweenTrackVector3 : NooTweenTrack<Vector3>
    {
        protected override Vector3 Lerp(Vector3 from, Vector3 to, float t) => Vector3.LerpUnclamped(from, to, t);
    }

    public abstract class NooTweenTrackColor : NooTweenTrack<Color>
    {
        protected override bool ShowDefaultDrawers => false;

#pragma warning disable IDE0051 // Remove unused private members
        [PropertyOrder(2)]
        [ShowInInspector, ShowIf("@from == NooTweenTargetType.Fixed"), LabelText(" "), ColorUsage(true, true)]
        Color FromValueDrawer { get => fixedFromValue; set => fixedFromValue = value; }

        [PropertyOrder(4)]
        [ShowInInspector, ShowIf("@to == NooTweenTargetType.Fixed"), LabelText(" "), ColorUsage(true, true)]
        Color ToValueDrawer { get => fixedToValue; set => fixedToValue = value; }
#pragma warning restore IDE0051 // Remove unused private members

        protected override Color Lerp(Color from, Color to, float t) => Color.LerpUnclamped(from, to, t);
    }

    [Serializable]
    public class TranslateLocal : NooTweenTrackVector3
    {
        protected override Vector3 GetValue(NooTweenPlayer player) => player.Target.transform.localPosition;
        protected override void SetValue(NooTweenPlayer player, Vector3 value) => player.Target.transform.localPosition = value;
    }

    [Serializable]
    public class TranslateWorld : NooTweenTrackVector3
    {
        protected override Vector3 GetValue(NooTweenPlayer player) => player.Target.transform.position;
        protected override void SetValue(NooTweenPlayer player, Vector3 value) => player.Target.transform.position = value;
    }

    [Serializable]
    public class RotateLocal : NooTweenTrackVector3
    {
        protected override Vector3 GetValue(NooTweenPlayer player) => player.Target.transform.localEulerAngles;
        protected override void SetValue(NooTweenPlayer player, Vector3 value) => player.Target.transform.localEulerAngles = value;
    }

    [Serializable]
    public class RotateWorld : NooTweenTrackVector3
    {
        protected override Vector3 GetValue(NooTweenPlayer player) => player.Target.transform.eulerAngles;
        protected override void SetValue(NooTweenPlayer player, Vector3 value) => player.Target.transform.eulerAngles = value;
    }

    [Serializable]
    public class ScaleLocal : NooTweenTrackVector3
    {
        protected override Vector3 GetValue(NooTweenPlayer player) => player.Target.transform.localScale;
        protected override void SetValue(NooTweenPlayer player, Vector3 value) => player.Target.transform.localScale = value;
    }

    [Serializable]
    public class SpriteColor : NooTweenTrackColor
    {
        protected override Color GetValue(NooTweenPlayer player)
        {
            return (player.Target.TryGetComponent<SpriteRenderer>(out var renderer)) ? renderer.color : default;
        }

        protected override void SetValue(NooTweenPlayer player, Color value)
        {
            if (player.Target.TryGetComponent<SpriteRenderer>(out var renderer)) renderer.color = value;
        }
    }

    [Serializable]
    public class MaterialPropertyFloat : NooTweenTrackFloat
    {
        [SerializeField]
        string propertyName;

        int PropertyId => RenderingUtils.GetShaderId(propertyName);

        protected override float GetValue(NooTweenPlayer player)
        {
            if (player.Target.TryGetComponent<Renderer>(out var renderer))
            {
                if (renderer.HasPropertyBlock())
                {
                    var props = player.GetCustomData(this, "mpb", RenderingUtils.GetNewMaterialPropertyBlock);
                    renderer.GetPropertyBlock(props);

                    if (props.HasFloat(PropertyId))
                    {
                        return props.GetFloat(PropertyId);
                    }
                }

                if (renderer.sharedMaterial)
                {
                    if (renderer.sharedMaterial.HasFloat(PropertyId))
                    {
                        return renderer.sharedMaterial.GetFloat(PropertyId);
                    }
                }
            }

            return default;
        }

        protected override void SetValue(NooTweenPlayer player, float value)
        {
            if (player.Target.TryGetComponent<Renderer>(out var renderer))
            {
                var props = player.GetCustomData(this, "mpb", RenderingUtils.GetNewMaterialPropertyBlock);

                if (renderer.HasPropertyBlock())
                {
                    renderer.GetPropertyBlock(props);
                }

                props.SetFloat(PropertyId, value);
                renderer.SetPropertyBlock(props);
            }
        }

        public override void Clear(NooTweenPlayer player)
        {
            base.Clear(player);

            var props = player.GetCustomData(this, "mpb", RenderingUtils.GetNewMaterialPropertyBlock);
            RenderingUtils.ReleaseMaterialPropertyBlock(props);
        }
    }

    [Serializable]
    public class MaterialPropertyColor : NooTweenTrackColor
    {
        [SerializeField]
        string propertyName;

        int PropertyId => RenderingUtils.GetShaderId(propertyName);

        protected override Color GetValue(NooTweenPlayer player)
        {
            if (player.Target.TryGetComponent<Renderer>(out var renderer))
            {
                if (renderer.HasPropertyBlock())
                {
                    var props = player.GetCustomData(this, "mpb", RenderingUtils.GetNewMaterialPropertyBlock);
                    renderer.GetPropertyBlock(props);

                    if (props.HasColor(PropertyId))
                    {
                        return props.GetColor(PropertyId);
                    }
                }

                if (renderer.sharedMaterial)
                {
                    if (renderer.sharedMaterial.HasColor(PropertyId))
                    {
                        return renderer.sharedMaterial.GetColor(PropertyId);
                    }
                }
            }

            return default;
        }

        protected override void SetValue(NooTweenPlayer player, Color value)
        {
            if (player.Target.TryGetComponent<Renderer>(out var renderer))
            {
                var props = player.GetCustomData(this, "mpb", RenderingUtils.GetNewMaterialPropertyBlock);

                if (renderer.HasPropertyBlock())
                {
                    renderer.GetPropertyBlock(props);
                }

                props.SetColor(PropertyId, value);
                renderer.SetPropertyBlock(props);
            }
        }

        public override void Clear(NooTweenPlayer player)
        {
            base.Clear(player);

            var props = player.GetCustomData(this, "mpb", RenderingUtils.GetNewMaterialPropertyBlock);
            RenderingUtils.ReleaseMaterialPropertyBlock(props);
        }
    }
}
