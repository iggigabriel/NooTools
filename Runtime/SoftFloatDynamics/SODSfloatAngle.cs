using System;

namespace Noo.Tools
{
    /// <summary>
    /// High level wrapper for angle values, and targetless mode.
    /// Targetless mode only updates previous value without changing velocity.
    /// </summary>
    [Serializable]
    public class SODSfloatAngle : SfloatSODValue<Sfloat, Sfloat>
    {
        public override Sfloat PreviousValue { get => state.previousValue; set { state.previousValue = value; Update(Sfloat.Zero); } }
        public override Sfloat Velocity { get => state.velocity; set => state.velocity = value; }
        public override Sfloat PreviousTarget { get => state.previousTarget; set => state.previousTarget = value; }
        public override Sfloat Target { get => state.target; set => state.target = value; }

        public override Sfloat Value
        {
            get => state.value;
            set
            {
                state.@value = value;
                state.previousValue = value - state.velocity * state.timeFraction * SfloatSOD.DeltaTime;
            }
        }

        /// <summary>Returns value from 0 to 360 degrees</summary>
        public Sfloat NormalizedValue => SfGeom.NormalizeAngle(state.value);

        public bool HasTarget { get; set; } = true;

        public override void Update(Sfloat deltaTime)
        {
            if (HasTarget)
            {
                SfloatSOD.UpdateState(ref state, deltaTime);
            }
            else
            {
                state.timeFraction += deltaTime / deltaTime;

                while (state.timeFraction >= Sfloat.One)
                {
                    state.previousValue += state.velocity * deltaTime;
                    state.timeFraction -= Sfloat.One;
                }

                state.value = state.previousValue + state.velocity * state.timeFraction * deltaTime;
            }
        }

        public override void Reset(Sfloat value, bool resetVelocity = true, bool resetTime = true) => state.Reset(value, resetVelocity, resetTime);

        /// <summary>Sets target angle to value in range from -180 to +180 around previous target value</summary>
        public void SetTargetNormalized(Sfloat angle)
        {
            state.target = SfGeom.NormalizeAngleSignedAroundPivot(angle, state.target);
            var diff = state.target - state.value;
            if (Sfloat.Abs(diff) > Sfloat.FromInt(540)) state.target -= Sfloat.FromInt(360) * Sfloat.Sign(diff);
        }
    }
}
