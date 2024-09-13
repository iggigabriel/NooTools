using System;
using UnityEngine;

namespace Noo.Tools
{
    /// <summary>
    /// Indicates how selectable classes should be collated in drop-down menu.
    /// </summary>
    public enum TypeGrouping
    {
        /// <summary>
        /// No grouping, just show type names in a list; for instance, "Some.Nested.Namespace.SpecialClass".
        /// </summary>
        None,
        /// <summary>
        /// Group classes by namespace and show foldout menus for nested namespaces; for
        /// instance, "Some > Nested > Namespace > SpecialClass".
        /// </summary>
        ByNamespace,
        /// <summary>
        /// Group classes by namespace; for instance, "Some.Nested.Namespace > SpecialClass".
        /// </summary>
        ByNamespaceFlat,
        /// <summary>
        /// Group classes in the same way as Unity does for its component menu. This
        /// grouping method must only be used for <see cref="MonoBehaviour"/> types.
        /// </summary>
        ByAddComponentMenu,
    }

    /// <summary>
    /// Base class for class selection constraints that can be applied when selecting
    /// a <see cref="TypeReference"/> with the Unity inspector.
    /// </summary>
    public abstract class TypeConstraintAttribute : PropertyAttribute
    {

        private TypeGrouping _grouping = TypeGrouping.ByNamespaceFlat;
        private bool _allowAbstract = false;

        /// <summary>
        /// Gets or sets grouping of selectable classes. Defaults to <see cref="TypeGrouping.ByNamespaceFlat"/>
        /// unless explicitly specified.
        /// </summary>
        public TypeGrouping Grouping
        {
            get { return _grouping; }
            set { _grouping = value; }
        }

        /// <summary>
        /// Gets or sets whether abstract classes can be selected from drop-down.
        /// Defaults to a value of <c>false</c> unless explicitly specified.
        /// </summary>
        public bool AllowAbstract
        {
            get { return _allowAbstract; }
            set { _allowAbstract = value; }
        }

        /// <summary>
        /// Determines whether the specified <see cref="Type"/> satisfies filter constraint.
        /// </summary>
        /// <param name="type">Type to test.</param>
        /// <returns>
        /// A <see cref="bool"/> value indicating if the type specified by <paramref name="type"/>
        /// satisfies this constraint and should thus be selectable.
        /// </returns>
        public virtual bool IsConstraintSatisfied(Type type)
        {
            return AllowAbstract || !type.IsAbstract;
        }

    }

    /// <summary>
    /// Constraint that allows selection of classes that extend a specific class when
    /// selecting a <see cref="TypeReference"/> with the Unity inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class TypeExtendsAttribute : TypeConstraintAttribute
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeExtendsAttribute"/> class.
        /// </summary>
        public TypeExtendsAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeExtendsAttribute"/> class.
        /// </summary>
        /// <param name="baseType">Type of class that selectable classes must derive from.</param>
        public TypeExtendsAttribute(Type baseType)
        {
            BaseType = baseType;
        }

        /// <summary>
        /// Gets the type of class that selectable classes must derive from.
        /// </summary>
        public Type BaseType { get; private set; }

        /// <inheritdoc/>
        public override bool IsConstraintSatisfied(Type type)
        {
            return base.IsConstraintSatisfied(type)
                && BaseType.IsAssignableFrom(type) && type != BaseType;
        }

    }

    /// <summary>
    /// Constraint that allows selection of classes that implement a specific interface
    /// when selecting a <see cref="TypeReference"/> with the Unity inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class TypeImplementsAttribute : TypeConstraintAttribute
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeImplementsAttribute"/> class.
        /// </summary>
        public TypeImplementsAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeImplementsAttribute"/> class.
        /// </summary>
        /// <param name="interfaceType">Type of interface that selectable classes must implement.</param>
        public TypeImplementsAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }

        /// <summary>
        /// Gets the type of interface that selectable classes must implement.
        /// </summary>
        public Type InterfaceType { get; private set; }

        /// <inheritdoc/>
        public override bool IsConstraintSatisfied(Type type)
        {
            if (base.IsConstraintSatisfied(type))
            {
                foreach (var interfaceType in type.GetInterfaces())
                    if (interfaceType == InterfaceType)
                        return true;
            }
            return false;
        }

    }

}
