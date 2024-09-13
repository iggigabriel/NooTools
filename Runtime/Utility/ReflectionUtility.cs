using System;
using System.Linq;

namespace Noo.Tools
{
    public static class ReflectionUtility
    {

        public static string GetCsharpExecutableName(this Type type)
        {
            var generics = !type.IsGenericType ? string.Empty : string.Join(",", type.GetGenericArguments().Select(x => GetCsharpExecutableName(x)));

            return EmbedGenericParameters(type, generics);

            static string EmbedGenericParameters(Type type, string generics = null)
            {
                var name = type.Name.Contains("`") ? $"{type.Name[..type.Name.IndexOf("`")]}<{generics}>" : type.Name;
                if (type.DeclaringType == null) return name;
                return $"{EmbedGenericParameters(type.DeclaringType, generics)}.{name}";
            }
        }

    }
}
