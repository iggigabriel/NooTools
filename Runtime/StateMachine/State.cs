namespace Nootools
{
    public abstract class State<T> : IState<T> where T : class
    {
        public IStateMachine<T> StateMachine { get; set; }

        public T Target => StateMachine?.Target;

        public int TicksSinceStarted => StateMachine?.CurrentState == this ? StateMachine.TicksSinceLastStateChange : -1;

        public virtual void OnEnter() { }

        public virtual void OnExit() { }

        public virtual void OnUpdate() { }
    }
}