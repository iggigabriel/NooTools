using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Noo.DevToolkit
{
    [DevCommands("Hidden/Example Properties", GenerateMemberFlags = BindingFlags.Static | BindingFlags.Public)]
    internal static class ExampleDrawers
    {
        public enum ExampleEnum
        {
            Cat, Dog, Bunny, Fish, Crow
        }

        [Flags]
        public enum ExampleEnumFlag
        {
            Monday = 1 << 0,
            Tuesday = 1 << 1,
            Wednesday = 1 << 2,
            Thursday = 1 << 3,
            Friday = 1 << 4,
            Saturday = 1 << 5,
            Sunday = 1 << 6,
        }

        public static byte Byte { get; set; }
        public static sbyte SByte { get; set; }
        public static short Int16 { get; set; }
        public static ushort UInt16 { get; set; }
        public static int Int32 { get; set; }
        public static uint UInt32 { get; set; }
        public static long Int64 { get; set; }
        public static ulong UInt64 { get; set; }
        public static float Single { get; set; }
        public static double Double { get; set; }
        public static string String { get; set; }
        public static char Char { get; set; }
        public static bool Bool { get; set; }
        public static Vector2 Vector2 { get; set; }
        public static Vector3 Vector3 { get; set; }
        public static Vector4 Vector4 { get; set; }
        public static Vector2Int Vector2Int { get; set; }
        public static Vector3Int Vector3Int { get; set; }

        public static ExampleEnum Enum { get; set; }
        public static ExampleEnumFlag EnumFlags { get; set; }

        [DevSlider(0, 10)]
        public static int IntSlider { get; set; }

        [DevSlider(0f, 10f)]
        public static float FloatSlider { get; set; }

        [DevDropdown(nameof(ExampleData))]
        public static int IntDropdown { get; set; }

        static Dictionary<object, string> ExampleData { get; } = new Dictionary<object, string>()
        {
            { 0, "-" },
            { 1, "One" },
            { 2, "Two" },
            { 3, "Three" },
            { 4, "Four" },
        };

        public static void ExampleMethod()
        {
            Debug.Log("Hello from Example Method");
        }

        public static void ExampleMethodWithParams(int number, ExampleEnumFlag days)
        {
            Debug.Log($"Number: {number}, Days: {days}");
        }
    }

    [DevCommands("Hidden/UnityEngine.Time", GenerateMemberFlags = BindingFlags.Static | BindingFlags.Public)]
    internal class ExampleUnityDrawers
    {
        [DevCommand("Time (String)")]
        public static string TimeAsString => TimeSpan.FromSeconds(Time).ToString("c");

        public static float Time => UnityEngine.Time.time;
        public static float UnscaledTime => UnityEngine.Time.unscaledTime;
        public static int FrameCount => UnityEngine.Time.frameCount;

        public static float AvgFPS => FrameCount / UnityEngine.Time.realtimeSinceStartup;

        public static float TimeScale
        {
            get => UnityEngine.Time.timeScale;
            set => UnityEngine.Time.timeScale = value;
        }
    }
}