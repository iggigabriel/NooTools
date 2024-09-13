using System;
using UnityEngine;

namespace Noo.Tools
{
    /// <summary>
    /// Reference to a class <see cref="System.Type"/> with support for Unity serialization.
    /// </summary>
    [Serializable]
    public sealed class TypeReference : ISerializationCallbackReceiver
    {

        public static string GetClassRef(Type type)
        {
            return type != null
                ? type.FullName + ", " + type.Assembly.GetName().Name
                : "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeReference"/> class.
        /// </summary>
        public TypeReference()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeReference"/> class.
        /// </summary>
        /// <param name="assemblyQualifiedClassName">Assembly qualified class name.</param>
        public TypeReference(string assemblyQualifiedClassName)
        {
            Type = !string.IsNullOrEmpty(assemblyQualifiedClassName)
                ? Type.GetType(assemblyQualifiedClassName)
                : null;
        }

        public T CreateInstance<T>() where T : class
        {
            if (Type == null)
            {
                return null;
            }
            else
            {
                return Activator.CreateInstance(this) as T;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeReference"/> class.
        /// </summary>
        /// <returns>The instance.</returns>
        /// <param name="args">Arguments.</param>
        /// <typeparam name="T">Assembly qualified class name.</typeparam>
        public T CreateInstanceWithArguments<T>(params object[] args) where T : class
        {
            if (Type == null)
            {
                return null;
            }
            else
            {
                return Activator.CreateInstance(this, args) as T;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeReference"/> class.
        /// </summary>
        /// <param name="type">Class type.</param>
        /// <exception cref="System.ArgumentException">
        /// If <paramref name="type"/> is not a class type.
        /// </exception>
        public TypeReference(Type type)
        {
            Type = type;
        }

        [SerializeField]
        private string _classRef;

        #region ISerializationCallbackReceiver Members

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(_classRef))
            {
                _type = System.Type.GetType(_classRef);

                if (_type == null)
                    Debug.LogWarning(string.Format("'{0}' was referenced but class type was not found.", _classRef));
            }
            else
            {
                _type = null;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        #endregion

        private Type _type;

        public bool HasValue => _type != null;

        /// <summary>
        /// Gets or sets type of class reference.
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// If <paramref name="value"/> is not a class type.
        /// </exception>
        public Type Type
        {
            get { return _type; }
            set
            {
                if (value != null && !value.IsClass)
                    throw new ArgumentException(string.Format("'{0}' is not a class type.", value.FullName), "value");

                _type = value;
                _classRef = GetClassRef(value);
            }
        }

        public static implicit operator string(TypeReference typeReference)
        {
            return typeReference._classRef;
        }

        public static implicit operator Type(TypeReference typeReference)
        {
            return typeReference.Type;
        }

        public static implicit operator TypeReference(Type type)
        {
            return new TypeReference(type);
        }

        public override string ToString()
        {
            return Type != null ? Type.FullName : "(None)";
        }

    }

}
