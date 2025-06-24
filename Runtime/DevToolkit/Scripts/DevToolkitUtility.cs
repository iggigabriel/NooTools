using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine.Pool;

namespace Noo.DevToolkit
{
    internal static class DevToolkitUtility
    {
        static readonly StringBuilder stringBuilder = new(1024);

        static readonly List<Assembly> devAssemblies;
        static readonly List<Type> devTypes;
        static readonly Dictionary<string, Type> devTypesMap;

        static readonly string[] hNotationPrefix = new string[] { "g_", "m_", "s_", "c_" };
        static readonly char[] nameTrimChars = new char[] { ' ', '/', '\\', '_', '-' };

        static readonly char[] pathSplitChars = new char[] { '/', '\\' };
        static readonly char[] pathTrimChars = new char[] { ' ', '/', '\\' };

        static readonly Dictionary<string, string> niceifiedNames = new();

        public static IReadOnlyList<Assembly> DevAssemblies => devAssemblies;
        public static IReadOnlyList<Type> DevTypes => devTypes;
        public static IReadOnlyDictionary<string, Type> DevTypesMap => devTypesMap;

        public static BindingFlags BindFlagAll => BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        public static BindingFlags BindFlagAllPublic => BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;
        public static BindingFlags BindFlagAllPrivate => BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

        static DevToolkitUtility()
        {
            devAssemblies = FindAssembliesWithAttribute<DevAssemblyAttribute>();
            devTypes = devAssemblies.SelectMany(x => x.GetTypes()).ToList();
            devTypesMap = devTypes.ToDictionary(x => x.FullName);
        }

        /// <summary>Ex: "m_thisIsCamelCase" -> "This Is Camel Case"</summary>
        public static string NiceifyName(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            if (niceifiedNames.TryGetValue(input, out var name)) return name;

            stringBuilder.Clear();

            foreach (var prefix in hNotationPrefix) if (input.StartsWith(prefix)) input = input[2..];

            input = input.Trim(nameTrimChars);

            if (string.IsNullOrEmpty(input)) return string.Empty;

            if (char.IsLetter(input[0])) stringBuilder.Append(char.ToUpper(input[0]));
            else stringBuilder.Append(input[0]);

            for (int i = 1; i < input.Length; i++)
            {
                char c = input[i];
                if (char.IsUpper(c) && !char.IsUpper(input[i - 1])) stringBuilder.Append(' ');
                stringBuilder.Append(c);
            }

            return niceifiedNames[input] = stringBuilder.ToString();
        }

        /// <summary>
        /// Returns the type name. If this is a generic type, appends
        /// the list of generic type arguments between angle brackets.
        /// (Does not account for embedded / inner generic arguments.)
        /// </summary>
        public static string GetFormattedTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                string genericArguments = type.GetGenericArguments().Select(x => x.Name).Aggregate((x1, x2) => $"{x1}, {x2}");
                return $"{type.Name[..type.Name.IndexOf("`")]}<{genericArguments}>";
            }

            return type.Name;
        }

        /// <summary>
        /// Returns the type name. If this is a generic type, appends
        /// the list of generic type arguments between angle brackets.
        /// (Does not account for embedded / inner generic arguments.)
        /// </summary>
        public static string GetFormattedTypeFullName(Type type)
        {
            if (type.IsGenericType)
            {
                string genericArguments = type.GetGenericArguments().Select(x => x.FullName).Aggregate((x1, x2) => $"{x1}, {x2}");
                return $"{type.FullName[..type.FullName.IndexOf("`")]}<{genericArguments}>";
            }

