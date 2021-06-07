namespace Nootools
{
    public interface IStateMachine<T> where T : class
    {
        T Target { get; }

        IState<T> PreviousState { get; }

        IState<T> CurrentState { get; }

        int TickCount { get; }

        int LastStateChangeTick { get; }

        int TicksSinceLastStateChange { get; }

        void SetState(IState<T> state);
    }
}