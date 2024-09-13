using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Noo.Tools
{
    [BurstCompile]
    public struct SODJobSfloat : IJobParallelFor
    {
        readonly float deltaTime;

        [ReadOnly]
        public NativeArray<SODState<float>> inStates;

        [WriteOnly]
        public NativeArray<SODState<float>> outStates;

        public void Execute(int index)
        {
            var state = inStates[index];

            SecondOrderDynamics.UpdateState(ref state, deltaTime);

            outStates[index] = state;
        }
    }

    [BurstCompile]
    public struct SODJobSfloat2 : IJobParallelFor
    {
        readonly float deltaTime;

        [ReadOnly]
        public NativeArray<SODState<float2>> inStates;

        [WriteOnly]
        public NativeArray<SODState<float2>> outStates;

        public void Execute(int index)
        {
            var state = inStates[index];

            SecondOrderDynamics.UpdateState(ref state, deltaTime);

            outStates[index] = state;
        }
    }

    [BurstCompile]
    public struct SODJobSfloat3 : IJobParallelFor
    {
        readonly float deltaTime;

        [ReadOnly]
        public NativeArray<SODState<float3>> inStates;

        [WriteOnly]
        public NativeArray<SODState<float3>> outStates;

        public void Execute(int index)
        {
            var state = inStates[index];

            SecondOrderDynamics.UpdateState(ref state, deltaTime);

            outStates[index] = state;
        }
    }

    [BurstCompile]
    public struct SODJobSfloat4 : IJobParallelFor
    {
        readonly float deltaTime;

        [ReadOnly]
        public NativeArray<SODState<float4>> inStates;

        [WriteOnly]
        public NativeArray<SODState<float4>> outStates;

        public void Execute(int index)
        {
            var state = inStates[index];

            SecondOrderDynamics.UpdateState(ref state, deltaTime);

            outStates[index] = state;
        }
    }

}