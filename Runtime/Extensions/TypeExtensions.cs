using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Noo.Tools
{
    public static class TypeExtensions
    {
        static readonly Dictionary<Type, string> typeNames = new();
        static readonly Dictionary<Type, string> typeNamesSnakeCase = new();

        public static string GetNameNonAlloc(this Type type)
        {
            if (!typeNames.TryGetValue(type, out var name))
            {
                name = type.Name;
                typeNames.Add(type, name);
            }

            return name;
        }

        public static string GetNameSnakeCase(this Type type)
        {
            if (typeNamesSnakeCase.TryGetValue(type, out var cached))
                return cached;

            var name = type.Name;
            var sb = new System.Text.StringBuilder(name.Length + 4);

            sb.Append(char.ToLower(name[0]));
            for (int i = 1; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]))
                    sb.Append('_');
                sb.Append(char.ToLower(name[i]));
            }

            var result = sb.ToString();
            typeNamesSnakeCase[type] = result;
            return result;
        }
    }
}
