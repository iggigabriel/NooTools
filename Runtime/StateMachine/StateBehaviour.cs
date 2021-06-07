using UnityEngine;

namespace Nootools
{
    public abstract class StateBehaviour<T> : MonoBehaviour, IState<T> where T : class
    {
        public IStateMachine<T> StateMachine { get; set; }

        public T Target => StateMachine?.Target;

        public int TicksSinceStarted => StateMachine?.CurrentState == (IStateMachine<T>)this ? StateMachine.TicksSinceLastStateChange : -1;

        public virtual void OnEnter() 
        {
            enabled = true;
        }

        public virtual void OnExit() 
        {
            enabled = false;
        }

        public virtual void OnUpdate() { }

#if UNITY_EDITOR
        void Reset()
        {
            enabled = false;
        }
#endif
    }
}
