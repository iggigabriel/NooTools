using System;
using System.Collections.Generic;
using UnityEngine;

namespace Noo.Tools
{
    public static class LinqExtensions
    {
        public static int IndexOf<TSource>(this IEnumerable<TSource> source, TSource item)
        {
            var index = 0;

            foreach (var srcItem in source)
            {
                if (srcItem.Equals(item)) return index;
                index++;
            }

            return -1;
        }

        public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var index = 0;

            foreach (var item in source)
            {
                if (predicate.Invoke(item)) return index;
                index++;
            }

            return -1;
        }
    }
}
