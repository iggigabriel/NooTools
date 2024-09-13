using System;
using System.Collections.Generic;

namespace Noo.Tools
{
    public class Gate
    {
        readonly HashSet<object> keys = new();

        public event Action OnStateChange;

        public bool IsOpen => keys.Count == 0;
        public bool IsLocked => keys.Count != 0;

        public void Lock(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var wasOpen = IsOpen;
            keys.Add(key);
            if (wasOpen && IsLocked) OnStateChange?.Invoke();
        }

        public object LockAndGetKey()
        {
            var key = new object();
            Lock(key);
            return key;
        }

        public void Unlock(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var wasLocked = IsLocked;
            keys.Remove(key);
            if (wasLocked && IsOpen) OnStateChange?.Invoke();
        }

        public void SetLocked(bool locked, object key)
        {
            if (locked) Lock(key); else Unlock(key);
        }

        public void UnlockAll()
        {
            var wasOpen = IsOpen;
            keys.Clear();
            if (wasOpen && IsLocked) OnStateChange?.Invoke();
        }
    }
}
