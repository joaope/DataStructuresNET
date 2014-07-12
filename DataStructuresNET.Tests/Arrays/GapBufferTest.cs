#region Copyright © 2014, João Correia
//
// Copyright © 2014, João Correia
// All rights reserved
// http://joaope.github.com
//
#endregion

using System;
using System.Linq;
using DataStructuresNET.Arrays;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataStructuresNET.Tests.Arrays
{
    [TestClass]
    public class GapBufferTests
    {
        [TestMethod]
        public void GapBuffer_Enumerator()
        {
            var buffer = new GapBuffer<int>(2, 2);
            buffer.AddRange(new[] { 1, 2, 3 }, 0, 6);
            var col = buffer.Select((n, index) => new { Number = n, Index = index }).ToArray();

            Assert.AreEqual(0, col[0].Index);
            Assert.AreEqual(1, col[1].Index);
            Assert.AreEqual(2, col[2].Index);
            Assert.AreEqual(1, col[0].Number);
            Assert.AreEqual(2, col[1].Number);
            Assert.AreEqual(3, col[2].Number);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException), AllowDerivedTypes = false)]
        public void GapBuffer_Enumerator_IndexOutOfRangeException() 
        {
            var buffer = new GapBuffer<int>(2, 2);
            buffer.AddRange(new[] { 1, 2, 3 }, 0, 6);
            var col = buffer.Select((n, index) => new { Number = n, Index = index }).ToArray();
            col[4] = null;
        }

        [TestMethod]
        public void GapBuffer_Ctor()
        {
            var buffer = new GapBuffer<bool>(10, 20);

            Assert.AreEqual(10, buffer.GapSize);
            Assert.AreEqual(20, buffer.Capacity);
            Assert.AreEqual(10, buffer.CurrentGapSize);
            Assert.AreEqual(0, buffer.Count);
            Assert.AreEqual(10, buffer.CountWithGap);
            Assert.AreEqual(0, buffer.GapStart);
            CollectionAssert.AreEqual(new bool[] {}, buffer.ToArray());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void GapBuffer_Ctor_ExceptionGapSize()
        {
            var buffer = new GapBuffer<bool>(-1, 10);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void GapBuffer_Ctor_CapacityLessThanZeroException()
        {
            var buffer = new GapBuffer<bool>(10, -10);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void GapBuffer_Ctor_CapacityLowerThanGapException()
        {
            var buffer = new GapBuffer<bool>(10, 5);
        }

        [TestMethod]
        public void GapBuffer_GapStart()
        {
            var data = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var buffer = new GapBuffer<int>(3, 3);
            buffer.AddRange(data, 0, 8);

            Assert.AreEqual(0, buffer.GapStart);
            Assert.AreEqual(3, buffer.CurrentGapSize);
            CollectionAssert.AreEqual(data, buffer.ToArray());

            buffer.GapStart = 5;

            Assert.AreEqual(5, buffer.GapStart);
            Assert.AreEqual(3, buffer.CurrentGapSize);
            CollectionAssert.AreEqual(data, buffer.ToArray());

            buffer.GapStart = 2;

            Assert.AreEqual(2, buffer.GapStart);
            Assert.AreEqual(3, buffer.CurrentGapSize);
            CollectionAssert.AreEqual(data, buffer.ToArray());
        }

        [TestMethod]
        public void GapBuffer_GapStart_WithSizeZero()
        {
            var data = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var buffer = new GapBuffer<int>();
            buffer.AddRange(data, 0, 8);

            Assert.AreEqual(0, buffer.GapStart);
            Assert.AreEqual(0, buffer.CurrentGapSize);
            CollectionAssert.AreEqual(data, buffer.ToArray());

            buffer.GapStart = 5;

            Assert.AreEqual(5, buffer.GapStart);
            Assert.AreEqual(0, buffer.CurrentGapSize);
            CollectionAssert.AreEqual(data, buffer.ToArray());

            buffer.GapStart = 2;

            Assert.AreEqual(2, buffer.GapStart);
            Assert.AreEqual(0, buffer.CurrentGapSize);
            CollectionAssert.AreEqual(data, buffer.ToArray());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes=false)]
        public void GapBuffer_GapStart_ArgumentOutOfRangeException()
        {
            var buffer = new GapBuffer<bool> { GapStart = 12 };
        }

        [TestMethod]
        public void GapBuffer_Add()
        {
            var buffer = new GapBuffer<int>(3, 3) { 1000 };

            Assert.AreEqual(1000, buffer[0]);
            Assert.AreEqual(1, buffer.Count);
            Assert.IsTrue(buffer.Contains(1000));
            Assert.IsFalse(buffer.Contains(999));
            Assert.AreEqual(3, buffer.CurrentGapSize);
            CollectionAssert.AreEqual(new[] { 1000 }, buffer.ToArray());

            buffer.Add(2000);
            buffer.Add(3000);
            buffer.Add(4000);

            Assert.AreEqual(1000, buffer[0]);
            Assert.AreEqual(4, buffer.Count);
            Assert.IsTrue(buffer.Contains(1000));
            Assert.IsTrue(buffer.Contains(2000));
            Assert.IsFalse(buffer.Contains(999));
            Assert.AreEqual(3, buffer.CurrentGapSize);

            CollectionAssert.AreEqual(new[] { 1000, 2000, 3000, 4000 }, buffer.ToArray());
        }

        [TestMethod]
        public void GapBuffer_AddRange()
        {
            var buffer = new GapBuffer<int>(8, 10);
            buffer.AddRange(new[] { 1, 2, 3, 4, 5, 6 }, 0, 6);

            Assert.AreEqual(2, buffer[1]);
            Assert.AreEqual(6, buffer.Count);
            Assert.IsTrue(buffer.Contains(5));
            Assert.IsFalse(buffer.Contains(100));
            Assert.AreEqual(8, buffer.CurrentGapSize);

            buffer.AddRange(new[] { 7, 8, 9, 10, 11, 12 }, 0, 6);
            Assert.AreEqual(8, buffer[7]);
            Assert.AreEqual(12, buffer.Count);
            Assert.IsTrue(buffer.Contains(12));
            Assert.IsFalse(buffer.Contains(100));
            Assert.AreEqual(8, buffer.CurrentGapSize);
        }

        [TestMethod]
        public void GapBuffer_Insert()
        {
            var buffer = new GapBuffer<int>(3, 6);
            buffer.AddRange(new[] { 1, 2, 3, 4 }, 0, 4);

            Assert.AreEqual(4, buffer.Count);
            Assert.AreEqual(0, buffer.GapStart);
            Assert.AreEqual(3, buffer.CurrentGapSize);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, buffer.ToArray());

            buffer.Insert(20);

            Assert.AreEqual(5, buffer.Count);
            Assert.AreEqual(1, buffer.GapStart);
            Assert.AreEqual(2, buffer.CurrentGapSize);
            CollectionAssert.AreEqual(new[] { 20, 1, 2, 3, 4 }, buffer.ToArray());

            buffer.Insert(21);
            buffer.Insert(22); // gap filled. new gap created here.

            Assert.AreEqual(7, buffer.Count);
            Assert.AreEqual(3, buffer.GapStart);
            Assert.AreEqual(3, buffer.CurrentGapSize);
            CollectionAssert.AreEqual(new[] { 20, 21, 22, 1, 2, 3, 4 }, buffer.ToArray());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void GapBuffer_Capacity_LowerThanAllowed()
        {
            var buffer = new GapBuffer<int>(10, 10);
            buffer.AddRange(new[] { 1, 2, 3 }, 0, 3);
            buffer.Capacity = 2;
        }

        [TestMethod]
        public void GapBuffer_InsertRange()
        {
            var buffer = new GapBuffer<int>(2, 5);
            buffer.AddRange(new[] { 1, 2, 3 }, 0, 3);

            Assert.AreEqual(3, buffer.Count);
            Assert.AreEqual(5, buffer.CountWithGap);
            Assert.AreEqual(0, buffer.GapStart);
            Assert.AreEqual(2, buffer.CurrentGapSize);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, buffer.ToArray());

            buffer.InsertRange(new[] { 20, 21, 22, 23 });

            Assert.AreEqual(7, buffer.Count);
            Assert.AreEqual(9, buffer.CountWithGap);
            Assert.AreEqual(4, buffer.GapStart);
            Assert.AreEqual(2, buffer.CurrentGapSize);
            CollectionAssert.AreEqual(new[] { 20, 21, 22, 23, 1, 2, 3 }, buffer.ToArray());

            buffer.InsertRange(5, new[] { 100, 101, 102 });

            Assert.AreEqual(10, buffer.Count);
            Assert.AreEqual(11, buffer.CountWithGap);
            Assert.AreEqual(8, buffer.GapStart);
            Assert.AreEqual(1, buffer.CurrentGapSize);
            CollectionAssert.AreEqual(new[] { 20, 21, 22, 23, 1, 100, 101, 102, 2, 3 }, buffer.ToArray());
        }

        [TestMethod]
        public void GapBuffer_TrimExcess()
        {
            var buffer = new GapBuffer<int>(2, 5);
            buffer.AddRange(new[] { 1, 2, 3, 4, 5, 6 }, 0, 6);

            Assert.AreEqual(6, buffer.Count);
            Assert.AreEqual(8, buffer.CountWithGap);
            Assert.AreEqual(0, buffer.GapStart);
            Assert.AreEqual(2, buffer.CurrentGapSize);
            Assert.AreEqual(10, buffer.Capacity);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6 }, buffer.ToArray());

            buffer.TrimExcess();

            Assert.AreEqual(6, buffer.Count);
            Assert.AreEqual(8, buffer.CountWithGap);
            Assert.AreEqual(0, buffer.GapStart);
            Assert.AreEqual(2, buffer.CurrentGapSize);
            Assert.AreEqual(8, buffer.Capacity);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6 }, buffer.ToArray());

            buffer.Add(20);

            Assert.AreEqual(7, buffer.Count);
            Assert.AreEqual(9, buffer.CountWithGap);
            Assert.AreEqual(0, buffer.GapStart);
            Assert.AreEqual(2, buffer.CurrentGapSize);
            Assert.AreEqual(16, buffer.Capacity);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6, 20 }, buffer.ToArray());

            buffer.Clear();
            buffer.TrimExcess(); // TrimExcess() after Clear() = buffer initial state

            Assert.AreEqual(0, buffer.Count);
            Assert.AreEqual(2, buffer.CountWithGap);
            Assert.AreEqual(0, buffer.GapStart);
            Assert.AreEqual(2, buffer.CurrentGapSize);
            Assert.AreEqual(5, buffer.Capacity);
            CollectionAssert.AreEqual(new int[] {}, buffer.ToArray());
        }

        [TestMethod]
        public void GapBuffer_RemoveRange()
        {
            var buffer = new GapBuffer<int>(4, 4);
            buffer.AddRange(new[] { 1, 2, 3, 4, 5, 6 }, 0, 6);

            Assert.AreEqual(6, buffer.Count);
            Assert.AreEqual(10, buffer.CountWithGap);
            Assert.AreEqual(0, buffer.GapStart);
            Assert.AreEqual(4, buffer.CurrentGapSize);
            Assert.AreEqual(16, buffer.Capacity);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6 }, buffer.ToArray());

            buffer.RemoveRange(2, 2);

            Assert.AreEqual(4, buffer.Count);
            Assert.AreEqual(8, buffer.CountWithGap);
            Assert.AreEqual(2, buffer.GapStart);
            Assert.AreEqual(4, buffer.CurrentGapSize);
            Assert.AreEqual(16, buffer.Capacity);
            CollectionAssert.AreEqual(new[] { 1, 2, 5, 6 }, buffer.ToArray());

            buffer.GapStart = 3;

            Assert.AreEqual(4, buffer.Count);
            Assert.AreEqual(8, buffer.CountWithGap);
            Assert.AreEqual(3, buffer.GapStart);
            Assert.AreEqual(4, buffer.CurrentGapSize);
            Assert.AreEqual(16, buffer.Capacity);
            CollectionAssert.AreEqual(new[] { 1, 2, 5, 6 }, buffer.ToArray());

            buffer.RemoveRange(2, 1);

            Assert.AreEqual(3, buffer.Count);
            Assert.AreEqual(7, buffer.CountWithGap);
            Assert.AreEqual(2, buffer.GapStart);
            Assert.AreEqual(4, buffer.CurrentGapSize);
            Assert.AreEqual(16, buffer.Capacity);
            CollectionAssert.AreEqual(new[] { 1, 2, 6 }, buffer.ToArray());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void GapBuffer_RemoveRange_ArgumentOutOfRangeException()
        {
            var buffer = new GapBuffer<int>();
            buffer.AddRange(new[] { 1, 2, 3, 4 }, 0, 4);

            buffer.RemoveRange(5, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void GapBuffer_RemoveRange_ArgumentException()
        {
            var buffer = new GapBuffer<int>();
            buffer.AddRange(new[] { 1, 2, 3, 4 }, 0, 4);

            buffer.RemoveRange(2, 3);
        }

        [TestMethod]
        public void GapBuffer_IndexOf()
        {
            var buffer = new GapBuffer<int>(3, 10);
            buffer.AddRange(new[] { 1, 2, 3, 4 }, 0, 4);

            Assert.AreEqual(2, buffer.IndexOf(3, 0, buffer.Count));
            Assert.AreEqual(3, buffer[buffer.IndexOf(3, 0, buffer.Count)]);

            buffer.GapStart = 1;

            Assert.AreEqual(2, buffer.IndexOf(3, 0, buffer.Count));
            Assert.AreEqual(3, buffer[buffer.IndexOf(3, 0, buffer.Count)]);

            Assert.AreEqual(1, buffer.IndexOf(2, 0, buffer.Count));
            Assert.AreEqual(2, buffer[buffer.IndexOf(2, 0, buffer.Count)]);
            Assert.AreEqual(-1, buffer.IndexOf(1000, 0, buffer.Count));
            Assert.AreEqual(-1, buffer.IndexOf(3, 0, 2));
            Assert.AreEqual(3, buffer.IndexOf(4));
            Assert.AreEqual(0, buffer.IndexOf(1,0));
        }

        [TestMethod]
        public void GapBuffer_CopyTo()
        {
            var buffer = new GapBuffer<int>(3, 3);
            buffer.AddRange(new[] { 1, 2, 3, 4, 5, 6, 7 }, 0, 7);

            var copy3 = new int[3];
            var copy7 = new int[7];

            buffer.CopyTo(0, copy3, 0, 3);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, copy3);

            buffer.CopyTo(2, copy3, 0, 3);
            CollectionAssert.AreEqual(new[] { 3, 4, 5 }, copy3);

            buffer.CopyTo(copy7);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6, 7 }, copy7);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void GapBuffer_CopyTo_ExceptionDestArrayToShort()
        {
            var buffer = new GapBuffer<int>();
            buffer.AddRange(new[] { 1, 2, 3, 4, 5, 6, 7 }, 0, 7);

            buffer.CopyTo(new int[3]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void GapBuffer_CopyTo_ExceptionDestArrayIsNull()
        {
            var buffer = new GapBuffer<int>();
            buffer.AddRange(new[] { 1, 2, 3, 4, 5, 6, 7 }, 0, 7);

            buffer.CopyTo(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void GapBuffer_CopyTo_ExceptionOutOfBounds()
        {
            var buffer = new GapBuffer<int>();
            buffer.AddRange(new[] { 1, 2, 3, 4, 5, 6, 7 }, 0, 7);

            buffer.CopyTo(0, new int[3], 0, 9);
        }
    }
}