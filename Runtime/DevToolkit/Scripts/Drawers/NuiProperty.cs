using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Noo.DevToolkit
{
    public sealed class NuiProperty
    {
        internal Func<object> targetProvider;
        internal Func<object> getter;
        internal Action<object> setter;

        private bool forceReadOnly;

        public object Value
        {
            get => getter?.Invoke() ?? (PropertyType.IsValueType ? Activator.CreateInstance(PropertyType) : null);
            set => setter?.Invoke(value);
        }
        public Type DeclaringType { get; private set; }
        public Type PropertyType { get; private set; }
        public List<Attribute> Attributes { get; private set; }

        public bool IsReadOnly
        {
            get => setter == null || forceReadOnly;
            set => forceReadOnly = value;
        }

        public bool HasValidTarget(out object target)
        {
            if (targetProvider == null)
            {
                target = null;
                return true;
            }
            else
            {
                target = targetProvider();
                return target != null;
            }
        }

        public NuiProperty(MemberInfo memberInfo)
        {
            if (!IsValidMemberForProperty(memberInfo))
            {
                throw new NotSupportedException($"{memberInfo.GetType()} is not supported as property");
            }

            PropertyType = memberInfo.GetMemberPropertyType();
            DeclaringType = memberInfo.DeclaringType;

            if (memberInfo.IsMemberReadable())
            {
                getter = () =>
                {
                    if (HasValidTarget(out var target))
                    {
                        return memberInfo.GetMemberValue(target);
                    }
                    else
                    {
                        return default;
                    }
                };
            }

            if (memberInfo.IsMemberWritable())
            {
                setter = (x) =>
                {
                    if (x != null && x.GetType() != PropertyType) return;

                    if (HasValidTarget(out var target))
                    {
                        memberInfo.SetMemberValue(target, x);
                    }
                };
            }

            if (memberInfo is MethodInfo)
            {
                getter = () => memberInfo;
                setter = (x) => { };
            }

            Attributes = memberInfo.GetAllAttributes(true);

            // TODO implement more property target providers
            if (!memberInfo.IsStatic())
            {
                if (memberInfo.DeclaringType.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    targetProvider = () => UnityEngine.Object.FindAnyObjectByType(memberInfo.DeclaringType, FindObjectsInactive.Exclude);
                }
            }
        }

        public NuiProperty(ParameterInfo parameterInfo)
        {
            PropertyType = parameterInfo.ParameterType;
            DeclaringType = parameterInfo.Member.DeclaringType;

            var parameterValue = parameterInfo.HasDefaultValue ? parameterInfo.DefaultValue : (PropertyType.IsValueType ? Activator.CreateInstance(PropertyType) : null);

            getter = () => parameterValue;
            setter = (x) => parameterValue = x;

            Attributes = parameterInfo.GetAllAttributes(true);
        }

        public NuiProperty(Type type, Func<object> getter, Action<object> setter, params DrawerAttribute[] attributes)
        {
            PropertyType = type;
            DeclaringType = type;

            this.getter = getter;
            this.setter = setter;

            Attributes = attributes?.Cast<Attribute>().ToList();
        }

        public static bool IsValidMemberForProperty(MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo) return true;
            if (memberInfo is PropertyInfo propertInfo && propertInfo.CanRead) return true;
            if (memberInfo is MethodInfo methodInfo && !methodInfo.IsSpecialName) return true;
            return false;
        }
    }
}
