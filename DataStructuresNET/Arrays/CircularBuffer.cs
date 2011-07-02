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
    /// Circular Buffer (Queue) implementation.
    /// </summary>
    /// <typeparam name="T">The type of elements hold by the <see cref="CircularBuffer{T}"/>.</typeparam>
    /// <author>João Correia</author>
    /// <owner>João Correia</owner>
    /// <history>
    /// __________________________________________________________________________
    /// History :
    /// 20110628 jcorreia [+] Initial version
    /// __________________________________________________________________________
    /// </history>
    public class CircularBuffer<T>
        : IQueue<T>, IArray<T>
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
        /// Gets and sets the buffer capacity.
        /// </summary>
        /// <remarks>
        /// To increase buffer size the reallocation of that same buffer is needed which can cost time and
        /// memory overhead in case of very large buffers.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the new specified capacity is lower than the current number of elements hold by the buffer.
        /// </exception>
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
        /// Get or set the element at the specified index. 
        /// 
        /// Although the index is zero-based take into account that the count starts from 
        /// the <see cref="Head"/> value, ie. if you specify 0 as index the returned value will be
        /// the element at the head of the buffer.
        /// </summary>
        /// <remarks>It accepts <b>null</b> as a valid value and allows duplicate elements.</remarks>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than 0. 
        /// 
        /// -or-
        ///
        /// index is equal to or greater than Count.
        /// </exception>
        public T this[int index]
        {
            get 
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(
                        "indexer",
                        index,
                        string.Format(
                            "Indexer property index out of range. Shouldn't be less than 0 and equal or greater than Count ({0})", 
                            Count));
                }

                int trueIndex = Head + index;
                return Buffer[trueIndex >= Capacity ? trueIndex - Capacity : trueIndex];
            }
            set
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(
                        "indexer",
                        index,
                        string.Format(
                            "Indexer property index out of range. Shouldn't be less than 0 and equal or greater than Count ({0})",
                            Count));
                }

                int trueIndex = Head + index;
                Buffer[trueIndex >= Capacity ? trueIndex - Capacity : trueIndex] = value;
            }
        }

        /// <summary>
        /// Determines if <see cref="CircularBuffer{T}"/> if full.
        /// </summary>
        /// <returns><b>true</b> if buffer is full; otherwise false.</returns>
        public bool IsFull
        {
            get 
            { 
                return (Capacity == Count);
            }
        }

        /// <summary>
        /// Determines if <see cref="CircularBuffer{T}"/> is empty.
        /// </summary>
        /// <returns><b>true</b> if buffer is empty; otherwise false.</returns>
        public bool IsEmpty
        {
            get
            {
                return (Count == 0);
            }
        }

        /// <summary>
        /// Gets and sets the value indicating if the queue allows the overlap of elements when the buffer is full.
        /// </summary>
        public bool AllowOverwrite { get; set; }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="CircularBuffer{T}"/>. 
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets the circular buffer head.
        /// </summary>
        public int Head { get; private set; }

        /// <summary>
        /// Gets the circular buffer tail.
        /// </summary>
        public int Tail { get; private set; }

        /// <summary>
        /// Gets and sets circular buffer data (internal property).
        /// </summary>
        internal T[] Buffer { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularBuffer{T}"/> class.
        /// </summary>
        /// <param name="capacity">The initial buffer's capacity.</param>
        /// <param name="allowOverwrite">Indicates if elements overwrite is allowed.</param>
        public CircularBuffer(int capacity = 255, bool allowOverwrite = true)
        {
            Capacity = capacity;
            AllowOverwrite = allowOverwrite;

            Head = Tail = Count = 0;
            Buffer = new T[Capacity];
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="CircularBuffer{T}"/>. 
        /// </summary>
        /// <param name="item">Item to add to the queue.</param>
        public void Enqueue(T item)
        {
            lock (((ICollection)this).SyncRoot)
            {
                if (!AllowOverwrite && IsFull)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "CircularBuffer is full ({0} items) and the option to automatically overwrite elements is turned off",
                            Count));
                }

                if (!IsFull)
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
        /// Adds a range of items to the <see cref="CircularBuffer{T}"/>.
        /// </summary>
        /// <param name="collection">Array of items to add to thequeue.</param>
        /// <param name="startIndex"><paramref name="colletion"/>'s starting element index.</param>
        /// <param name="count">Number of items to enqueue starting on <paramref name="startIndex"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="collection"/> is lower than 0 (zero-based index)
        /// 
        /// -OR-
        /// 
        /// If <paramref name="collection"/> is higher than the actual <see cref="CircularBuffer{T}"/> length.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Number of elements to add, starting from <paramref name="startIndex"/>, exceeds 
        /// the <see cref="CircularBuffer{T}"/>'s actual length.
        /// </exception>
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
        /// Determines whether an element is in the <see cref="CircularBuffer{T}"/>.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="CircularBuffer{T}"/>.
        /// The value can be <b>null</b> for reference types.</param>
        /// <returns><b>true</b> if item is found in the <see cref="CircularBuffer{T}"/>; otherwise, <b>false</b>.</returns>
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
        /// Removes and returns the object at the beginning of the <see cref="CircularBuffer{T}"/>. 
        /// </summary>
        /// <returns>The object that is removed from the beginning of the <see cref="CircularBuffer{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="CircularBuffer{T}"/> is empty.</exception>
        public T Dequeue()
        {
            lock (((ICollection)this).SyncRoot)
            {
                if (IsEmpty)
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
        /// Returns the object at the beginning of the <see cref="CircularBuffer{T}"/> without removing it. 
        /// </summary>
        /// <returns>The object at the beginning of the <see cref="CircularBuffer{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="CircularBuffer{T}"/> is empty.</exception>
        public T Peek()
        {
            lock (((ICollection)this).SyncRoot)
            {
                if (IsEmpty)
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
        /// Copies the elements of the <see cref="CircularBuffer{T}"/> to a new array. 
        /// </summary>
        /// <returns>Array containing copies of the elements of the <see cref="CircularBuffer{T}"/>.</returns>
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
        /// Removes all objects from the <see cref="CircularBuffer{T}"/>.
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
        /// however, the <b>TrimExcess</b> method does nothing if the list is at more than 90 percent of capacity.
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
                if (IsEmpty)
                {
                    Buffer = new T[Capacity];
                    Tail = 0;
                }
                else if (Count > 0 && (Count * 100 / Capacity) <= 90)
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