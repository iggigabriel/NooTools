using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Noo.Tools
{
    [Serializable, InlineProperty]
    public struct Optional<T> : IEquatable<Optional<T>>
    {
        [HorizontalGroup(20), HideLabel]
        [SerializeField, JsonProperty]
        bool hasValue;

        [HorizontalGroup, HideLabel]
        [EnableIf(nameof(hasValue))]
        [SerializeField, JsonProperty]
        T value;

        [JsonIgnore]
        public readonly bool HasValue => hasValue;

        [JsonIgnore]
        public T Value
        {
            readonly get
            {
                if (!hasValue)
                {
                    throw new InvalidOperationException("Optional has no value.");
                }

                return value;
            }
            set
            {
                this.value = value;
                hasValue = value != null;
            }
        }

        public Optional(T value)
        {
            this.value = value;
            hasValue = value != null;
        }

        public readonly T GetValueOrDefault()
        {
            return hasValue ? value : default;
        }

        public readonly T GetValueOrDefault(T defaultValue)
        {
            return hasValue ? value : defaultValue;
        }

        public void Clear()
        {
            hasValue = false;
            value = default;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Optional<T> other && Equals(other);
        }

        public readonly bool Equals(Optional<T> other)
        {
            if (hasValue != other.hasValue)
            {
                return false;
            }

            if (!hasValue)
            {
                return true;
            }

            return EqualityComparer<T>.Default.Equals(value, other.value);
        }

        public override readonly int GetHashCode()
        {
            unchecked
            {
                return hasValue
                    ? (hasValue.GetHashCode() * 397) ^ EqualityComparer<T>.Default.GetHashCode(value)
                    : hasValue.GetHashCode();
            }
        }

        public override readonly string ToString()
        {
            return hasValue ? value.ToString() ?? "null" : string.Empty;
        }

        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }

        public static explicit operator T(Optional<T> optional)
        {
            return optional.Value;
        }

        public static bool operator ==(Optional<T> left, Optional<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Optional<T> left, Optional<T> right)
        {
            return !left.Equals(right);
        }
    }
}
