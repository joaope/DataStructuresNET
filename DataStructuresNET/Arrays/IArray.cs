#region Copyright © 2014, João Correia
//
// Copyright © 2014, João Correia
// All rights reserved
// http://joaope.github.com
//
#endregion

using System.Collections;
using System.Collections.Generic;

namespace DataStructuresNET.Arrays
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    public interface IArray<T>
        : IEnumerable<T>, ICollection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        T this[int index] { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int Capacity { get; set; }

        /// <summary>
        /// Determines if <see cref="IArray{T}"/> is empty.
        /// </summary>
        /// <returns><b>true</b> if buffer is empty; otherwise false.</returns>
        bool IsEmpty { get; }

        /// <summary>
        /// Determines if <see cref="IArray{T}"/> if full.
        /// </summary>
        /// <returns><b>true</b> if buffer is full; otherwise false.</returns>
        bool IsFull { get; }

        /// <summary>
        /// Determines whether an element is in the <see cref="IArray{T}"/>.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="IArray{T}"/>.
        /// The value can be <b>null</b> for reference types.</param>
        /// <returns><b>true</b> if item is found in the <see cref="CircularBuffer{T}"/>; otherwise, <b>false</b>.</returns>
        bool Contains(T item);

        /// <summary>
        /// Copies the elements of the <see cref="IArray{T}"/> to a new array. 
        /// </summary>
        /// <returns>Array containing copies of the elements of the <see cref="IArray{T}"/>.</returns>
        T[] ToArray();

        /// <summary>
        /// Removes all objects from the <see cref="IArray{T}"/>.
        /// </summary>
        void Clear();

        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="IArray{T}"/>, if that
        /// number is less than 90 percent of current capacity. 
        /// </summary>
        /// <remarks>
        /// This method can be used to minimize a collection's memory overhead if no new elements will be 
        /// added to the collection. The cost of reallocating and copying a large <see cref="IArray{T}"/> can be considerable, 
        /// however, the <b>TrimExcess</b> method does nothing if the list is at more than 90 percent of capacity.
        /// This avoids incurring a large reallocation cost for a relatively small gain.
        /// 
        /// To reset a <see cref="IArray{T}"/> to its initial state, call the Clear method before 
        /// calling <b>TrimExcess</b> method. Trimming an empty <see cref="IArray{T}"/> sets the capacity of 
        /// the <see cref="IArray{T}"/> to the default capacity. 
        /// </remarks>
        void TrimExcess();
    }
}