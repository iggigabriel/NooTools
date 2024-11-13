using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Noo.Tools
{
    public static class EnumExtensions
    {
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
        public static TEnum SetFlag<TEnum>(this TEnum enumValue, TEnum flag) where TEnum : unmanaged, Enum
        {
            var value = UnsafeUtility.EnumToInt(enumValue);
            var flagValue = UnsafeUtility.EnumToInt(flag);
            value |= flagValue;
            return UnsafeUtility.As<int, TEnum>(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum UnsetFlag<TEnum>(this TEnum enumValue, TEnum flag) where TEnum : unmanaged, Enum
        {
            var value = UnsafeUtility.EnumToInt(enumValue);
            var flagValue = UnsafeUtility.EnumToInt(flag);
            value &= ~flagValue;
            return UnsafeUtility.As<int, TEnum>(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum SetFlag<TEnum>(this TEnum enumValue, TEnum flag, bool isSet) where TEnum : unmanaged, Enum
        {
            return isSet ? enumValue.SetFlag(flag) : enumValue.UnsetFlag(flag);
        }
    }
}
