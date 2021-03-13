namespace Nootools.StateMachine
{
    public abstract class State<T> where T : class
    {
        public T Parent { get; internal set; }

        protected virtual void OnEnter() { }

        protected virtual void OnExit() { }

        protected virtual void OnUpdate() { }
    }
}