            return type.Name;
        }

        public static List<Assembly> FindAssembliesWithAttribute<T>() where T : Attribute
        {
            var assemblies = ListPool<Assembly>.Get();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetCustomAttribute<T>() != null) assemblies.Add(assembly);
            }

            return assemblies;
        }

        public static string TrimPath(string path)
        {
            return path.Trim(pathTrimChars);
        }

        public static string[] SplitPath(string path)
        {
            return path.Split(pathSplitChars, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string GetLastPathPart(string path)
        {
            return Path.GetFileNameWithoutExtension(TrimPath(path));
        }

        public static bool TryGetParentPath(string path, out string parentPath)
        {
            path = TrimPath(path);

            var index = Math.Max(path.LastIndexOf('\\'), path.LastIndexOf("/"));

            if (index == 0)
            {
                parentPath = string.Empty;
                return true;
            }
            else if (index > 0)
            {
                parentPath = path[..index];
                return true;
            }
            else
            {
                parentPath = string.Empty;
                return false;
            }
        }

        public static object GetTypesDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        public static IReadOnlyList<string> ParseSearchQuery(string query)
        {
            // TODO OPTIMIZE THIS NO GC
            return query.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim().ToLowerInvariant()).ToList();
        }

        internal static object GetMemberValue(this MemberInfo member, object target)
        {
            if (member is FieldInfo fieldInfo) return fieldInfo.GetValue(target);
            else if (member is PropertyInfo propertyInfo) return propertyInfo.GetValue(target);
            else return default;
        }

        internal static void SetMemberValue(this MemberInfo member, object target, object value)
        {
            if (member is FieldInfo fieldInfo) fieldInfo.SetValue(target, value);
            else if (member is PropertyInfo propertyInfo) propertyInfo.SetValue(target, value);
        }

        internal static Type GetMemberType(this MemberInfo member)
        {
            if (member is FieldInfo fieldInfo) return fieldInfo.FieldType;
            else if (member is PropertyInfo propertyInfo) return propertyInfo.PropertyType;
            return default;
        }

        internal static Type GetMemberPropertyType(this MemberInfo member)
        {
            if (member is FieldInfo fieldInfo) return fieldInfo.FieldType;
            else if (member is PropertyInfo propertyInfo) return propertyInfo.PropertyType;
            else if (member is MethodInfo) return typeof(MethodInfo);
            return default;
        }

        internal static bool IsMemberReadable(this MemberInfo member)
        {
            if (member is PropertyInfo property) return property.CanRead;
            if (member is FieldInfo) return true;
            return false;
        }

        internal static bool IsMemberWritable(this MemberInfo member)
        {
            if (member is PropertyInfo property) return property.CanWrite;
            if (member is FieldInfo fieldInfo) return !fieldInfo.IsInitOnly;
            return false;
        }

        internal static bool TryGetGenericArgs(this Type type, Type genericDefinition, out Type[] genericTypes)
        {
            if (type.IsGenericOfType(genericDefinition))
            {
                genericTypes = type.GetGenericArguments();
                return true;
            }
            else if (type.BaseType != null)
            {
                return type.BaseType.TryGetGenericArgs(genericDefinition, out genericTypes);
            }
            else
            {
                genericTypes = null;
                return false;
            }
        }

        internal static bool TryGetGenericArg(this Type type, Type genericDefinition, out Type genericType)
        {
            if (type.IsGenericOfType(genericDefinition))
            {
                genericType = type.GetGenericArguments().FirstOrDefault();
                return true;
            }
            else if (type.BaseType != null)
            {
                return type.BaseType.TryGetGenericArg(genericDefinition, out genericType);
            }
            else
            {
                genericType = null;
                return false;
            }
        }

        internal static bool IsGenericOfType(this Type type, Type genericDefinition)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == genericDefinition;
        }

        internal static bool IsStatic(this MemberInfo member)
        {
            FieldInfo fieldInfo = member as FieldInfo;
            if (fieldInfo != null)
            {
                return fieldInfo.IsStatic;
            }

            PropertyInfo propertyInfo = member as PropertyInfo;
            if (propertyInfo != null)
            {
                if (!propertyInfo.CanRead)
                {
                    return propertyInfo.GetSetMethod(nonPublic: true).IsStatic;
                }

                return propertyInfo.GetGetMethod(nonPublic: true).IsStatic;
            }

            MethodBase methodBase = member as MethodBase;
            if (methodBase != null)
            {
                return methodBase.IsStatic;
            }

            EventInfo eventInfo = member as EventInfo;
            if (eventInfo != null)
            {
                return eventInfo.GetRaiseMethod(nonPublic: true).IsStatic;
            }

            Type type = member as Type;
            if (type != null)
            {
                if (type.IsSealed)
                {
                    return type.IsAbstract;
                }

                return false;
            }

            string message = string.Format(CultureInfo.InvariantCulture, "Unable to determine IsStatic for member {0}.{1}MemberType was {2} but only fields, properties, methods, events and types are supported.", member.DeclaringType.FullName, member.Name, member.GetType().FullName);
            throw new NotSupportedException(message);
        }

        internal static List<Attribute> GetAllAttributes(this ICustomAttributeProvider provider, bool inherit = false)
        {
            var rawAttr = provider.GetCustomAttributes(inherit);
            var attributes = new List<Attribute>(rawAttr.Length);

            for (int i = 0; i < rawAttr.Length; i++)
            {
                if (rawAttr[i] is Attribute attr) attributes.Add(attr);
            }

            return attributes;
        }

        internal static MemberInfo ParseMemberInfo(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentException("Input string is null or empty.");

            int lastDot = fullPath.LastIndexOf('.');

            if (lastDot == -1 || lastDot == fullPath.Length - 1)
                throw new ArgumentException("Invalid format. Expected format: Namespace.ClassName.MemberName");

            var typeName = fullPath[..lastDot];
            var memberName = fullPath[(lastDot + 1)..];

            if (devTypesMap.TryGetValue(typeName, out var targetType))
            {
                var member = targetType.GetMember(memberName, BindFlagAll).FirstOrDefault();
                return member ?? throw new MissingMemberException($"Member '{memberName}' not found in type '{typeName}', you must include Namespace too.");
            }
            else
            {
                throw new ArgumentException($"Type ({typeName}) not found in assemblies marked with [DevAssembly].");
            }
        }
    }
}
