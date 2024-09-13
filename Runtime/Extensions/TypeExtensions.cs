using System;
using System.Collections.Generic;

namespace Noo.Tools
{
    public static class TypeExtensions
    {
        static readonly Dictionary<Type, string> typeNames = new();

        public static string GetNameNonAlloc(this Type type) 
        {
            if (!typeNames.TryGetValue(type, out var name))
            {
                name = type.Name;
                typeNames.Add(type, name);
            }

            return name;
        }
    }
}
