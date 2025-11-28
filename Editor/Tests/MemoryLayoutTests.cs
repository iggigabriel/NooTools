using Noo.Tools;
using NUnit.Framework;
using System.Linq;
using UnityEngine;

namespace Noo.Tools.Editor
{
    public class MemoryLayoutTests
    {
        [Test]
        public void Constructor_SetsInitialCapacityAndSingleSlice()
        {
            var mem = new MemoryLayout(100);

            Assert.AreEqual(100, mem.Capacity);

            var slices = mem.Slices.ToList();
            Assert.AreEqual(1, slices.Count);
            Assert.AreEqual(0, slices[0].Start);
            Assert.AreEqual(100, slices[0].Length);
        }

        [Test]
        public void Slice_Equality_Works()
        {
            var a = new MemoryLayout.Slice(10, 20);
            var b = new MemoryLayout.Slice(10, 20);
            var c = new MemoryLayout.Slice(15, 20);

            Assert.IsTrue(a == b);
            Assert.IsFalse(a == c);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Allocate_ExactMatch_RemovesSlice()
        {
            var mem = new MemoryLayout(50);

            var s = mem.Allocate(50);

            Assert.AreEqual(0, s.Start);
            Assert.AreEqual(50, s.Length);

            Assert.AreEqual(0, mem.Slices.ToList().Count);
        }

        [Test]
        public void Allocate_SplitSlice_CreatesRemainder()
        {
            var mem = new MemoryLayout(100);

            var a = mem.Allocate(30);

            Assert.AreEqual(0, a.Start);
            Assert.AreEqual(30, a.Length);

            var slices = mem.Slices.ToList();

            Assert.AreEqual(1, slices.Count);
            Assert.AreEqual(30, slices[0].Start);
            Assert.AreEqual(70, slices[0].Length);
        }

        [Test]
        public void Allocate_GrowsLastSliceAndCapacity()
        {
            var mem = new MemoryLayout(10);

            var a = mem.Allocate(10); // uses whole slice → slices empty

            Assert.AreEqual(0, mem.Slices.ToList().Count);

            var b = mem.Allocate(20); // should grow capacity

            Assert.AreEqual(10, b.Start);
            Assert.AreEqual(20, b.Length);
            Assert.AreEqual(30, mem.Capacity);
            Assert.AreEqual(0, mem.Slices.ToList().Count);
        }

        [Test]
        public void Allocate_NewSliceBeyondEnd()
        {
            var mem = new MemoryLayout(10);

            mem.Allocate(5); // leaves slice [5..10]

            // remove everything by allocating 5 more
            mem.Allocate(5); // slices empty now

            var extra = mem.Allocate(7);

            Assert.AreEqual(10, extra.Start);
            Assert.AreEqual(7, extra.Length);
            Assert.AreEqual(17, mem.Capacity);
        }

        [Test]
        public void Deallocate_NoMerge_AddsSlice()
        {
            var mem = new MemoryLayout(50);
            var s = mem.Allocate(20);

            mem.Deallocate(s);

            Assert.AreEqual(1, mem.Slices.ToList().Count);
            Assert.AreEqual(50, mem.Slices.ToList()[0].Length);
        }

        [Test]
        public void Deallocate_MergesRight()
        {
            var mem = new MemoryLayout(50);

            var a = mem.Allocate(20); // slice = [0..20]
            var b = mem.Allocate(10); // slice = [20..30]

            // free [20..30]
            mem.Deallocate(b);

            // free [0..20] now merges with [20..30]
            mem.Deallocate(a);

            var slices = mem.Slices.ToList();

            Assert.AreEqual(1, slices.Count);
            Assert.AreEqual(0, slices[0].Start);
            Assert.AreEqual(50, slices[0].Length);
        }

        [Test]
        public void Deallocate_MergesLeft()
        {
            var mem = new MemoryLayout(100);

            var a = mem.Allocate(30);    // [0..30]
            var b = mem.Allocate(20);    // [30..50]

            // free left later merges backward
            mem.Deallocate(a);
            mem.Deallocate(b);

            var slices = mem.Slices.ToList();
            Assert.AreEqual(1, slices.Count);
            Assert.AreEqual(100, slices[0].Length);
        }

        [Test]
        public void Deallocate_MergesBothSides()
        {
            var mem = new MemoryLayout(100);

            var a = mem.Allocate(20);   // [0..20]
            var b = mem.Allocate(20);   // [20..40]
            var c = mem.Allocate(20);   // [40..60]

            mem.Deallocate(a);
            mem.Deallocate(c);

            // free b should merge (a,b,c)
            mem.Deallocate(b);

            var slices = mem.Slices.ToList();
            Assert.AreEqual(1, slices.Count);
            Assert.AreEqual(100, slices[0].Length);
        }

        [Test]
        public void Clear_ResetsToSingleFullSlice()
        {
            var mem = new MemoryLayout(60);
            mem.Allocate(20);
            mem.Allocate(10);
            mem.Allocate(30);

            mem.Clear();

            var slices = mem.Slices.ToList();
            Assert.AreEqual(1, slices.Count);
            Assert.AreEqual(60, slices[0].Length);
        }

        [Test]
        public void Enumeration_ReturnsAllSlices()
        {
            var mem = new MemoryLayout(100);

            mem.Allocate(20);
            mem.Allocate(10);

            var list = mem.Slices.ToList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(70, list[0].Length);
        }

        [Test]
        public void ToString_ReportsCorrectValues()
        {
            var mem = new MemoryLayout(100);
            mem.Allocate(40);

            string s = mem.ToString();

            Assert.IsTrue(s.Contains("Capacity: 100"));
            Assert.IsTrue(s.Contains("Free: 60"));
            Assert.IsTrue(s.Contains("Partitions: 1"));
        }
    }
}
