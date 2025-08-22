using System;
using System.Collections.Generic;
using System.Reflection;
using static Noo.DevToolkit.DevToolkitUtility;

namespace Noo.DevToolkit
{
    public static class NuiDrawerUtility
    {
        static readonly Dictionary<Type, Type> propertyDrawerTypes = new();
        static readonly Dictionary<Type, Type> attributeDrawerTypes = new();

        static NuiDrawerUtility()
        {
            var drawerType = typeof(NuiPropertyDrawer);
            var baseType = typeof(NuiPropertyDrawer<>);
            var attributeDrawerType = typeof(NuiAttributeDrawer<>);

            //Find all property drawer types
            foreach (var type in DevTypes)
            {
                if (type.IsAbstract || !drawerType.IsAssignableFrom(type)) continue;

                if (type.TryGetGenericArg(baseType, out var genericType))
                {
                    if (!propertyDrawerTypes.ContainsKey(genericType) || propertyDrawerTypes[genericType].IsAssignableFrom(type))
                    {
                        propertyDrawerTypes[genericType] = type;
                    }
                }

                if (type.TryGetGenericArg(attributeDrawerType, out var attributeGenericType))
                {
                    if (!attributeDrawerTypes.ContainsKey(attributeGenericType) || attributeDrawerTypes[attributeGenericType].IsAssignableFrom(type))
                    {
                        attributeDrawerTypes[attributeGenericType] = type;
                    }
                }
            }
        }

        public static NuiPropertyDrawer CreateDrawer(MemberInfo memberInfo)
        {
            if (!NuiProperty.IsValidMemberForProperty(memberInfo))
            {
                throw new ArgumentException($"MemberInfo of type {memberInfo.GetType()} is not supported.");
            }

            var commandInfo = DevToolkitCommands.GetCommandInfo(memberInfo);
            var memberType = memberInfo.GetMemberPropertyType();

            var drawer = default(NuiPropertyDrawer);

            if (memberInfo is FieldInfo || memberInfo is PropertyInfo)
            {
                var drawerAttribute = memberInfo.GetCustomAttribute<DrawerAttribute>(true);
                TryCreateDrawerForAttribute(drawerAttribute, out drawer);
            }

            drawer ??= CreateDrawerForType(memberType);

            drawer.Label = commandInfo.displayName;
            drawer.Info = commandInfo.info;
            drawer.Order = commandInfo.order;
            drawer.Property = new NuiProperty(memberInfo);

            drawer.OnInit();

            return drawer;
        }

        public static NuiPropertyDrawer CreateDrawer(ParameterInfo parameterInfo)
        {
            var drawer = default(NuiPropertyDrawer);

            var drawerAttributes = parameterInfo.Member.GetCustomAttributes<DrawerAttribute>(true);

            foreach (var drawerAttribute in drawerAttributes)
            {
                if (drawerAttribute.Parameter == parameterInfo.Name)
                {
                    TryCreateDrawerForAttribute(drawerAttribute, out drawer);
                }
            }

            drawer ??= CreateDrawerForType(parameterInfo.ParameterType);

            drawer.Label = NiceifyName(parameterInfo.Name);
            drawer.Property = new NuiProperty(parameterInfo);

            drawer.OnInit();

            return drawer;
        }

        public static NuiPropertyDrawer CreateDrawer<T>(string label, Func<T> getter, Action<T> setter, params DrawerAttribute[] attributes)
        {
            var memberType = typeof(T);
            var drawer = default(NuiPropertyDrawer);

            if (attributes != null)
            {
                foreach (var attr in attributes)
                {
                    TryCreateDrawerForAttribute(attr, out drawer);
                }
            }

            drawer ??= CreateDrawerForType(memberType);

            drawer.Label = label;

            drawer.Property = new NuiProperty
            (
                memberType,
                () => getter != null ? getter() : default,
                (x) => { if (x == null) setter?.Invoke(default); if (x is T tx) setter?.Invoke(tx); },
                attributes
            );

            drawer.OnInit();

            return drawer;

        }

        static bool TryCreateDrawerForAttribute(DrawerAttribute attribute, out NuiPropertyDrawer drawer)
        {
            if (attribute != null && attributeDrawerTypes.TryGetValue(attribute.GetType(), out var drawerType))
            {
                var attrDrawer = Activator.CreateInstance(drawerType) as NuiAttributeDrawer;
                attrDrawer.DrawerAttribute = attribute;
                drawer = attrDrawer;
                return true;
            }
            else
            {
                drawer = default;
                return false;
            }
        }

        static NuiPropertyDrawer CreateDrawerForType(Type type)
        {
            // TODO check also base types

            if (type.IsEnum && propertyDrawerTypes.TryGetValue(typeof(Enum), out var enumDrawer))
            {
                return Activator.CreateInstance(enumDrawer) as NuiPropertyDrawer;
            }

            if (propertyDrawerTypes.TryGetValue(type, out var drawerType))
            {
                return Activator.CreateInstance(drawerType) as NuiPropertyDrawer;
            }

            return new NuiPropertyDrawerNull();
        }
    }
}
