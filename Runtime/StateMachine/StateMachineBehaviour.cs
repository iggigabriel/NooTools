using UnityEngine;

namespace Nootools
{
    /// <summary>
    /// Deterministic Finite State Machine
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StateMachineBehaviour<T> : MonoBehaviour, IStateMachine<T> where T : MonoBehaviour
    {
        public T Target { get; private set; }

        public IState<T> PreviousState { get; private set; }

        public IState<T> CurrentState { get; private set; }

        /// <summary>
        /// Number of updates called since creation
        /// </summary>
        public int TickCount { get; private set; }

        /// <summary>
        /// Tick of last state change
        /// </summary>
        public int LastStateChangeTick { get; private set; }

        /// <summary>
        /// Number of updates called since last state changed
        /// </summary>
        public int TicksSinceLastStateChange => TickCount - LastStateChangeTick;

        protected virtual void Awake()
        {
            Target = GetComponent<T>();
        }

        protected virtual void Update()
        {
            TickCount++;

            CurrentState?.OnUpdate();
        }

        public void SetState(IState<T> state)
        {
            if (state != null && state.StateMachine != null && state.StateMachine != (IStateMachine<T>)this)
            {
                throw new System.Exception($"State ({state}) already belongs to another StateMachine.");
            }

            if (state != null)
            {
                state.StateMachine = this;
            }

            PreviousState = CurrentState;
            CurrentState = state;

            PreviousState?.OnExit();
            CurrentState?.OnEnter();
        }

        public void SetState<TState>(bool createIfDoesntExist = true) where TState : StateBehaviour<T>
        {
            if (Target == null)
            {
                throw new System.Exception($"StateMachine is not initialized.");
            }

            var newState = Target.GetComponent<TState>();

            if (newState == null && createIfDoesntExist)
            {
                newState = Target.gameObject.AddComponent<TState>();
            }

            SetState(newState);
        }
    }
}