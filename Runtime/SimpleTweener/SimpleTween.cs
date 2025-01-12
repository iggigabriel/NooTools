using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Noo.Tools.SimpleTweener
{
    [Serializable]
    public class SimpleTween
    {
        public enum RepeatBehaviour
        {
            None,
            Repeat,
        }

        [SerializeField]
        RepeatBehaviour repeat;

        [SerializeReference]
        public List<SimpleTweenAnimation> animations = new();

        [NonSerialized]
        GameObject target;

        [NonSerialized]
        int currentAnimationIndex;

        [NonSerialized]
        int currentAnimationLoop;

        [NonSerialized]
        float time;

        public void Play(GameObject go)
        {
            target = go;
            time = 0f;
            currentAnimationIndex = -1;
            currentAnimationLoop = 0;

            if (target)
            {
                foreach (var animation in animations)
                {
                    animation?.Init(target);
                }
            }

            PlayNext();
        }

        public void Reset()
        {
            if (target)
            {
                foreach (var animation in animations)
                {
                    animation?.Reset(target);
                }
            }

            target = null;
            time = 0f;
            currentAnimationIndex = -1;
        }

        public void Update(float deltaTime)
        {
            if (!target) return;
            if (currentAnimationIndex >= animations.Count) return;

            var animation = animations[currentAnimationIndex];

            if (animation == null)
            {
                time = 0f;
                PlayNext();
                Update(deltaTime);
            }
            else
            {
                time += deltaTime;

                if (time >= animation.duration)
                {
                    time -= animation.duration;
                    PlayNext();
                    Update(time);
                }
                else
                {
                    animation.Update(target, deltaTime);
                }
            }
        }

        private void PlayNext()
        {
            if (animations.Count == 0) return;

            currentAnimationIndex++;

            if (currentAnimationIndex >= animations.Count)
            {
                if (repeat == RepeatBehaviour.None)
                {
                    return;
                }
                else if (repeat == RepeatBehaviour.Repeat)
                {
                    currentAnimationIndex = 0;
                    currentAnimationLoop++;
                }
            }

            animations[currentAnimationIndex]?.Start(target);
            animations[currentAnimationIndex]?.Update(target, 0f);
        }
    }

    [Serializable, InlineProperty]
    public abstract class SimpleTweenAnimation
    {
        [HorizontalGroup]
        public float delay;

        [HorizontalGroup]
        public float duration;

        [InlineProperty, HideLabel]
        public SimpleTweenAnimationEasing easing;

        [NonSerialized]
        protected float currentTime;

        [NonSerialized]
        protected float normalizedTime;

        public virtual void Init(GameObject target)
        {
        }

        public virtual void Reset(GameObject target)
        {
        }

        public virtual void Start(GameObject target)
        {
            currentTime = 0f;

            if (easing.easingType == SimpleTweenAnimationEasing.Type.SecondOrderDynamics)
            {
                easing.sodCurve.Reset(0f);
                easing.sodCurve.Target = 1f;
            }
        }

        public virtual void Update(GameObject target, float deltaTime)
        {
            currentTime += deltaTime;

            if (easing.easingType == SimpleTweenAnimationEasing.Type.SecondOrderDynamics)
            {
                if (deltaTime > 0f) easing.sodCurve.Update(deltaTime);
            }

            if (duration == 0)
            {
                normalizedTime = 1f;
            }
            else
            {
                normalizedTime = easing.easingType switch
                {
                    SimpleTweenAnimationEasing.Type.Easing => Easing.Evaluate(easing.easingStyle, Mathf.Clamp01(currentTime / duration)),
                    SimpleTweenAnimationEasing.Type.AnimationCurve => easing.animationCurve.Evaluate(Mathf.Clamp01(currentTime / duration)),
                    SimpleTweenAnimationEasing.Type.SecondOrderDynamics => easing.sodCurve.Value,
                    _ => 1f,
                };
            }
        }
    }

    public abstract class SimpleTweenAnimation<T> : SimpleTweenAnimation
    {
        public SimpleTweenAnimationTargetType from;

        [ShowIf("@from == SimpleTweenAnimationTargetType.Fixed"), LabelText(" ")]
        public T fixedFromValue;

        public SimpleTweenAnimationTargetType to;

        [ShowIf("@to == SimpleTweenAnimationTargetType.Fixed"), LabelText(" ")]
        public T fixedToValue;

        [NonSerialized]
        protected T initialValue;

        [NonSerialized]
        protected T fromValue;

        [NonSerialized]
        protected T toValue;

        public override void Init(GameObject gameObject)
        {
            base.Init(gameObject);

            initialValue = GetValue(gameObject);
        }

        public override void Reset(GameObject target)
        {
            base.Reset(target);

            SetValue(target, initialValue);
        }

        public override void Start(GameObject gameObject)
        {
            base.Start(gameObject);

            fromValue = from switch
            {
                SimpleTweenAnimationTargetType.Initial => initialValue,
                SimpleTweenAnimationTargetType.Current => GetValue(gameObject),
                SimpleTweenAnimationTargetType.Fixed => fixedFromValue,
                _ => default,
            };

            toValue = from switch
            {
                SimpleTweenAnimationTargetType.Initial => initialValue,
                SimpleTweenAnimationTargetType.Current => GetValue(gameObject),
                SimpleTweenAnimationTargetType.Fixed => fixedToValue,
                _ => default,
            };
        }

        public override void Update(GameObject target, float deltaTime)
        {
            base.Update(target, deltaTime);

            SetValue(target, Lerp(fromValue, toValue, normalizedTime));
        }

        protected abstract T GetValue(GameObject target);
        protected abstract void SetValue(GameObject target, T value);
        protected abstract T Lerp(T from, T to, float t);
    }

    public enum SimpleTweenAnimationTargetType
    {
        Initial,
        Current,
        Fixed
    }

    [Serializable]
    public class SimpleTweenAnimationEasing
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
        public SODFloat sodCurve;
    }

    public abstract class SimpleTweenAnimationFloat : SimpleTweenAnimation<float>
    {
        protected override float Lerp(float from, float to, float t) => Mathf.LerpUnclamped(from, to, t);
    }

    public abstract class SimpleTweenAnimationVector2 : SimpleTweenAnimation<Vector2>
    {
        protected override Vector2 Lerp(Vector2 from, Vector2 to, float t) => Vector2.LerpUnclamped(from, to, t);
    }

    public abstract class SimpleTweenAnimationVector3 : SimpleTweenAnimation<Vector3>
    {
        protected override Vector3 Lerp(Vector3 from, Vector3 to, float t) => Vector3.LerpUnclamped(from, to, t);
    }

    public abstract class SimpleTweenAnimationColor : SimpleTweenAnimation<Color>
    {
        protected override Color Lerp(Color from, Color to, float t) => Color.LerpUnclamped(from, to, t);
    }

    [Serializable]
    public class TranslateLocal : SimpleTweenAnimationVector3
    {
        protected override Vector3 GetValue(GameObject target) => target.transform.localPosition;
        protected override void SetValue(GameObject target, Vector3 value) => target.transform.localPosition = value;
    }

    [Serializable]
    public class TranslateWorld : SimpleTweenAnimationVector3
    {
        protected override Vector3 GetValue(GameObject target) => target.transform.position;
        protected override void SetValue(GameObject target, Vector3 value) => target.transform.position = value;
    }

    [Serializable]
    public class RotateLocal : SimpleTweenAnimationVector3
    {
        protected override Vector3 GetValue(GameObject target) => target.transform.localEulerAngles;
        protected override void SetValue(GameObject target, Vector3 value) => target.transform.localEulerAngles = value;
    }

    [Serializable]
    public class RotateWorld : SimpleTweenAnimationVector3
    {
        protected override Vector3 GetValue(GameObject target) => target.transform.eulerAngles;
        protected override void SetValue(GameObject target, Vector3 value) => target.transform.eulerAngles = value;
    }

    [Serializable]
    public class ScaleLocal : SimpleTweenAnimationVector3
    {
        protected override Vector3 GetValue(GameObject target) => target.transform.localScale;
        protected override void SetValue(GameObject target, Vector3 value) => target.transform.localScale = value;
    }

    [Serializable]
    public class SpriteColor : SimpleTweenAnimationColor
    {
        protected override Color GetValue(GameObject target)
        {
            if (target.TryGetComponent<SpriteRenderer>(out var renderer))
            {
                return renderer.color;
            }
            else
            {
                return default;
            }
        }    
        
        protected override void SetValue(GameObject target, Color value)
        {
            if (target.TryGetComponent<SpriteRenderer>(out var renderer))
            {
                renderer.color = value;
            }
        }
    }
}
