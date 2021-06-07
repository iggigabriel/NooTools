namespace Nootools
{
    /// <summary>
    /// Deterministic Finite State Machine
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StateMachine<T> : IStateMachine<T> where T : class
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

        public StateMachine(T target)
        {
            Target = target ?? throw new System.NullReferenceException("Target object of StateMachine cant be null.");
        }

        public void Update()
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
    }
}