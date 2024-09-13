using System.Runtime.CompilerServices;
using Unity.Burst;

namespace Noo.Tools
{
    [BurstCompile]
    public static class SfloatSOD
    {
        // If changed, all SODCurve K-Values  need to be recalculated, so i prefer to keep it as compile constant
        public static Sfloat DeltaTime { [MethodImpl(SfUtil.AggressiveInlining)] get { return Sfloat.Ratio100(1); } }

        [BurstCompile]
        public static void UpdateState(ref SfloatSODState<Sfloat> state, Sfloat deltaTime)
        {
            var k = state.kValues;

            state.time += deltaTime;
            state.timeFraction += deltaTime / DeltaTime;

            while (state.timeFraction >= Sfloat.One)
            {
                state.previousValue += state.velocity * DeltaTime;
                state.velocity += (state.target + k.z * (state.target - state.previousTarget) - state.previousValue - k.x * state.velocity) / k.y;

                state.timeFraction -= Sfloat.One;
                state.previousTarget = state.target;
            }

            state.value = state.previousValue + state.velocity * state.timeFraction * DeltaTime;
        }

        [BurstCompile]
        public static void UpdateState(ref SfloatSODState<Sfloat2> state, Sfloat deltaTime)
        {
            var k = state.kValues;

            state.time += deltaTime;
            state.timeFraction += deltaTime / DeltaTime;

            while (state.timeFraction >= Sfloat.One)
            {
                state.previousValue += state.velocity * DeltaTime;
                state.velocity += (state.target + k.z * (state.target - state.previousTarget) - state.previousValue - k.x * state.velocity) / k.y;

                state.timeFraction -= Sfloat.One;
                state.previousTarget = state.target;
            }

            state.value = state.previousValue + state.velocity * state.timeFraction * DeltaTime;
        }

        [BurstCompile]
        public static void UpdateState(ref SfloatSODState<Sfloat3> state, Sfloat deltaTime)
        {
            var k = state.kValues;

            state.time += deltaTime;
            state.timeFraction += deltaTime / DeltaTime;

            while (state.timeFraction >= Sfloat.One)
            {
                state.previousValue += state.velocity * DeltaTime;
                state.velocity += (state.target + k.z * (state.target - state.previousTarget) - state.previousValue - k.x * state.velocity) / k.y;

                state.timeFraction -= Sfloat.One;
                state.previousTarget = state.target;
            }

            state.value = state.previousValue + state.velocity * state.timeFraction * DeltaTime;
        }

        [BurstCompile]
        public static void UpdateState(ref SfloatSODState<Sfloat4> state, Sfloat deltaTime)
        {
            var k = state.kValues;

            state.time += deltaTime;
            state.timeFraction += deltaTime / DeltaTime;

            while (state.timeFraction >= Sfloat.One)
            {
                state.previousValue += state.velocity * DeltaTime;
                state.velocity += (state.target + k.z * (state.target - state.previousTarget) - state.previousValue - k.x * state.velocity) / k.y;

                state.timeFraction -= Sfloat.One;
                state.previousTarget = state.target;
            }

            state.value = state.previousValue + state.velocity * state.timeFraction * DeltaTime;
        }
    }
}