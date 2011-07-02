#region Copyright © 2011, João Correia
//
// Copyright © 2011, João Correia
// All rights reserved
// http://joaope.github.com
//
#endregion

#region Using
using System.Collections;
using System.Collections.Generic;
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
    /// 20110701 jcorreia [+] Initial version
    /// __________________________________________________________________________
    /// </history>
    public interface IArray<T>
        : IEnumerable, IEnumerable<T>, ICollection
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
        /// Copies the elements of the <see cref="CircularBuffer{T}"/> to a new array. 
        /// </summary>
        /// <returns>Array containing copies of the elements of the <see cref="CircularBuffer{T}"/>.</returns>
        T[] ToArray();

        /// <summary>
        /// Removes all objects from the <see cref="IArray{T}"/>.
        /// </summary>
        void Clear();
    }
}