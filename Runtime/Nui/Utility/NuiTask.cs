using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Noo.Nui
{
    public class NuiTask : MonoBehaviour
    {
        static NuiTask instance;
        static readonly WaitForEndOfFrame endOfFrameWaiter = new();
        static readonly object locker = new();
        static List<DelayedAction> delayedActions = new(32);
        static List<DelayedAction> delayedActionsTemp = new(32);
        static List<Action> cancelledActions = new(32);

        private static event Action EveryUpdateEvent;
        private static event Action EveryLateUpdateEvent;
        private static event Action EveryFixedUpdateEvent;
        private static event Action EveryEndOfFrameEvent;

        public static event Action EveryUpdate
        {
            add { AssertInstance(); EveryLateUpdateEvent += value; }
            remove { EveryLateUpdateEvent -= value; }
        }

        public static event Action EveryLateUpdate
        {
            add { AssertInstance(); EveryUpdateEvent += value; }
            remove { EveryUpdateEvent -= value; }
        }

        public static event Action EveryFixedUpdate
        {
            add { AssertInstance(); EveryFixedUpdateEvent += value; }
            remove { EveryFixedUpdateEvent -= value; }
        }

        public static event Action EveryEndOfFrame
        {
            add { AssertInstance(); EveryEndOfFrameEvent += value; }
            remove { EveryEndOfFrameEvent -= value; }
        }

        public enum ExecutionOrder : short
        {
            Update,
            FixedUpdate,
            LateUpdate,
            EndOfFrame
        }

        public enum TimingUnit : short
        {
            Frames,
            Time,
            UnscaledTime,
        }

        private readonly struct DelayedAction
        {
            public readonly ExecutionOrder executionOrder;
            public readonly TimingUnit unit;
            public readonly float time;
            public readonly float interval;
            public readonly Action action;

            public DelayedAction(ExecutionOrder executionOrder, TimingUnit unit, float time, float interval, Action action)
            {
                this.executionOrder = executionOrder;
                this.unit = unit;
                this.time = time;
                this.interval = interval;
                this.action = action;
            }

            public readonly void Run()
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        static void AssertInstance()
        {
            if (!instance)
            {
                var go = new GameObject("NuiTaskController") { hideFlags = HideFlags.HideAndDontSave };
                instance = go.AddComponent<NuiTask>();
            }
        }

        static void Queue(DelayedAction action)
        {
            AssertInstance();

            lock (locker)
            {
                delayedActions.Add(action);
            }
        }

        public static void OnUpdate(Action action, int delayFrames = 0)
        {
            if (action == null) return;
            Queue(new DelayedAction(ExecutionOrder.Update, TimingUnit.Frames, Time.frameCount + delayFrames, -1f, action));
        }

        public static void OnLateUpdate(Action action, int delayFrames = 0)
        {
            if (action == null) return;
            Queue(new DelayedAction(ExecutionOrder.LateUpdate, TimingUnit.Frames, Time.frameCount + delayFrames, -1f, action));
        }

        public static void OnFixedUpdate(Action action, int delayFrames = 0)
        {
            if (action == null) return;
            Queue(new DelayedAction(ExecutionOrder.FixedUpdate, TimingUnit.Frames, Time.frameCount + delayFrames, -1f, action));
        }

        public static void OnEndOfFrame(Action action, int delayFrames = 0)
        {
            if (action == null) return;
            Queue(new DelayedAction(ExecutionOrder.EndOfFrame, TimingUnit.Frames, Time.frameCount + delayFrames, -1f, action));
        }

        public static void OnUpdate(Action action, float delayTime = 0, bool unscaled = false)
        {
            if (action == null) return;

            if (unscaled)
            {
                Queue(new DelayedAction(ExecutionOrder.Update, TimingUnit.UnscaledTime, Time.unscaledTime + delayTime, -1f, action));
            }
            else
            {
                Queue(new DelayedAction(ExecutionOrder.Update, TimingUnit.Time, Time.time + delayTime, -1f, action));
            }
        }

        public static void OnLateUpdate(Action action, float delayTime = 0, bool unscaled = false)
        {
            if (action == null) return;

            if (unscaled)
            {
                Queue(new DelayedAction(ExecutionOrder.LateUpdate, TimingUnit.UnscaledTime, Time.unscaledTime + delayTime, -1f, action));
            }
            else
            {
                Queue(new DelayedAction(ExecutionOrder.LateUpdate, TimingUnit.Time, Time.time + delayTime, -1f, action));
            }
        }

        public static void OnFixedUpdate(Action action, float delayTime = 0, bool unscaled = false)
        {
            if (action == null) return;

            if (unscaled)
            {
                Queue(new DelayedAction(ExecutionOrder.FixedUpdate, TimingUnit.UnscaledTime, Time.unscaledTime + delayTime, -1f, action));
            }
            else
            {
                Queue(new DelayedAction(ExecutionOrder.FixedUpdate, TimingUnit.Time, Time.time + delayTime, -1f, action));
            }
        }

        public static void OnEndOfFrame(Action action, float delayTime = 0, bool unscaled = false)
        {
            if (action == null) return;

            if (unscaled)
            {
                Queue(new DelayedAction(ExecutionOrder.EndOfFrame, TimingUnit.UnscaledTime, Time.unscaledTime + delayTime, -1f, action));
            }
            else
            {
                Queue(new DelayedAction(ExecutionOrder.EndOfFrame, TimingUnit.Time, Time.time + delayTime, -1f, action));
            }
        }

        public static void OnInterval(ExecutionOrder executionOrder, int intervalFrames, Action action)
        {
            if (action == null) return;
            Queue(new DelayedAction(executionOrder, TimingUnit.Frames, Time.frameCount + intervalFrames, intervalFrames, action));
        }

        public static void OnInterval(ExecutionOrder executionOrder, float intervalTime, bool unscaled, Action action)
        {
            if (action == null) return;

            if (unscaled)
            {
                Queue(new DelayedAction(executionOrder, TimingUnit.UnscaledTime, Time.unscaledTime + intervalTime, intervalTime, action));
            }
            else
            {
                Queue(new DelayedAction(executionOrder, TimingUnit.Time, Time.time + intervalTime, intervalTime, action));
            }
        }

        public static void Cancel(Action action)
        {
            if (action == null) return;
            cancelledActions.Add(action);
        }

        private void Update()
        {
            EveryUpdateEvent?.Invoke();
            RunNext(ExecutionOrder.Update);
        }

        private void LateUpdate()
        {
            EveryLateUpdateEvent?.Invoke();
            RunNext(ExecutionOrder.LateUpdate);
        }

        private void FixedUpdate()
        {
            EveryFixedUpdateEvent?.Invoke();
            RunNext(ExecutionOrder.FixedUpdate);
        }

        IEnumerator Start()
        {
            while (true)
            {
                yield return endOfFrameWaiter;
                EveryEndOfFrameEvent?.Invoke();
                RunNext(ExecutionOrder.EndOfFrame);
            }
        }

        static void RunNext(ExecutionOrder executionOrder)
        {
            var time = Time.time;
            var unscaledTime = Time.unscaledTime;
            var frameCount = (float)Time.frameCount;

            delayedActionsTemp.Clear();

            for (int i = 0; i < cancelledActions.Count; i++)
            {
                var cancelledAction = cancelledActions[i];

                for (int j = 0; j < delayedActions.Count; j++)
                {
                    if (delayedActions[j].action == cancelledAction)
                    {
                        lock (locker) { RemoveAtPushBack(delayedActions, j--); }
                    }
                }
            }

            cancelledActions.Clear();

            (delayedActions, delayedActionsTemp) = (delayedActionsTemp, delayedActions);

            for (int i = 0; i < delayedActionsTemp.Count; i++)
            {
                var delayedAction = delayedActionsTemp[i];

                if (delayedAction.executionOrder != executionOrder) continue;
                
                float actionTime = delayedAction.unit switch
                {
                    TimingUnit.Frames => frameCount,
                    TimingUnit.Time => time,
                    TimingUnit.UnscaledTime => unscaledTime,
                    _ => default,
                };

                if (actionTime >= delayedAction.time)
                {
                    delayedAction.Run();

                    lock (locker) { RemoveAtPushBack(delayedActionsTemp, i--); }

                    if (delayedAction.interval >= 0)
                    {
                        var nextIterval = delayedAction.unit == TimingUnit.UnscaledTime ? actionTime : delayedAction.time;
                        Queue(new DelayedAction(delayedAction.executionOrder, delayedAction.unit, nextIterval + delayedAction.interval, delayedAction.interval, delayedAction.action));
                    }
                }
            }

            for (int i = 0; i < delayedActionsTemp.Count; i++)
            {
                delayedActions.Add(delayedActionsTemp[i]);
            }
        }

        public static void RemoveAtPushBack<T>(List<T> list, int index)
        {
            var lastIndex = list.Count - 1;
            list[index] = list[lastIndex];
            list.RemoveAt(lastIndex);
        }
    }
}
