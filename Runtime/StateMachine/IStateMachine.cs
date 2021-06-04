namespace Nootools
{
    public interface IStateMachine<T> where T : class
    {
        T Target { get; }

        State<T> PreviousState { get; }

        State<T> CurrentState { get; }

        int TickCount { get; }

        int LastStateChangeTick { get; }

        int TicksSinceLastStateChange { get; }

        void SetState(State<T> state);
    }
}