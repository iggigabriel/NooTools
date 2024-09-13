using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Noo.Tools
{
    public interface IObservableValue
    {
        event Action OnChanged;
    }

    [InlineProperty]
    public class ObservableValue<T> : IObservableValue
    {
        public event Action OnChanged;

        [SerializeField, HorizontalGroup, HideLabel]
        private T value;

        public ObservableValue() 
        { 
        }

        public ObservableValue(T value)
        {
            this.value = value;
        }

        public T Value
        {
            get => value;
            set => Set(value);
        }

        public void Set(T value)
        {
            if (this.value is IEquatable<T> eValue && eValue.Equals(value)) return;
            else if (this.value.Equals(value)) return;
            this.value = value;
            OnChanged?.Invoke();
        }

        public void SetWithoutNotify(T value)
        {
            this.value = value;
        }

        public static implicit operator T(ObservableValue<T> multiLock) => multiLock.value;
    }

    [Serializable]
    public class ObservableBool : ObservableValue<bool>
    {
        public ObservableBool(bool value) : base(value)
        {
        }
    }

    [Serializable]
    public class ObservableInt : ObservableValue<int>
    {
        public ObservableInt(int value) : base(value)
        {
        }
    }
}
