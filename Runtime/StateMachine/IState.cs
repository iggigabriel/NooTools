using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nootools
{
    public interface IState<T> where T : class
    {
        IStateMachine<T> StateMachine { get; set; }

        T Target { get; }

        int TicksSinceStarted { get; }

        void OnEnter();

        void OnExit();

        void OnUpdate();
    }
}