#region Copyright © 2014, João Correia
//
// Copyright © 2014, João Correia
// All rights reserved
// http://joaope.github.com
//
#endregion

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace DataStructuresNET.Arrays
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class GapBuffer<T>
        : IArray<T>
    {
        /// <summary>
        /// 
        /// </summary>
        private object fSyncRoot;
        
        /// <summary>
        /// 
        /// </summary>
        private int fGapStart;

        /// <summary>
        /// 
        /// </summary>
        public int GapStart
        {
            get
            {
                return fGapStart;
            }
            set
            {
                if (value == fGapStart)
                {
                    return;
                }

                if (value < 0 || value >= Count)
                {
                    throw new ArgumentOutOfRangeException(
                        "value",
                        value,
                        "GapStart can't be lower than 0 and higher or equal to Count");
                }

                lock (((ICollection)this).SyncRoot)
                {
                    if (value > fGapStart)
                    {
                        var delta = value - fGapStart;

                        var segment = new T[delta];
                        Array.Copy(buffer, (GapEnd == GapStart ? GapSize : GapEnd), segment, 0, delta);

                        Array.Copy(segment, 0, buffer, fGapStart, delta);
                    }
                    else
                    {
                        var delta = fGapStart - value;

                        var segment = new T[delta];
                        Array.Copy(buffer, GapStart - delta, segment, 0, delta);

                        Array.Copy(segment, 0, buffer, GapEnd - delta, delta);
                    }

                    fGapStart = value;
                    GapEnd = fGapStart + GapSize;

                    // ResetGap(); // debug purposes only
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private int GapEnd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int BufferEnd
        {
            get
            {
                return Count + CurrentGapSize;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int GapSize { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int CurrentGapSize
        {
            get
            {
                return GapEnd - GapStart;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private T[] buffer;

        /// <summary>
        /// 
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int CountWithGap
        {
            get { return CurrentGapSize + Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        private int InitialCapacity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private int fCapacity;

        /// <summary>
        /// Gets or sets the total number of elements the internal data structure can hold without resizing.
        /// </summary>
        public int Capacity
        {
            get
            {
                return fCapacity;
            }
            set
            {
                if (value == fCapacity)
                {
                    return;
                }

                if (value < CountWithGap)
                {
                    throw new ArgumentOutOfRangeException(
                        "value",
                        value,
                        string.Format("Capacity must be greater than or equal to CountWithGap value ({0})", CountWithGap));
                }

                lock (((ICollection)this).SyncRoot)
                {
                    fCapacity = value;
                    Array.Resize(ref buffer, fCapacity + GapSize);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return (Count == 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFull
        {
            get
            {
                return (Count == Capacity);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gapSize"></param>
        /// <param name="capacity"></param>
        public GapBuffer(int gapSize = 0, int capacity = 0)
        {
            if (gapSize < 0)
            {
                throw new ArgumentOutOfRangeException("gapSize", gapSize, "GapSize must be higher than 0");
            }

            if (capacity < 0) 
            {
                throw new ArgumentOutOfRangeException("capacity", capacity, "Capacity must be higher than 0");
            }

            if (capacity < gapSize)
            {
                throw new ArgumentOutOfRangeException(
                    "capacity",
                    capacity,
                    "Capacity should have, at least, the same value as GapSize");
            }

            lock (((ICollection)this).SyncRoot)
            {
                fSyncRoot = new Object();

                var bufSize = capacity + gapSize;
                buffer = new T[(bufSize == 0 ? 4 : bufSize)];

                GapSize = gapSize;
                fGapStart = 0;
                GapEnd = gapSize;
                Count = 0;
                InitialCapacity = Capacity = capacity;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            lock (((ICollection)this).SyncRoot)
            {
                Array.Clear(buffer, 0, buffer.Length);

                fGapStart = 0;
                GapEnd = GapSize;
                Count = 0;
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
                var comparer = EqualityComparer<T>.Default;

                for (var i = 0; i < fGapStart; i++)
                {
                    if (comparer.Equals(buffer[i], item))
                    {
                        return true;
                    }
                }

                var end = BufferEnd;
                for (var i = GapEnd; i < end; i++)
                {
                    if (comparer.Equals(buffer[i], item))
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
        public T[] ToArray()
        {
            lock (((ICollection)this).SyncRoot)
            {
                var array = new T[Count];

                Array.Copy(buffer, 0, array, 0, GapStart);
                Array.Copy(buffer, GapEnd, array, GapStart, BufferEnd - GapEnd);

                return array;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(
                        "index",
                        index,
                        string.Format(
                            "Index must be non-negative and less than the array count ({0})",
                            Count));
                }

                lock (((ICollection)this).SyncRoot)
                {
                    if (index >= fGapStart)
                    {
                        index = GapEnd + (index - fGapStart);
                    }

                    return buffer[index];
                }
            }
            set
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(
                        "index",
                        index,
                        string.Format(
                            "Index must be non-negative and less than the array count ({0})",
                            Count));
                }

                lock (((ICollection)this).SyncRoot)
                {
                    if (index >= fGapStart)
                    {
                        index = GapEnd + (index - fGapStart);
                    }

                    buffer[index] = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemsToAdd"></param>
        private void ExpandCapacityIfNecessary(int itemsToAdd)
        {
            lock (((ICollection)this).SyncRoot)
            {
                var totalItems = CountWithGap + itemsToAdd;

                if (totalItems > Capacity)
                {
                    var newCapacity = (Capacity == 0 ? 4 : Capacity);

                    while (newCapacity < totalItems) // double until satisfy
                    {
                        newCapacity *= 2;
                    }

                    Array.Resize(ref buffer, newCapacity);
                    Capacity = newCapacity;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void EnsureGap()
        {
            lock (((ICollection)this).SyncRoot)
            {
                if (CurrentGapSize == 0)
                {
                    ExpandCapacityIfNecessary(0);

                    Array.Copy(buffer, GapEnd, buffer, GapEnd + GapSize, BufferEnd - GapEnd);
                    GapEnd += GapSize;

                    // ResetGap(); // debug purposes only
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            lock (((ICollection)this).SyncRoot)
            {
                ExpandCapacityIfNecessary(1);

                buffer[BufferEnd] = item;
                Count++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceIndex"></param>
        /// <param name="length"></param>
        public void AddRange(T[] source, int sourceIndex, int length)
        {
            if (source == null)
            {
                throw new ArgumentNullException(
                    "source",
                    "Source cannot be null");
            }

            if (sourceIndex < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "sourceIndex",
                    sourceIndex,
                    "sourceIndex is less than 0 (zero)");
            }

            if (sourceIndex >= source.Length)
            {
                throw new ArgumentException("sourceIndex and length do not denote a valid range of elements in the GapBuffer");
            }

            lock (((ICollection)this).SyncRoot)
            {
                ExpandCapacityIfNecessary(source.Length);

                Array.Copy(source, 0, buffer, BufferEnd, source.Length);
                Count += source.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newGapStart"></param>
        /// <param name="item"></param>
        public void Insert(int newGapStart, T item)
        {
            try
            {
                GapStart = newGapStart;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException(
                    "New GapStart is less than 0 (zero) or higher than the actual number of elements within GapBuffer",
                    e);
            }

            lock (((ICollection)this).SyncRoot)
            {
                buffer[GapStart] = item;
                Count++;
                fGapStart++;

                EnsureGap();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Insert(T item)
        {
            Insert(GapStart, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newGapStart"></param>
        /// <param name="source"></param>
        public void InsertRange(int newGapStart, T[] source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(
                    "source",
                    "Source cannot be null");
            }

            try
            {
                GapStart = newGapStart;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException(
                    "New GapStart is less than 0 (zero) or higher than the actual number of elements within GapBuffer",
                    e);
            }

            lock (((ICollection)this).SyncRoot)
            {
                var itemsInserted = 0;

                while (itemsInserted < source.Length)
                {
                    var segmentLength = (itemsInserted + CurrentGapSize <= source.Length ? CurrentGapSize : source.Length - itemsInserted);

                    var segmentToInsert = new T[segmentLength];
                    Array.Copy(source, itemsInserted, segmentToInsert, 0, segmentLength);

                    Array.Copy(segmentToInsert, 0, buffer, GapStart, segmentLength);
                    fGapStart += segmentLength;
                    Count += segmentLength;
                    EnsureGap();

                    // ResetGap(); // debug purposes only

                    itemsInserted += segmentLength;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public void InsertRange(T[] source)
        {
            InsertRange(GapStart, source);
        }

        /// <summary>
        /// 
        /// </summary>
        public void TrimExcess()
        {
            lock (((ICollection)this).SyncRoot)
            {
                if (IsEmpty)
                {
                    Capacity = InitialCapacity;
                    buffer = new T[Capacity];
                }
                else
                {
                    if (CountWithGap <= Capacity * 0.9)
                    {
                        Capacity = CountWithGap;
                        Array.Resize(ref buffer, Capacity);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public void RemoveRange(int index, int count)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(
                    "index",
                    index,
                    "index is less than zero or index is higher or equal to the actual number of elements within GapBuffer");
            }

            if ((index + count) > Count)
            {
                throw new ArgumentException("index and count do not denote a valid range of elements");
            }

            lock (((ICollection)this).SyncRoot)
            {
                var bufferWithoutGap = new T[buffer.Length - GapSize];
                Array.Copy(buffer, 0, bufferWithoutGap, 0, GapStart);
                Array.Copy(buffer, GapEnd, bufferWithoutGap, GapStart, buffer.Length - GapEnd);

                Array.Clear(buffer, 0, buffer.Length);
                Array.Copy(bufferWithoutGap, 0, buffer, 0, index);
                Array.Copy(bufferWithoutGap, index + count, buffer, index + GapSize, bufferWithoutGap.Length - index - count);

                Count -= count;
                fGapStart = index;
                GapEnd = index + GapSize;

                // ResetGap(); // debug purposes only
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public int IndexOf(T item, int index, int count)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(
                    "index",
                    index,
                    "index is less than zero or index is higher or equal to the actual number of elements within GapBuffer");
            }

            if ((index + count) > Count)
            {
                throw new ArgumentException("index and count do not denote a valid range of elements");
            }

            lock (((ICollection)this).SyncRoot)
            {
                var comparer = EqualityComparer<T>.Default;
                var i = index;

                for (; i < fGapStart; i++)
                {
                    if (i == count)
                    {
                        return -1;
                    }

                    if (comparer.Equals(buffer[i], item))
                    {
                        return i;
                    }
                }

                var end = BufferEnd;
                for (var j = GapEnd; j < end; j++, i++)
                {
                    if (i == count)
                    {
                        return -1;
                    }

                    if (comparer.Equals(buffer[j], item))
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public int IndexOf(T item, int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(
                    "index",
                    index,
                    "Index can't be lower than 0 and higher or equal to Count");
            }

            return IndexOf(item, index, Count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            return IndexOf(item, 0, Count);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetGap()
        {
            lock (((ICollection)this).SyncRoot)
            {
                Array.Copy(new T[CurrentGapSize], 0, buffer, GapStart, CurrentGapSize);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            lock (((ICollection)this).SyncRoot)
            {
                for (var i = 0; i < fGapStart; i++)
                {
                    yield return buffer[i];
                }

                var end = BufferEnd;
                for (var i = GapEnd; i < end; i++)
                {
                    yield return buffer[i];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        /// <param name="count"></param>
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            lock (((ICollection)this).SyncRoot)
            {
                try
                {
                    Array.Copy(ToArray(), index, array, arrayIndex, count);
                }
                catch (ArgumentNullException e)
                {
                    throw new ArgumentNullException("array is null", e);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    throw new ArgumentOutOfRangeException(
                        "either index, arrayIndex or length are less then 0 (zero) or out of bounds with associated arrays",
                        e);
                }
                catch (ArgumentException e)
                {
                    throw new ArgumentException(
                        "index is equal to or greater than Count of the source GapBuffer",
                        e);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            try
            {
                CopyTo(0, array, arrayIndex, Count);
            }
            catch (ArgumentNullException e)
            {
                throw new ArgumentNullException("array is null", e);
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException("arrayIndex is less than 0 (zero)", e);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(
                    "number of elements in the source is greater then the space available from arrayIndex to the end of the destination array",
                    e);
            }
        }

        public void CopyTo(T[] array)
        {
            try
            {
                CopyTo(array, 0);
            }
            catch (ArgumentNullException e)
            {
                throw new ArgumentNullException("array is null", e);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(
                    "number of elements in the source is greater then the number of elements that the destination array can contain",
                    e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("GapBuffer<{0}>(GapSize={1}, Capacity={2}, GapStart={3}, CurrentGapSize={4}, Count={5}, CountWithGap={6}",
                typeof(T),
                GapSize,
                Capacity,
                GapStart,
                CurrentGapSize,
                Count,
                CountWithGap);
        }

        #region IEnumerable explicit implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
            get { return true; }
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