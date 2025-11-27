#if UNITY_EDITOR
using Noo.Tools;
using NUnit.Framework;
using System;
using UnityEngine;

namespace Noo.Tools.Editor
{
    // Unity can't call ReadOnlySpan<T>.ToArray() directly, so helper:
    static class SpanExtensions
    {
        public static T[] ToArrayFast<T>(this ReadOnlySpan<T> span)
        {
            var arr = new T[span.Length];
            span.CopyTo(arr);
            return arr;
        }
    }

    public class MultiValueHashMapTests
    {
        [Test]
        public void Count_EmptyDictionary_ShouldBeZero()
        {
            var d = new MultiValueHashMap<string, int>();
            Assert.AreEqual(0, d.Count);
        }

        [Test]
        public void Count_Increases_OnAdd()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 1);   // +1
            d.Add("A", 2);   // +1
            d.Add("B", 10);  // +1

            Assert.AreEqual(3, d.Count);
        }

        [Test]
        public void Count_DoesNotIncrease_OnDuplicateAdd()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 5);    // +1
            d.Add("A", 5);    // duplicate, no change

            Assert.AreEqual(1, d.Count);
        }

        [Test]
        public void Count_Decreases_OnRemoveKey()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 1);
            d.Add("A", 2);

            Assert.AreEqual(2, d.Count);

            d.Remove("A");     // removes both values

            Assert.AreEqual(0, d.Count);
        }

        [Test]
        public void Count_Decreases_OnRemoveSpecificValue()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 1);
            d.Add("A", 2);

            Assert.AreEqual(2, d.Count);

            d.Remove("A", 2);

            Assert.AreEqual(1, d.Count);
        }

        [Test]
        public void Count_ResetToZero_OnClear()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 1);
            d.Add("B", 2);

            d.Clear();

            Assert.AreEqual(0, d.Count);
        }

        // ------------------------------------------------------------
        // Capacity
        // ------------------------------------------------------------

        [Test]
        public void Capacity_StartsAtZero()
        {
            var d = new MultiValueHashMap<string, int>();
            Assert.AreEqual(0, d.Capacity);
        }

        [Test]
        public void Capacity_Increases_AfterAdd()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 1);

            Assert.Greater(d.Capacity, 0);
        }

        [Test]
        public void Capacity_Grows_OnExpansion()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 1);
            var initial = d.Capacity;

            // Force growth in maxValues or keyCount
            d.Add("A", 2);
            d.Add("A", 3);
            d.Add("A", 4);
            d.Add("B", 10);

            Assert.Greater(d.Capacity, initial);
        }

        [Test]
        public void Capacity_DoesNotShrink_OnRemove()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 1);
            var initial = d.Capacity;

            d.Remove("A");

            Assert.AreEqual(initial, d.Capacity);
        }

        [Test]
        public void Capacity_DoesNotShrink_OnClear()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 1);
            var initial = d.Capacity;

            d.Clear();

            Assert.AreEqual(initial, d.Capacity);
        }

        // ------------------------------------------------------------
        // Indexer
        // ------------------------------------------------------------

        [Test]
        public void Indexer_ReturnsValues_ForExistingKey()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 3);
            d.Add("A", 5);

            var span = d["A"];

            CollectionAssert.AreEqual(new[] { 3, 5 }, span.ToArrayFast());
        }

        [Test]
        public void Indexer_ReturnsEmptySpan_ForMissingKey()
        {
            var d = new MultiValueHashMap<string, int>();

            var span = d["NoKey"];

            Assert.AreEqual(0, span.Length);
        }

        [Test]
        public void Indexer_ReflectsUpdatesAfterRemove()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 1);
            d.Add("A", 2);

            d.Remove("A", 1);

            CollectionAssert.AreEqual(new[] { 2 }, d["A"].ToArrayFast());
        }

        [Test]
        public void Indexer_EmptyAfterKeyRemoval()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 7);
            d.Remove("A");

            Assert.AreEqual(0, d["A"].Length);
        }


        [Test]
        public void Add_FirstValue_ShouldReturnTrue_AndContainKey()
        {
            var d = new MultiValueHashMap<string, int>();

            var added = d.Add("A", 10);

            Assert.IsTrue(added);
            Assert.IsTrue(d.Contains("A"));
            CollectionAssert.AreEqual(new[] { 10 }, d.GetValues("A").ToArrayFast());
        }

        [Test]
        public void Add_DuplicateValue_ShouldReturnFalse_AndNotDuplicate()
        {
            var d = new MultiValueHashMap<string, int>();

            Assert.IsTrue(d.Add("A", 10));
            Assert.IsFalse(d.Add("A", 10));

            CollectionAssert.AreEqual(new[] { 10 }, d.GetValues("A").ToArrayFast());
        }

        [Test]
        public void Add_MultipleValues_ShouldExpand()
        {
            var d = new MultiValueHashMap<string, int>();

            Assert.IsTrue(d.Add("A", 1));
            Assert.IsTrue(d.Add("A", 2));
            Assert.IsTrue(d.Add("A", 3));

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, d.GetValues("A").ToArrayFast());
        }

        [Test]
        public void Add_MultipleKeys_ShouldAssignSeparateLists()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 1);
            d.Add("B", 2);
            d.Add("C", 3);

            CollectionAssert.AreEqual(new[] { 1 }, d.GetValues("A").ToArrayFast());
            CollectionAssert.AreEqual(new[] { 2 }, d.GetValues("B").ToArrayFast());
            CollectionAssert.AreEqual(new[] { 3 }, d.GetValues("C").ToArrayFast());
        }

        [Test]
        public void Add_ReusesFreeIndexAfterRemoval()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 1);
            d.Remove("A");

            d.Add("B", 2);

            CollectionAssert.AreEqual(new[] { 2 }, d.GetValues("B").ToArrayFast());
            Assert.IsFalse(d.Contains("A"));
        }

        [Test]
        public void Remove_KeyOnly_RemovesAllValues()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 1);
            d.Add("A", 2);

            Assert.IsTrue(d.Remove("A"));

            Assert.IsFalse(d.Contains("A"));
            Assert.AreEqual(0, d.GetValues("A").Length);
        }

        [Test]
        public void Remove_KeyOnly_ReturnsFalseIfKeyMissing()
        {
            var d = new MultiValueHashMap<string, int>();
            Assert.IsFalse(d.Remove("A"));
        }

        [Test]
        public void Remove_SpecificValue_ShouldRemoveOnlyOne()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 1);
            d.Add("A", 2);
            d.Add("A", 3);

            Assert.IsTrue(d.Remove("A", 2));

            CollectionAssert.AreEqual(new[] { 1, 3 }, d.GetValues("A").ToArrayFast());
        }

        [Test]
        public void Remove_SpecificValue_LastValue_RemovesKeyCompletely()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 10);
            Assert.IsTrue(d.Remove("A", 10));

            Assert.IsFalse(d.Contains("A"));
            Assert.AreEqual(0, d.GetValues("A").Length);
        }

        [Test]
        public void Remove_SpecificValue_ReturnsFalseIfValueMissing()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 1);

            Assert.IsFalse(d.Remove("A", 999));
            CollectionAssert.AreEqual(new[] { 1 }, d.GetValues("A").ToArrayFast());
        }

        [Test]
        public void GetValues_ReturnsEmptyWhenKeyMissing()
        {
            var d = new MultiValueHashMap<string, int>();
            Assert.AreEqual(0, d.GetValues("X").Length);
        }

        [Test]
        public void Contains_KeyOnly()
        {
            var d = new MultiValueHashMap<string, int>();

            Assert.IsFalse(d.Contains("A"));
            d.Add("A", 5);
            Assert.IsTrue(d.Contains("A"));
        }

        [Test]
        public void Contains_KeyValue()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 5);

            Assert.IsTrue(d.Contains("A", 5));
            Assert.IsFalse(d.Contains("A", 999));
            Assert.IsFalse(d.Contains("B", 5));
        }

        [Test]
        public void Clear_RemovesAllData()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 1);
            d.Add("A", 2);
            d.Add("B", 3);

            d.Clear();

            Assert.IsFalse(d.Contains("A"));
            Assert.IsFalse(d.Contains("B"));
            Assert.AreEqual(0, d.GetValues("A").Length);
            Assert.AreEqual(0, d.GetValues("B").Length);
        }

        [Test]
        public void Keys_ReturnsAllUsedKeys()
        {
            var d = new MultiValueHashMap<string, int>();

            d.Add("A", 1);
            d.Add("B", 2);

            CollectionAssert.AreEquivalent(new[] { "A", "B" }, d.Keys);
        }
    }
}
#endif