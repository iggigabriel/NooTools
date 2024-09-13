using Sirenix.OdinInspector;
using UnityEngine;

namespace Noo.Tools
{
    public abstract class LinkedValueSource<T> : ScriptableObject
    {
        [SerializeField, HideLabel]
        private T value;

        public T Value
        {
            get => value;
            set => this.value = value;
        }
    }
}
