using System;
using UnityEngine;

namespace Noo.Tools
{
    [Serializable]
    public abstract class SfloatSODValue<TValue, TState>
        where TValue : struct
        where TState : struct
    {
        [SerializeField]
        protected SfloatSODCurve curve = SfloatSODCurve.Default;

        [SerializeField]
        protected SfloatSODState<TState> state = new(default);

        public Sfloat TimeFraction => state.timeFraction;
        public Sfloat Time => state.time;

        public SfloatSODCurve Curve { get => curve; set { curve = value; state.kValues = curve.KValues; } }
        public SfloatSODState<TState> State { get => state; set => state = value; }

        public abstract TValue PreviousValue { get; set; }
        public abstract TValue Velocity { get; set; }
        public abstract TValue PreviousTarget { get; set; }
        public abstract TValue Target { get; set; }

        public abstract TValue Value { get; set; }

        public abstract void Update(Sfloat deltaTime);
        public abstract void Reset(TValue value, bool resetVelocity = true, bool resetTime = true);

        public TValue Extrapolate(Sfloat deltaTime)
        {
            var tempState = state;
            Update(deltaTime);
            var value = Value;
            state = tempState;
            return value;
        }
    }

    [Serializable]
    public class SODSfloat : SfloatSODValue<Sfloat, Sfloat>
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
                state.value = value;
                state.previousValue = value - state.velocity * state.timeFraction * SfloatSOD.DeltaTime;
            }
        }

        public override void Update(Sfloat deltaTime) => SfloatSOD.UpdateState(ref state, deltaTime);
        public override void Reset(Sfloat value, bool resetVelocity = true, bool resetTime = true) => state.Reset(value, resetVelocity, resetTime);
    }

    [Serializable]
    public class SODSfloat2 : SfloatSODValue<Sfloat2, Sfloat2>
    {
        public override Sfloat2 PreviousValue { get => state.previousValue; set { state.previousValue = value; Update(Sfloat.Zero); } }
        public override Sfloat2 Velocity { get => state.velocity; set => state.velocity = value; }
        public override Sfloat2 PreviousTarget { get => state.previousTarget; set => state.previousTarget = value; }
        public override Sfloat2 Target { get => state.target; set => state.target = value; }

        public override Sfloat2 Value
        {
            get => state.value;
            set
            {
                state.value = value;
                state.previousValue = value - state.velocity * state.timeFraction * SfloatSOD.DeltaTime;
            }
        }

        public override void Update(Sfloat deltaTime) => SfloatSOD.UpdateState(ref state, deltaTime);
        public override void Reset(Sfloat2 value, bool resetVelocity = true, bool resetTime = true) => state.Reset(value, resetVelocity, resetTime);

    }

    [Serializable]
    public class SODSfloat3 : SfloatSODValue<Sfloat3, Sfloat3>
    {
        public override Sfloat3 PreviousValue { get => state.previousValue; set { state.previousValue = value; Update(Sfloat.Zero); } }
        public override Sfloat3 Velocity { get => state.velocity; set => state.velocity = value; }
        public override Sfloat3 PreviousTarget { get => state.previousTarget; set => state.previousTarget = value; }
        public override Sfloat3 Target { get => state.target; set => state.target = value; }

        public override Sfloat3 Value
        {
            get => state.value;
            set
            {
                state.value = value;
                state.previousValue = value - state.velocity * state.timeFraction * SfloatSOD.DeltaTime;
            }
        }

        public override void Update(Sfloat deltaTime) => SfloatSOD.UpdateState(ref state, deltaTime);
        public override void Reset(Sfloat3 value, bool resetVelocity = true, bool resetTime = true) => state.Reset(value, resetVelocity, resetTime);

    }

    [Serializable]
    public class SODSfloat4 : SfloatSODValue<Sfloat4, Sfloat4>
    {
        public override Sfloat4 PreviousValue { get => state.previousValue; set { state.previousValue = value; Update(Sfloat.Zero); } }
        public override Sfloat4 Velocity { get => state.velocity; set => state.velocity = value; }
        public override Sfloat4 PreviousTarget { get => state.previousTarget; set => state.previousTarget = value; }
        public override Sfloat4 Target { get => state.target; set => state.target = value; }

        public override Sfloat4 Value
        {
            get => state.value;
            set
            {
                state.value = value;
                state.previousValue = value - state.velocity * state.timeFraction * SfloatSOD.DeltaTime;
            }
        }

        public override void Update(Sfloat deltaTime) => SfloatSOD.UpdateState(ref state, deltaTime);
        public override void Reset(Sfloat4 value, bool resetVelocity = true, bool resetTime = true) => state.Reset(value, resetVelocity, resetTime);
    }
}