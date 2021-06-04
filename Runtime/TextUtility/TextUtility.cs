using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nootools
{
    public static class TextUtility
    {
        /// <param name="seconds"></param>
        /// <param name="format"><see href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings"/></param>
        public static string GetTimeDisplay(double seconds, string format = @"mm\:ss")
        {
            return TimeSpan.FromSeconds(seconds).ToString(format);
        }
    }
}
