#region Copyright © 2011, João Correia
//
// Copyright © 2011, João Correia
// All rights reserved
// http://joaope.github.com
//
#endregion

#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
#endregion

namespace DataStructuresNET.Arrays
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
    /// 20110101 jcorreia [+] Initial version
    /// __________________________________________________________________________
    /// </history>
    public class CircularBuffer<T>
        : IEnumerable, IEnumerable<T>, ICollection
    {
        /// <summary>
        /// Field for <see cref="ICollection.SyncRoot"/> property.
        /// </summary>
        private object fSyncRoot;

        /// <summary>
        /// Field for <see cref="Capacity"/> property.
        /// </summary>
        private int fCapacity;

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int Capacity
        {
            get
            {
                return fCapacity;
            }
            set
            {
                lock (((ICollection)this).SyncRoot)
                {
                    if (value == fCapacity)
                    {
                        return;
                    }

                    if (value < Count)
                    {
                        throw new ArgumentOutOfRangeException(
                            "Capacity",
                            value,
                            string.Format("Capacity must be greater than or equal to the current buffer size, which is {0}", Count));
                    }

                    T[] destination = new T[value];

                    if (Count > 0)
                    {
                        CopyTo(destination);
                    }

                    Buffer = destination;
                    fCapacity = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AllowOverwrite { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Head { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Tail { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private T[] Buffer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="allowOverwrite"></param>
        public CircularBuffer(int capacity = 255, bool allowOverwrite = true)
        {
            Capacity = capacity;
            AllowOverwrite = allowOverwrite;

            Head = Tail = Count = 0;
            Buffer = new T[Capacity];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            lock (((ICollection)this).SyncRoot)
            {
                if (!AllowOverwrite && IsFull())
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "CircularBuffer is full ({0} items) and the option to automatically overwrite elements is turned off",
                            Count));
                }

                if (!IsFull())
                {
                    Count++;
                }

                Buffer[Tail] = item;

                if (++Tail == Capacity)
                {
                    Tail = 0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void EnqueueRange(T[] source, int startIndex, int count)
        {
            lock (((ICollection)this).SyncRoot)
            {
                if (source == null)
                {
                    throw new ArgumentNullException(
                        "source",
                        "Source cannot be null");
                }

                if (startIndex >= source.Length || startIndex < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "startIndex",
                        startIndex,
                        string.Format(
                            "StartIndex argument is a zero-based index and it must be lower than source actual length ({0})",
                            source.Length));
                }

                if ((startIndex + count) > source.Length)
                {
                    throw new ArgumentException("Number of elements to add, starting from startIndex, exceeds the source actual length");
                }

                for (int i = startIndex; count > 0; count--, i++)
                {
                    Enqueue(source[i]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            lock (((ICollection)this).SyncRoot)
            {
                int bufferIndex = Head;
                var comparer = EqualityComparer<T>.Default;

                for (int i = 0; i < Count; i++, bufferIndex++)
                {
                    if (bufferIndex == Capacity)
                    {
                        bufferIndex = 0;
                    }

                    if (item == null && Buffer[bufferIndex] == null)
                    {
                        return true;
                    }
                    else if ((Buffer[bufferIndex] != null) && comparer.Equals(Buffer[bufferIndex], item))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="invalidOperationException"></exception>
        public T Dequeue()
        {
            lock (((ICollection)this).SyncRoot)
            {
                if (IsEmpty())
                {
                    throw new InvalidOperationException("CircularBuffer is empty");
                }

                T item = Buffer[Head];

                if (Head++ == Capacity)
                {
                    Head = 0;
                }

                Count--;
                return item;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="invalidOperationException"></exception>
        public T Peek()
        {
            lock (((ICollection)this).SyncRoot)
            {
                if (IsEmpty())
                {
                    throw new InvalidOperationException("CircularBuffer is empty");
                }

                return Buffer[Head];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void CopyTo(T[] array, int startIndex, int count)
        {
            lock (((ICollection)this).SyncRoot)
            {
                if (startIndex >= Count || startIndex < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "startIndex",
                        startIndex,
                        string.Format(
                            "StartIndex argument is a zero-based index and it must be lower than buffer length ({0})",
                            Count));
                }

                if ((startIndex + count) > Count)
                {
                    throw new ArgumentException(
                        string.Format("Number of elements to copy, starting from startIndex, exceeds buffer length ({0})", Count));
                }

                int bufferIndex = Head;
                for (int i = 0; i < count; i++, bufferIndex++, startIndex++)
                {
                    if (bufferIndex == Capacity)
                        bufferIndex = 0;
                    array[startIndex] = Buffer[bufferIndex];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="startIndex"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void CopyTo(T[] array, int startIndex)
        {
            CopyTo(array, startIndex, Count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            lock (((ICollection)this).SyncRoot)
            {
                T[] array = new T[Count];
                CopyTo(array);
                return array;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsFull()
        {
            return (Capacity == Count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return (Count == 0);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            lock (((ICollection)this).SyncRoot)
            {
                Count = Tail = Head = 0;
                Buffer = new T[Capacity];
            }
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="CircularBuffer{T}"/>, if that
        /// number is less than 90 percent of current capacity. 
        /// </summary>
        /// <remarks>
        /// This method can be used to minimize a collection's memory overhead if no new elements will be 
        /// added to the collection. The cost of reallocating and copying a large Queue<T>can be considerable, 
        /// however, so the <b>TrimExcess</b> method does nothing if the list is at more than 90 percent of capacity.
        /// This avoids incurring a large reallocation cost for a relatively small gain.
        /// 
        /// The <see cref="Tail"/>, which was pointing to the next empty space, is now 0, which corresponds to
        /// the next element on the queue.
        /// 
        /// This method is an O(n) operation, where n is <see cref="Count"/>.
        /// 
        /// To reset a <see cref="CircularBuffer{T}"/> to its initial state, call the Clear method before 
        /// calling <b>TrimExcess</b> method. Trimming an empty <see cref="CircularBuffer{T}"/> sets the capacity of 
        /// the <see cref="CircularBuffer{T}"/> to the default capacity. 
        /// </remarks>
        public void TrimExcess()
        {
            lock (((ICollection)this).SyncRoot)
            {
                if (Count > 0 && (Count * 100 / Capacity) <= 90)
                {
                    int newCapacity = Count;

                    T[] newBuffer = new T[newCapacity];
                    CopyTo(newBuffer);

                    Buffer = newBuffer;
                    Capacity = newCapacity;

                    Tail = 0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("CircularBuffer(Head={0}, Tail={1}, Capacity={2}, Count={3})", Head, Tail, Capacity, Count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            lock (((ICollection)this).SyncRoot)
            {
                int currentIndex = Head;

                for (int i = 0; i < Count; i++, currentIndex++)
                {
                    if (currentIndex == Capacity)
                    {
                        currentIndex = 0;
                    }
                    yield return Buffer[currentIndex];
                }
            }
        }

        #region IEnumerable explicit implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        #endregion

        #region IEnumerable<T> explicit implementation

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ICollection explicit implementation

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((T[])array, index);
        }

        int ICollection.Count
        {
            get { return Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (fSyncRoot == null)
                {
                    Interlocked.CompareExchange(ref fSyncRoot, new object(), null);
                }
                return fSyncRoot;
            }
        }

        #endregion
    }
}