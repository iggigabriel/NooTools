using System.Text.RegularExpressions;

namespace Noo.Tools
{
    public static class StringExtensions
    {
        public static string ToFirstLower(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return str[..1].ToLower() + str[1..];
        }

        public static string ToFirstUpper(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return str[..1].ToUpper() + str[1..];
        }

        public static string SplitCamelCase(this string str)
        {
            return Regex.Replace(
                Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"),
                @"(\p{Ll})(\P{Ll})", "$1 $2"
            );
        }

        public static string Wrap(this string str, string prefix, string suffix)
        {
            return $"{prefix}{str}{suffix}";
        }
    }
}
