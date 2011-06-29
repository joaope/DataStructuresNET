#region Copyright © 2011, João Correia
//
// Copyright © 2011, João Correia
// All rights reserved
// http://joaope.github.com
//
#endregion

#region Using
#endregion

namespace DataStructuresNET.Arrays
{
    /// <summary>
    /// Queues interface.
    /// </summary>
    /// <typeparam name="T">The type of elements hold by the <see cref="IQueue{T}"/>.</typeparam>
    /// <author>João Correia</author>
    /// <owner>João Correia</owner>
    /// <history>
    /// __________________________________________________________________________
    /// History :
    /// 20110628 jcorreia [+] Initial version
    /// __________________________________________________________________________
    /// </history>
    public interface IQueue<T>
    {
        /// <summary>
        /// Adds an object to the end of the <see cref="IQueue{T}"/>. 
        /// </summary>
        /// <param name="item">Item to add to the queue.</param>
        void Enqueue(T item);

        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="IQueue{T}"/>. 
        /// </summary>
        /// <returns>The object that is removed from the beginning of the <see cref="IQueue{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="IQueue{T}"/> is empty.</exception>
        T Dequeue();

        /// <summary>
        /// Returns the object at the beginning of the <see cref="IQueue{T}"/> without removing it. 
        /// </summary>
        /// <returns>The object at the beginning of the <see cref="IQueue{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="IQueue{T}"/> is empty.</exception>
        T Peek();

        /// <summary>
        /// Determines whether an element is in the <see cref="IQueue{T}"/>.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="IQueue{T}"/>.
        /// The value can be <b>null</b> for reference types.</param>
        /// <returns><b>true</b> if item is found in the <see cref="IQueue{T}"/>; otherwise, <b>false</b>.</returns>
        bool Contains(T item);

        /// <summary>
        /// Removes all objects from the <see cref="IQueue{T}"/>.
        /// </summary>
        void Clear();
    }
}
