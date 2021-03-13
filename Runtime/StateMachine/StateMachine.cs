namespace Nootools.StateMachine
{
    public class StateMachine<TParent, TState> where TParent : class where TState : State<TParent>
    {
        private TState previousState = null;
        private TState currentState = null;

        /// <summary>
        /// Number of updates called since creation
        /// </summary>
        private int tickCount = 0;

        /// <summary>
        /// Tick of last state change
        /// </summary>
        private int lastStateChangeTick = 0;

        /// <summary>
        /// Number of updates called since last state changed
        /// </summary>
        public int TicksSinceLastStateChange => tickCount - lastStateChangeTick;

        public void Update()
        {
            tickCount++;
        }
    }
}