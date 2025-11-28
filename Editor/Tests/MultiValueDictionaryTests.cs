#if UNITY_EDITOR
using Noo.Tools;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Noo.Tools.Editor
{
    public class MultiValueDictionaryTests
    {
        const int Seed = 123456;
        System.Random rng;

        MultiValueDictionary<string, int> map;

        [SetUp]
        public void Setup()
        {
            rng = new System.Random(Seed);
            map = new MultiValueDictionary<string, int>();
        }

        // ---------------------------------------------------------
        // Basic Operations
        // ---------------------------------------------------------

        [Test]
        public void Add_SingleKeyMultipleValues()
        {
            Assert.IsTrue(map.Add("A", 1));
            Assert.IsTrue(map.Add("A", 2));
            Assert.IsTrue(map.Add("A", 3));

            var values = map["A"];
            Assert.AreEqual(3, values.Count);
            CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, values);
        }

        [Test]
        public void Add_DuplicateValueRejected()
        {
            Assert.IsTrue(map.Add("A", 1));
            Assert.IsFalse(map.Add("A", 1));   // duplicate
        }

        [Test]
        public void Remove_KeyRemovesAllValues()
        {
            map.Add("A", 1);
            map.Add("A", 2);
            map.Add("A", 3);

            Assert.IsTrue(map.Remove("A"));
            Assert.IsFalse(map.Contains("A"));
            Assert.AreEqual(0, map.Count);
        }

        [Test]
        public void Remove_SpecificValue()
        {
            map.Add("A", 1);
            map.Add("A", 2);

            Assert.IsTrue(map.Remove("A", 1));
            Assert.IsTrue(map.Contains("A"));
            Assert.IsFalse(map.Contains("A", 1));
            Assert.IsTrue(map.Contains("A", 2));
            Assert.AreEqual(1, map.Count);
        }

        [Test]
        public void Clear_RemovesEverything()
        {
            map.Add("A", 1);
            map.Add("B", 2);
            map.Add("C", 3);

            map.Clear();

            Assert.AreEqual(0, map.Count);
            Assert.IsFalse(map.Contains("A"));
            Assert.IsFalse(map.Contains("B"));
            Assert.IsFalse(map.Contains("C"));
        }

        // ---------------------------------------------------------
        // KeyCollection enumeration
        // ---------------------------------------------------------

        [Test]
        public void Keys_EnumerateCorrectly()
        {
            map.Add("A", 1);
            map.Add("B", 2);
            map.Add("C", 3);

            var ks = new List<string>();
            foreach (var k in map.Keys)
                ks.Add(k);

            CollectionAssert.AreEquivalent(new[] { "A", "B", "C" }, ks);
        }

        [Test]
        public void Keys_ModifyingDuringEnumeration_Throws()
        {
            map.Add("A", 1);
            map.Add("B", 2);

            Assert.Throws<InvalidOperationException>(() =>
            {
                foreach (var k in map.Keys)
                {
                    map.Add("C", 3);   // mutation
                }
            });
        }

        // ---------------------------------------------------------
        // ValueCollection enumeration
        // ---------------------------------------------------------

        [Test]
        public void Values_EnumerateCorrectly()
        {
            map.Add("A", 10);
            map.Add("A", 20);

            var vs = new List<int>();
            foreach (var v in map["A"])
                vs.Add(v);

            CollectionAssert.AreEquivalent(new[] { 10, 20 }, vs);
        }

        [Test]
        public void Values_ModifyingDuringEnumeration_Throws()
        {
            map.Add("A", 1);
            map.Add("A", 2);

            Assert.Throws<InvalidOperationException>(() =>
            {
                foreach (var v in map["A"])
                {
                    map.Add("A", 3); // modify same key while iterating values
                }
            });
        }

        // ---------------------------------------------------------
        // Combined Enumerator (IEnumerable<KeyValuePair<TKey,TValue>>)
        // ---------------------------------------------------------

        [Test]
        public void Enumerator_EnumeratesAllKeyValuePairs()
        {
            map.Add("A", 1);
            map.Add("A", 2);
            map.Add("B", 3);

            var list = new List<KeyValuePair<string, int>>();
            foreach (var kv in map)
                list.Add(kv);

            CollectionAssert.AreEquivalent(
                new[]
                {
                new KeyValuePair<string,int>("A",1),
                new KeyValuePair<string,int>("A",2),
                new KeyValuePair<string,int>("B",3),
                },
                list
            );
        }

        [Test]
        public void Enumerator_ModifyingDuringEnumeration_Throws()
        {
            map.Add("A", 1);
            map.Add("B", 2);

            Assert.Throws<InvalidOperationException>(() =>
            {
                foreach (var kv in map)
                {
                    map.Add("C", 3); // mutation
                }
            });
        }

        [Test]
        public void Enumerator_Reset_Works()
        {
            map.Add("A", 1);
            map.Add("A", 2);

            var e = map.GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            e.Reset();
            Assert.IsTrue(e.MoveNext()); // should restart
        }

        // ---------------------------------------------------------
        // Memory expansion & relocation
        // ---------------------------------------------------------

        [Test]
        public void Add_ExpandsSlicesProperly()
        {
            for (int i = 0; i < 20; i++)
                map.Add("A", i);

            Assert.AreEqual(20, map["A"].Count);

            var s = new List<int>(map["A"]);
            CollectionAssert.AreEquivalent(
                System.Linq.Enumerable.Range(0, 20),
                s
            );
        }

        // ---------------------------------------------------------
        // Enumeration correctness after removals
        // ---------------------------------------------------------

        [Test]
        public void Values_EnumeratorAfterRemoval()
        {
            map.Add("A", 1);
            map.Add("A", 2);
            map.Remove("A", 1);

            var v = new List<int>(map["A"]);
            CollectionAssert.AreEquivalent(new[] { 2 }, v);
        }

        // ---------------------------------------------------------
        // Helper: Validate state matches a reference model
        // ---------------------------------------------------------

        void ValidateAgainstReference(int iteration, Dictionary<string, HashSet<int>> reference)
        {
            Assert.AreEqual(reference.Count, map.KeyCount);

            // Validate counts
            int refCount = 0;
            foreach (var kv in reference)
                refCount += kv.Value.Count;

            Assert.AreEqual(refCount, map.Count);

            // Validate keys
            foreach (var key in reference.Keys)
                Assert.IsTrue(map.Contains(key));

            // Validate values
            foreach (var kv in reference)
            {
                var values = new HashSet<int>(map[kv.Key]);
                CollectionAssert.AreEquivalent(kv.Value, values);
            }

            // Validate enumeration matches all key-value pairs
            var all = new HashSet<(string, int)>();
            foreach (var kv in reference)
                foreach (var v in kv.Value)
                    all.Add((kv.Key, v));

            var enumerated = new HashSet<(string, int)>();
            foreach (var kv in map)
                enumerated.Add((kv.Key, kv.Value));

            CollectionAssert.AreEquivalent(all, enumerated);
        }

        // ---------------------------------------------------------
        // Fuzz Test
        // ---------------------------------------------------------

        [Test]
        public void FuzzTest_RandomizedOperations()
        {
            var reference = new Dictionary<string, HashSet<int>>();
            string[] keys = { "A", "B", "C", "D", "E" };

            for (int i = 0; i < 5000; i++)
            {
                int op = rng.Next(3);
                string key = keys[rng.Next(keys.Length)];
                int value = rng.Next(0, 20);

                switch (op)
                {
                    case 0: // Add
                        map.Add(key, value);
                        if (!reference.TryGetValue(key, out var set0))
                            reference[key] = set0 = new HashSet<int>();
                        set0.Add(value);
                        break;

                    case 1: // Remove key
                        map.Remove(key);
                        reference.Remove(key);
                        break;

                    case 2: // Remove value
                        map.Remove(key, value);
                        if (reference.TryGetValue(key, out var set2))
                        {
                            set2.Remove(value);
                            if (set2.Count == 0)
                                reference.Remove(key);
                        }
                        break;
                }

                ValidateAgainstReference(i, reference);
            }
        }

        // ---------------------------------------------------------
        // High-Load Mixed Operations (10k ops)
        // ---------------------------------------------------------

        [Test]
        public void HighLoad_10kOperations()
        {
            var reference = new Dictionary<string, HashSet<int>>();
            string[] keys = { "K0", "K1", "K2", "K3", "K4", "K5", "K6", "K7" };

            for (int i = 0; i < 10000; i++)
            {
                int op = rng.Next(5);
                string key = keys[rng.Next(keys.Length)];
                int value = rng.Next(0, 50);

                if (op == 0)
                {
                    // Add
                    map.Add(key, value);
                    if (!reference.TryGetValue(key, out var set))
                        reference[key] = set = new HashSet<int>();
                    set.Add(value);
                }
                else if (op == 1)
                {
                    // Remove value
                    map.Remove(key, value);
                    if (reference.TryGetValue(key, out var set))
                    {
                        set.Remove(value);
                        if (set.Count == 0)
                            reference.Remove(key);
                    }
                }
                else if (op == 2)
                {
                    // Remove whole key
                    map.Remove(key);
                    reference.Remove(key);
                }
                else if (op == 3)
                {
                    // Enumerate keys
                    foreach (var k in map.Keys)
                        Assert.IsTrue(reference.ContainsKey(k));
                }
                else if (op == 4)
                {
                    // Enumerate full map
                    var seen = new HashSet<(string, int)>();
                    foreach (var kv in map)
                        seen.Add((kv.Key, kv.Value));

                    var expected = new HashSet<(string, int)>();
                    foreach (var kv in reference)
                        foreach (var v in kv.Value)
                            expected.Add((kv.Key, v));

                    CollectionAssert.AreEquivalent(expected, seen);
                }

                ValidateAgainstReference(i, reference);
            }
        }

        // ---------------------------------------------------------
        // 1. Version-safety fuzzing (mutation while enumerating)
        // ---------------------------------------------------------

        [Test]
        public void VersionSafety_Fuzz_ModifyDuringKeyEnumeration_Throws()
        {
            for (int i = 0; i < 50; i++)
                map.Add("K" + (i % 5), i);

            for (int trial = 0; trial < 200; trial++)
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    foreach (var k in map.Keys)
                    {
                        map.Add("Mut" + trial, trial);
                    }
                });
            }
        }

        [Test]
        public void VersionSafety_Fuzz_ModifyDuringValueEnumeration_Throws()
        {
            map.Add("A", 1);
            map.Add("A", 2);
            map.Add("A", 3);

            for (int trial = 0; trial < 200; trial++)
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    foreach (var v in map["A"])
                    {
                        map.Add("A", 99 + trial);
                    }
                });
            }
        }

        [Test]
        public void VersionSafety_Fuzz_ModifyDuringKVPEnumeration_Throws()
        {
            map.Add("A", 5);
            map.Add("B", 6);

            for (int trial = 0; trial < 200; trial++)
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    foreach (var kv in map)
                    {
                        map.Add("C" + trial, trial);
                    }
                });
            }
        }

        // ---------------------------------------------------------
        // 2. Enumerator-Mutation Stress Tests
        // ---------------------------------------------------------

        [Test]
        public void EnumeratorStress_MutateBetweenEnumerations_NotDuring()
        {
            for (int i = 0; i < 1000; i++)
                map.Add("A", i);

            for (int i = 0; i < 100; i++)
            {
                foreach (var kv in map)
                {
                }

                map.Add("A", 1000 + i);
                map.Remove("A", i);
            }

            Assert.AreEqual(1000, map.Count);
        }

        // ---------------------------------------------------------
        // 3. Editor-Friendly Performance Benchmarks
        // ---------------------------------------------------------
        //
        // These are not strict timing tests. They verify that calls complete
        // quickly and without allocator errors. They do NOT assert timing thresholds.
        //

        [UnityTest]
        public IEnumerator Performance_Add_10k()
        {
            for (int i = 0; i < 10000; i++)
                map.Add("A", i);

            yield return null;
        }

        [UnityTest]
        public IEnumerator Performance_Mixed_10k()
        {
            var keys = new[] { "A", "B", "C", "D" };

            for (int i = 0; i < 10000; i++)
            {
                int op = rng.Next(3);
                var key = keys[rng.Next(keys.Length)];

                if (op == 0)
                {
                    map.Add(key, rng.Next(2000));
                }
                else if (op == 1)
                {
                    map.Remove(key, rng.Next(2000));
                }
                else
                {
                    foreach (var kv in map)
                    {
                    }
                }

                if (i % 20000 == 0)
                    yield return null;
            }
            yield return null;
        }

        // ---------------------------------------------------------
        // 4. Extreme Allocation Pattern Tests
        // ---------------------------------------------------------

        [Test]
        public void Extreme_ManyKeys_FewValuesEach()
        {
            const int keys = 5000;

            for (int i = 0; i < keys; i++)
            {
                map.Add("K" + i, 1);
            }

            Assert.AreEqual(keys, map.Count);
            Assert.AreEqual(keys, map.Keys.Count);

            foreach (var key in map.Keys)
                Assert.AreEqual(1, map[key].Count);
        }

        [Test]
        public void Extreme_FewKeys_ManyValues()
        {
            const int values = 5000;

            for (int i = 0; i < values; i++)
            {
                map.Add("A", i);
            }

            Assert.AreEqual(values, map["A"].Count);
            Assert.AreEqual(values, map.Count);

            int sum = 0;
            foreach (var v in map["A"])
                sum += v;

            Assert.AreEqual((values - 1) * values / 2, sum);
        }

        [Test]
        public void Extreme_MassiveChurn_AddRemoveSameKeysRepeatedly()
        {
            for (int cycle = 0; cycle < 500; cycle++)
            {
                for (int i = 0; i < 200; i++)
                    map.Add("X", i);

                for (int i = 0; i < 200; i++)
                    map.Remove("X", i);

                Assert.IsFalse(map.Contains("X"));
                Assert.AreEqual(0, map.Count);
            }
        }

        [Test]
        public void Extreme_HeavyEnumerationUnderChurn()
        {
            string[] K = { "K0", "K1", "K2", "K3" };

            for (int iter = 0; iter < 2000; iter++)
            {
                string key = K[rng.Next(K.Length)];
                int value = rng.Next(100);

                if (rng.NextDouble() < 0.5)
                    map.Add(key, value);
                else
                    map.Remove(key, value);

                foreach (var kv in map)
                {
                }
            }
        }

        // ---------------------------------------------------------
        // 5. Version-safety hard test: mutate always AFTER reading enumerator.Current
        // ---------------------------------------------------------

        [Test]
        public void VersionSafety_ModifyImmediatelyAfterEnumerateStep_AlwaysThrows()
        {
            map.Add("A", 1);
            map.Add("A", 2);
            map.Add("B", 3);

            Assert.Throws<InvalidOperationException>(() =>
            {
                var e = map.GetEnumerator();
                while (e.MoveNext())
                {
                    var current = e.Current;
                    map.Add("M", 5); // modify immediately after reading
                }
            });
        }
    }
}
#endif