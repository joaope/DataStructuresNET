#region Copyright © 2011, João Correia
//
// Copyright © 2011, João Correia
// All rights reserved
// http://joaope.github.com
//
#endregion

#region Using
using System;
using DataStructuresNET.Arrays;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endregion

namespace DataStructuresNET.Tests.Arrays
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <author>João Correia</author>
    /// <owner>João Correia</owner>
    /// <history>
    /// __________________________________________________________________________
    /// History :
    /// 20110728 jcorreia [+] Initial version
    /// __________________________________________________________________________
    /// </history>
    [TestClass]
    public class CircularBufferTests
    {
        [TestMethod]
        public void CircularBuffer_IndexerGet()
        {
            var buffer = new CircularBuffer<int>(10);
            buffer.EnqueueRange(
                new int[5] { 12, 4, 9, 43, 0 }, 
                0, 5);

            Assert.AreEqual<int>(12, buffer[0]);
            Assert.AreEqual<int>(0, buffer[4]);

            buffer.TrimExcess();
            buffer.EnqueueRange(new int[] { 99, 100 }, 0, 2);

            Assert.AreEqual<int>(99, buffer[0]);
            Assert.AreEqual<int>(9, buffer[2]);
        }

        [TestMethod]
        public void CircularBuffer_CopyTo()
        {
            var data = new int[5] { 12, 4, 9, 43, 0 };
            var buffer = new CircularBuffer<int>(10);
            buffer.EnqueueRange(data, 0, data.Length);

            var dataCopy = new int[5];
            buffer.CopyTo(dataCopy, 0, 5);

            CollectionAssert.AreEqual(new int[5] { 12, 4, 9, 43, 0 }, dataCopy);
        }

        [TestMethod]
        public void CircularBuffer_Enqueue()
        {
            var buffer = new CircularBuffer<string>(10);
            buffer.Enqueue("test");
            buffer.Enqueue("string");

            CollectionAssert.AreEqual(new string[] { "test", "string" }, buffer.ToArray());
            Assert.AreEqual<int>(2, buffer.Count);
            Assert.AreEqual<int>(10, buffer.Capacity);
            Assert.AreEqual<int>(0, buffer.Head);
            Assert.AreEqual<int>(2, buffer.Tail);
            Assert.IsTrue(buffer.Contains("string"));
            Assert.IsFalse(buffer.Contains("other string"));
        }

        [TestMethod]
        public void CircularBuffer_EnqueueRange()
        {
            string[] data = { "a", "b", "c", "d", "e" };
            var buffer = new CircularBuffer<string>(data.Length);
            buffer.EnqueueRange(data, 0, data.Length);

            CollectionAssert.AreEqual(data, buffer.ToArray());
            Assert.AreEqual<int>(data.Length, buffer.Count);
            Assert.AreEqual<int>(data.Length, buffer.Capacity);
            Assert.AreEqual<int>(0, buffer.Head);
            Assert.AreEqual<int>(0, buffer.Tail);
            Assert.IsTrue(buffer.Contains("a"));
            Assert.IsFalse(buffer.Contains("z"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
        public void CircularBuffer_EnqueueOverwriteNotAllowed()
        {
            int[] data = { 1, 2, 3, 4, 5 };
            var buffer = new CircularBuffer<int>(data.Length, false);
            buffer.EnqueueRange(data, 0, data.Length);
            buffer.Enqueue(10);
        }

        [TestMethod]
        public void CircularBuffer_EnqueueOverwriteAllowed()
        {
            string[] data = { "a", "b", "c", "d" };
            var buffer = new CircularBuffer<string>(data.Length);
            buffer.EnqueueRange(data, 0, data.Length);

            buffer.Enqueue("z");

            CollectionAssert.AreEqual(new string[] { "z", "b", "c", "d" }, buffer.ToArray());
            Assert.AreEqual<int>(data.Length, buffer.Count);
            Assert.AreEqual<int>(data.Length, buffer.Capacity);
            Assert.AreEqual<int>(0, buffer.Head);
            Assert.AreEqual<int>(1, buffer.Tail);
            Assert.IsTrue(buffer.Contains("z"));
            Assert.IsFalse(buffer.Contains("a"));
        }

        [TestMethod]
        public void CircularBuffer_EnqueueAfterTrimExcess()
        {
            var buffer = new CircularBuffer<string>(10);
            buffer.EnqueueRange(new string[] { "a", "b", "c" }, 0, 3);

            buffer.TrimExcess();
            buffer.Enqueue("z");

            CollectionAssert.AreEqual(new string[] { "z", "b", "c" }, buffer.ToArray());
            Assert.AreEqual<int>(3, buffer.Count);
            Assert.AreEqual<int>(3, buffer.Capacity);
            Assert.AreEqual<int>(0, buffer.Head);
            Assert.AreEqual<int>(1, buffer.Tail);
            Assert.IsTrue(buffer.Contains("z"));
            Assert.IsFalse(buffer.Contains("a"));
        }

        [TestMethod]
        public void CircularBuffer_Dequeue()
        {
            int[] data = { 0, 1, 2, 3 };
            var buffer = new CircularBuffer<int>(data.Length);
            buffer.EnqueueRange(data, 0, data.Length);

            buffer.Dequeue();

            Assert.AreEqual<int>(3, buffer.Count);
            Assert.AreEqual<int>(data.Length, buffer.Capacity);
            Assert.AreEqual<int>(1, buffer.Head);
            Assert.AreEqual<int>(0, buffer.Tail);
            Assert.IsTrue(buffer.Contains(1));
            Assert.IsFalse(buffer.Contains(0));
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, buffer.ToArray());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
        public void CircularBuffer_DequeueWhenEmpty()
        {
            var buffer = new CircularBuffer<string>();
            string s = buffer.Dequeue();
        }

        [TestMethod]
        public void CircularBuffer_TrimExcess()
        {
            var buffer = new CircularBuffer<string>(10);
            buffer.Enqueue("a");
            buffer.Enqueue("b");

            buffer.TrimExcess();

            Assert.AreEqual<int>(2, buffer.Count);
            Assert.AreEqual<int>(2, buffer.Capacity);
            Assert.AreEqual<int>(0, buffer.Head);
            Assert.AreEqual<int>(0, buffer.Tail);
            Assert.IsTrue(buffer.Contains("a"));
            Assert.IsFalse(buffer.Contains("z"));
            CollectionAssert.AreEqual(new string[] { "a", "b" }, buffer.ToArray());
        }
    }
}