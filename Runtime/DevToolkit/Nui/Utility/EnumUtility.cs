using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Noo.Nui
{
    internal static class EnumUtility
    {
        readonly static Dictionary<Type, string[]> enumNames = new();
        readonly static Dictionary<Type, object> enumValues = new();
        readonly static Dictionary<Type, object> enumNameValue = new();
        readonly static Dictionary<Type, object> enumValueName = new();

        readonly static Dictionary<Type, object> genericEnumValues = new();
        readonly static Dictionary<Type, object> genericEnumNameValue = new();
        readonly static Dictionary<Type, object> genericEnumValueName = new();

        public static T[] GetValues<T>() where T : unmanaged, Enum
        {
            var type = typeof(T);

            if (!enumValues.TryGetValue(type, out var values) || values is not T[] tValues)
            {
                var rawValues = Enum.GetValues(type);
                enumValues[type] = tValues = new T[rawValues.Length];
                Array.Copy(rawValues, tValues, rawValues.Length);
            }

            return tValues;
        }

        public static string[] GetNames<T>() where T : unmanaged, Enum
        {
            var type = typeof(T);

            if (!enumNames.TryGetValue(type, out var values))
            {
                enumNames[type] = values = Enum.GetNames(type);
            }

            return values;
        }

        public static int[] GetValues(Type type)
        {
            if (!genericEnumValues.TryGetValue(type, out var values) || values is not int[] tValues)
            {
                var rawValues = Enum.GetValues(type);
                genericEnumValues[type] = tValues = new int[rawValues.Length];

                for (int i = 0; i < rawValues.Length; i++)
                {
                    tValues[i] = Convert.ToInt32(rawValues.GetValue(i));
                }
            }

            return tValues;
        }

        public static string[] GetNames(Type type)
        {
            if (!enumNames.TryGetValue(type, out var values))
            {
                enumNames[type] = values = Enum.GetNames(type);
            }

            return values;
        }

        public static IReadOnlyDictionary<string, T> GetNameValuePairs<T>() where T : unmanaged, Enum
        {
            var type = typeof(T);

            if (!enumNameValue.TryGetValue(type, out var dict) || dict is not Dictionary<string, T> tDict)
            {
                var names = GetNames<T>();
                var values = GetValues<T>();
                enumNameValue[type] = tDict = new Dictionary<string, T>();
                for (int i = 0; i < names.Length; i++) tDict[names[i]] = values[i];
            }

            return tDict;
        }

        public static IReadOnlyDictionary<int, string> GetValueNamePairs(Type type)
        {
            if (!genericEnumValueName.TryGetValue(type, out var dict) || dict is not Dictionary<int, string> tDict)
            {
                var names = GetNames(type);
                var values = GetValues(type);
                enumValueName[type] = tDict = new Dictionary<int, string>();
                for (int i = 0; i < names.Length; i++) tDict[Convert.ToInt32(values[i])] = names[i];
            }

            return tDict;
        }

        public static IReadOnlyDictionary<T, string> GetValueNamePairs<T>() where T : unmanaged, Enum
        {
            var type = typeof(T);

            if (!enumValueName.TryGetValue(type, out var dict) || dict is not Dictionary<T, string> tDict)
            {
                var names = GetNames<T>();
                var values = GetValues<T>();
                enumValueName[type] = tDict = new Dictionary<T, string>();
                for (int i = 0; i < names.Length; i++) tDict[values[i]] = names[i];
            }

            return tDict;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt32NonAlloc<TEnum>(this TEnum enumValue) where TEnum : unmanaged, Enum
        {
            var value = UnsafeUtility.EnumToInt(enumValue);
            return value;
        }

        /// <summary>
        /// Works only for INT type enums
        /// </summary>
        /// <returns>(this & flag) == flag</returns>
        public static bool HasFlagNonAlloc<TEnum>(this TEnum value, TEnum flag) where TEnum : unmanaged, Enum
        {
            var a = UnsafeUtility.EnumToInt(value);
            var b = UnsafeUtility.EnumToInt(flag);

            return (a & b) == b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Set<TEnum>(this TEnum enumValue, TEnum flag) where TEnum : unmanaged, Enum
        {
            var value = UnsafeUtility.EnumToInt(enumValue);
            var flagValue = UnsafeUtility.EnumToInt(flag);
            value |= flagValue;
            return UnsafeUtility.As<int, TEnum>(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Unset<TEnum>(this TEnum enumValue, TEnum flag) where TEnum : unmanaged, Enum
        {
            var value = UnsafeUtility.EnumToInt(enumValue);
            var flagValue = UnsafeUtility.EnumToInt(flag);
            value &= ~flagValue;
            return UnsafeUtility.As<int, TEnum>(ref value);
        }
    }
}
