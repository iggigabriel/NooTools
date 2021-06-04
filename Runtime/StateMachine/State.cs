namespace Nootools
{
    public abstract class State<T> where T : class
    {
        public IStateMachine<T> StateMachine { get; internal set; }

        public T Target => StateMachine?.Target;

        public int TicksSinceStarted => StateMachine?.CurrentState == this ? StateMachine.TicksSinceLastStateChange : -1;

        internal virtual void OnEnter() { }

        internal virtual void OnExit() { }

        internal virtual void OnUpdate() { }
    }
}