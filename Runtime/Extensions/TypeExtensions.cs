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
            if (!typeNamesSnakeCase.TryGetValue(type, out var name))
            {
                name = ToSnakeCase(type.Name);
                typeNamesSnakeCase.Add(type, name);
            }

            return name;
        }

        private static string ToSnakeCase(string name)
        {
            return Regex.Replace(name, @"(?<!^)[A-Z]", "_$0").ToLower();
        }
    }
}
