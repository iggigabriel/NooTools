using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Noo.Tools
{
    [Serializable, InlineProperty]
    public struct NooNullable<T> : IEquatable<NooNullable<T>>, IEquatable<T> where T : struct
    {
        [SerializeField, ToggleLeft, HideLabel, HorizontalGroup(width: 18f), Tooltip("Has Value?")]
        private bool hasValue;

        [SerializeField, HorizontalGroup, EnableIf(nameof(hasValue)), LabelText(" "), LabelWidth(12f)]
        private T value;

        public NooNullable(T value) 
        {
            hasValue = true;
            this.value = value;
        }

        public readonly bool HasValue => hasValue;

        public readonly T Value => value;

        public override readonly bool Equals(object obj)
        {
            if (obj is NooNullable<T> nullable)
            {
                return Equals(nullable);
            }
            else if (obj is T val)
            {
                return hasValue && EqualityComparer<T>.Default.Equals(value, val);
            }
            else if (!hasValue && obj == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public readonly bool Equals(NooNullable<T> other)
        {
            return hasValue == other.hasValue && EqualityComparer<T>.Default.Equals(value, other.value);
        }

        public readonly bool Equals(T other)
        {
            return hasValue && EqualityComparer<T>.Default.Equals(value, other);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(hasValue, value);
        }

        public static bool operator ==(NooNullable<T> left, NooNullable<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NooNullable<T> left, NooNullable<T> right)
        {
            return !(left == right);
        }

        public static bool operator ==(NooNullable<T> left, T right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NooNullable<T> left, T right)
        {
            return !(left == right);
        }

        public static bool operator ==(T left, NooNullable<T> right)
        {
            return right.Equals(left);
        }

        public static bool operator !=(T left, NooNullable<T> right)
        {
            return !(left == right);
        }

        public static implicit operator NooNullable<T> (T? source)
        {
            return source.HasValue ? new NooNullable<T>(source.Value) : default;
        }

        public static implicit operator NooNullable<T> (T source)
        {
            return new NooNullable<T> (source);
        }

        public override readonly string ToString()
        {
            return hasValue ? value.ToString() : "Null";
        }
    }
}
