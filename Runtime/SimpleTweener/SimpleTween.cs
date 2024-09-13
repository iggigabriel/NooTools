using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Noo.Tools
{
    [Serializable]
    public class SimpleTween
    {
        [SerializeReference]
        public List<SimpleTweenAnimation> animations = new();
    }

    [Serializable, InlineProperty]
    public abstract class SimpleTweenAnimation
    {
        [HorizontalGroup]
        public float delay;

        [Min(0), HorizontalGroup]
        public float duration;

        [InlineProperty, HideLabel]
        public SimpleTweenAnimationEasing easing;

        //public abstract void Init();
        //public abstract void Update(float deltaTime);
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
    }

    public enum SimpleTweenAnimationTargetType
    {
        Initial,
        Current,
        Fixed
    }

    [Serializable, InlineProperty]
    public abstract class SimpleTweenAnimationTarget<T>
    {
        [HideLabel]
        public SimpleTweenAnimationTargetType type;

        [HideLabel, ShowIf("@type == SimpleTweenAnimationTargetType.Fixed")]
        public T value;
    }

    [Serializable]
    public class SimpleTweenAnimationTargetVector3 : SimpleTweenAnimationTarget<Vector3>
    {

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

    [Serializable]
    public class SimpleTweenAnimationTranslateLocal : SimpleTweenAnimation<Vector3>
    {
    }

    [Serializable]
    public class SimpleTweenAnimationTranslateWorld : SimpleTweenAnimation<Vector3>
    {
    }
}
