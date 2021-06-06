namespace Nootools
{
    public abstract class State<T> where T : class
    {
        public IStateMachine<T> StateMachine { get; internal set; }

        public T Target => StateMachine?.Target;

        public int TicksSinceStarted => StateMachine?.CurrentState == this ? StateMachine.TicksSinceLastStateChange : -1;

        internal protected virtual void OnEnter() { }

        internal protected virtual void OnExit() { }

        internal protected virtual void OnUpdate() { }
    }
}