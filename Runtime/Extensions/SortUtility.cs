using System;
using System.Collections.Generic;

namespace Noo.Tools
{
    /// <summary>
    /// Unities old BCL is still allocating on every List.Sort() and Array.Sort(),
    /// thats why we will implement custom non-allocating sorts
    /// </summary>
    public static class SortUtility
    {
        /// <summary>
        /// Sorts an array in-place using QuickSort with a custom comparer.
        /// Completely allocation-free.
        /// </summary>
        public static void QuickSort<T, TComparer>(this List<T> list, TComparer comparer = default) where TComparer : IComparer<T>
        {
            if (list == null || list.Count <= 1) return;
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            QuickSort<T, List<T>, TComparer>(list, 0, list.Count - 1, comparer);
        }

        /// <summary>
        /// Sorts an array in-place using QuickSort with a custom comparer.
        /// Completely allocation-free.
        /// </summary>
        public static void QuickSort<T, TComparer>(this T[] array, TComparer comparer = default) where TComparer : IComparer<T>
        {
            if (array == null || array.Length <= 1) return;
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            QuickSort<T, T[], TComparer>(array, 0, array.Length - 1, comparer);
        }

        /// <summary>
        /// Sorts an array in-place using QuickSort with a custom comparer.
        /// Completely allocation-free.
        /// </summary>
        public static void QuickSort<T>(this List<T> list, Comparison<T> comparer)
        {
            if (list == null || list.Count <= 1) return;
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            QuickSort(list, 0, list.Count - 1, comparer);
        }

        /// <summary>
        /// Sorts an array in-place using QuickSort with a custom comparer.
        /// Completely allocation-free.
        /// </summary>
        public static void QuickSort<T>(this T[] array, Comparison<T> comparer)
        {
            if (array == null || array.Length <= 1) return;
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            QuickSort(array, 0, array.Length - 1, comparer);
        }

        private static void QuickSort<T, TList, TComparer>(TList array, int left, int right, TComparer comparer)
            where TComparer : IComparer<T>
            where TList : IList<T>
        {
            int i = left;
            int j = right;
            T pivot = array[(left + right) >> 1];

            while (i <= j)
            {
                while (comparer.Compare(array[i], pivot) < 0) i++;
                while (comparer.Compare(array[j], pivot) > 0) j--;

                if (i <= j)
                {
                    (array[j], array[i]) = (array[i], array[j]);
                    i++;
                    j--;
                }
            }

            if (left < j) QuickSort<T, TList, TComparer>(array, left, j, comparer);
            if (i < right) QuickSort<T, TList, TComparer>(array, i, right, comparer);
        }

        private static void QuickSort<T, TList>(TList array, int left, int right, Comparison<T> comparer)
            where TList : IList<T>
        {
            int i = left;
            int j = right;
            T pivot = array[(left + right) >> 1];

            while (i <= j)
            {
                while (comparer(array[i], pivot) < 0) i++;
                while (comparer(array[j], pivot) > 0) j--;

                if (i <= j)
                {
                    (array[j], array[i]) = (array[i], array[j]);
                    i++;
                    j--;
                }
            }

            if (left < j) QuickSort(array, left, j, comparer);
            if (i < right) QuickSort(array, i, right, comparer);
        }

        public static void SortBy<TIn, TOut>(this List<TIn> list, Func<TIn, TOut> getter)
        {
            list.QuickSort(new AscSort<TIn, TOut>(getter));
        }

        public static void SortByDescending<TIn, TOut>(this List<TIn> list, Func<TIn, TOut> getter)
        {
            list.QuickSort(new DescSort<TIn, TOut>(getter));
        }

        readonly struct AscSort<TIn, TOut> : IComparer<TIn>
        {
            readonly Func<TIn, TOut> getter;
            public AscSort(Func<TIn, TOut> getter) => this.getter = getter;
            public int Compare(TIn x, TIn y)
            {
                if (getter(x) is IComparable<TOut> xOut && getter(y) is TOut yOut)
                {
                    return xOut.CompareTo(yOut);
                }

                return 0;
            }
        }

        readonly struct DescSort<TIn, TOut> : IComparer<TIn>
        {
            readonly Func<TIn, TOut> getter;
            public DescSort(Func<TIn, TOut> getter) => this.getter = getter;
            public int Compare(TIn x, TIn y)
            {
                if (getter(x) is IComparable<TOut> xOut && getter(y) is TOut yOut)
                {
                    return -xOut.CompareTo(yOut);
                }

                return 0;
            }
        }
    }
}
