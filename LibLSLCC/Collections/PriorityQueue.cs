﻿#region FileInfo

// 
// File: PriorityQueue.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2015, Eric A. Blundell
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
// 

#endregion

#region Imports

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace LibLSLCC.Collections
{
    /// <summary>
    ///     Priority queue based on binary heap,
    ///     Elements with minimum priority dequeued first
    /// </summary>
    /// <typeparam name="TPriority">Type of priorities</typeparam>
    /// <typeparam name="TValue">Type of values</typeparam>
    public class PriorityQueue<TPriority, TValue> : ICollection<KeyValuePair<TPriority, TValue>>, ICloneable
    {
        private readonly List<KeyValuePair<TPriority, TValue>> _baseHeap;
        private readonly IComparer<TPriority> _comparer;


        /// <summary>
        ///     Create a shallow clone of this priority queue.
        /// </summary>
        /// <returns>A shallow clone of this priority queue.</returns>
        public virtual object Clone()
        {
            var instance = new PriorityQueue<TPriority, TValue>(_comparer);
            foreach (var item in _baseHeap)
            {
                instance._baseHeap.Add(item);
            }
            return instance;
        }

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of priority queue with default initial capacity and default priority comparer
        /// </summary>
        public PriorityQueue()
            : this(Comparer<TPriority>.Default)
        {
        }


        /// <summary>
        ///     Initializes a new instance of priority queue with specified initial capacity and default priority comparer
        /// </summary>
        /// <param name="capacity">initial capacity</param>
        public PriorityQueue(int capacity)
            : this(capacity, Comparer<TPriority>.Default)
        {
        }


        /// <summary>
        ///     Initializes a new instance of priority queue with specified initial capacity and specified priority comparer
        /// </summary>
        /// <param name="capacity">initial capacity</param>
        /// <param name="comparer">priority comparer</param>
        public PriorityQueue(int capacity, IComparer<TPriority> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            _baseHeap = new List<KeyValuePair<TPriority, TValue>>(capacity);
            _comparer = comparer;
        }


        /// <summary>
        ///     Initializes a new instance of priority queue with default initial capacity and specified priority comparer
        /// </summary>
        /// <param name="comparer">priority comparer</param>
        public PriorityQueue(IComparer<TPriority> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            _baseHeap = new List<KeyValuePair<TPriority, TValue>>();
            _comparer = comparer;
        }


        /// <summary>
        ///     Initializes a new instance of priority queue with specified data and default priority comparer
        /// </summary>
        /// <param name="data">data to be inserted into priority queue</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public PriorityQueue(IEnumerable<KeyValuePair<TPriority, TValue>> data)
            : this(data, Comparer<TPriority>.Default)
        {
        }


        /// <summary>
        ///     Initializes a new instance of priority queue with specified data and specified priority comparer
        /// </summary>
        /// <param name="data">data to be inserted into priority queue</param>
        /// <param name="comparer">priority comparer</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public PriorityQueue(IEnumerable<KeyValuePair<TPriority, TValue>> data, IComparer<TPriority> comparer)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            _comparer = comparer;
            _baseHeap = new List<KeyValuePair<TPriority, TValue>>(data);
            // heapify data
            for (var pos = _baseHeap.Count/2 - 1; pos >= 0; pos--)
                HeapifyFromBeginningToEnd(pos);
        }

        #endregion

        #region Merging

        /// <summary>
        ///     Merges two priority queues
        /// </summary>
        /// <param name="pq1">first priority queue</param>
        /// <param name="pq2">second priority queue</param>
        /// <returns>resultant priority queue</returns>
        /// <remarks>
        ///     source priority queues must have equal comparers,
        ///     otherwise <see cref="InvalidOperationException" /> will be thrown
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static PriorityQueue<TPriority, TValue> MergeQueues(PriorityQueue<TPriority, TValue> pq1,
            PriorityQueue<TPriority, TValue> pq2)
        {
            if (pq1 == null)
            {
                throw new ArgumentNullException("pq1");
            }

            if (pq2 == null)
            {
                throw new ArgumentNullException("pq2");
            }

            if (pq1._comparer != pq2._comparer)
            {
                throw new InvalidOperationException("Priority queues to be merged must have equal comparers");
            }
            return MergeQueues(pq1, pq2, pq1._comparer);
        }


        /// <summary>
        ///     Merges two priority queues and sets specified comparer for resultant priority queue
        /// </summary>
        /// <param name="pq1">first priority queue</param>
        /// <param name="pq2">second priority queue</param>
        /// <param name="comparer">comparer for resultant priority queue</param>
        /// <returns>resultant priority queue</returns>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static PriorityQueue<TPriority, TValue> MergeQueues(PriorityQueue<TPriority, TValue> pq1,
            PriorityQueue<TPriority, TValue> pq2, IComparer<TPriority> comparer)
        {
            if (pq1 == null)
                throw new ArgumentNullException("pq1");

            if (pq2 == null)
                throw new ArgumentNullException("pq2");

            if (comparer == null)
                throw new ArgumentNullException("comparer");
            // merge data
            var result = new PriorityQueue<TPriority, TValue>(pq1.Count + pq2.Count, pq1._comparer);
            result._baseHeap.AddRange(pq1._baseHeap);
            result._baseHeap.AddRange(pq2._baseHeap);
            // heapify data
            for (var pos = result._baseHeap.Count/2 - 1; pos >= 0; pos--)
                result.HeapifyFromBeginningToEnd(pos);

            return result;
        }

        #endregion

        #region Priority queue operations

        /// <summary>
        ///     Gets whether priority queue is empty
        /// </summary>
        public bool IsEmpty
        {
            get { return _baseHeap.Count == 0; }
        }


        /// <summary>
        ///     Enqueues element into priority queue
        /// </summary>
        /// <param name="priority">element priority</param>
        /// <param name="value">element value</param>
        public void Enqueue(TPriority priority, TValue value)
        {
            Insert(priority, value);
        }


        /// <summary>
        ///     Dequeues element with minimum priority and return its priority and value as
        ///     <see cref="KeyValuePair{TPriority,TValue}" />
        /// </summary>
        /// <returns>priority and value of the dequeued element</returns>
        /// <remarks>
        ///     Method throws <see cref="InvalidOperationException" /> if priority queue is empty
        /// </remarks>
        public KeyValuePair<TPriority, TValue> Dequeue()
        {
            if (!IsEmpty)
            {
                var result = _baseHeap[0];
                DeleteRoot();
                return result;
            }
            throw new InvalidOperationException("Priority queue is empty");
        }


        /// <summary>
        ///     Dequeues element with minimum priority and return its value
        /// </summary>
        /// <returns>value of the dequeued element</returns>
        /// <remarks>
        ///     Method throws <see cref="InvalidOperationException" /> if priority queue is empty
        /// </remarks>
        public TValue DequeueValue()
        {
            return Dequeue().Value;
        }


        /// <summary>
        ///     Returns priority and value of the element with minimum priority, without removing it from the queue
        /// </summary>
        /// <returns>priority and value of the element with minimum priority</returns>
        /// <remarks>
        ///     Method throws <see cref="InvalidOperationException" /> if priority queue is empty
        /// </remarks>
        public KeyValuePair<TPriority, TValue> Peek()
        {
            if (!IsEmpty)
                return _baseHeap[0];
            throw new InvalidOperationException("Priority queue is empty");
        }


        /// <summary>
        ///     Returns value of the element with minimum priority, without removing it from the queue
        /// </summary>
        /// <returns>value of the element with minimum priority</returns>
        /// <remarks>
        ///     Method throws <see cref="InvalidOperationException" /> if priority queue is empty
        /// </remarks>
        public TValue PeekValue()
        {
            return Peek().Value;
        }

        #endregion

        #region Heap operations

        private void ExchangeElements(int pos1, int pos2)
        {
            var val = _baseHeap[pos1];
            _baseHeap[pos1] = _baseHeap[pos2];
            _baseHeap[pos2] = val;
        }


        private void Insert(TPriority priority, TValue value)
        {
            var val = new KeyValuePair<TPriority, TValue>(priority, value);
            _baseHeap.Add(val);

            // heap[i] have children heap[2*i + 1] and heap[2*i + 2] and parent heap[(i-1)/ 2];

            // heapify after insert, from end to beginning
            HeapifyFromEndToBeginning(_baseHeap.Count - 1);
        }


        private int HeapifyFromEndToBeginning(int pos)
        {
            if (pos >= _baseHeap.Count) return -1;

            while (pos > 0)
            {
                var parentPos = (pos - 1)/2;
                if (_comparer.Compare(_baseHeap[parentPos].Key, _baseHeap[pos].Key) > 0)
                {
                    ExchangeElements(parentPos, pos);
                    pos = parentPos;
                }
                else break;
            }
            return pos;
        }


        private void DeleteRoot()
        {
            if (_baseHeap.Count <= 1)
            {
                _baseHeap.Clear();
                return;
            }

            _baseHeap[0] = _baseHeap[_baseHeap.Count - 1];
            _baseHeap.RemoveAt(_baseHeap.Count - 1);

            // heapify
            HeapifyFromBeginningToEnd(0);
        }


        private void HeapifyFromBeginningToEnd(int pos)
        {
            if (pos >= _baseHeap.Count) return;

            // heap[i] have children heap[2*i + 1] and heap[2*i + 2] and parent heap[(i-1)/ 2];

            while (true)
            {
                // on each iteration exchange element with its smallest child
                var smallest = pos;
                var left = 2*pos + 1;
                var right = 2*pos + 2;
                if (left < _baseHeap.Count && _comparer.Compare(_baseHeap[smallest].Key, _baseHeap[left].Key) > 0)
                    smallest = left;
                if (right < _baseHeap.Count && _comparer.Compare(_baseHeap[smallest].Key, _baseHeap[right].Key) > 0)
                    smallest = right;

                if (smallest != pos)
                {
                    ExchangeElements(smallest, pos);
                    pos = smallest;
                }
                else break;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TPriority, TValue>> implementation

        /// <summary>
        ///     Enqueues element into priority queue
        /// </summary>
        /// <param name="item">element to add</param>
        public void Add(KeyValuePair<TPriority, TValue> item)
        {
            Enqueue(item.Key, item.Value);
        }


        /// <summary>
        ///     Clears the collection
        /// </summary>
        public void Clear()
        {
            _baseHeap.Clear();
        }


        /// <summary>
        ///     Determines whether the priority queue contains a specific element
        /// </summary>
        /// <param name="item">The object to locate in the priority queue</param>
        /// <returns><c>true</c> if item is found in the priority queue; otherwise, <c>false.</c> </returns>
        public bool Contains(KeyValuePair<TPriority, TValue> item)
        {
            return _baseHeap.Contains(item);
        }


        /// <summary>
        ///     Gets number of elements in the priority queue
        /// </summary>
        public int Count
        {
            get { return _baseHeap.Count; }
        }


        /// <summary>
        ///     Copies the elements of the priority queue to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional Array that is the destination of the elements copied from the priority queue.
        ///     The Array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <remarks>
        ///     It is not guaranteed that items will be copied in the sorted order.
        /// </remarks>
        public void CopyTo(KeyValuePair<TPriority, TValue>[] array, int arrayIndex)
        {
            _baseHeap.CopyTo(array, arrayIndex);
        }


        /// <summary>
        ///     Gets a value indicating whether the collection is read-only.
        /// </summary>
        /// <remarks>
        ///     For priority queue this property returns <c>false</c>.
        /// </remarks>
        public bool IsReadOnly
        {
            get { return false; }
        }


        /// <summary>
        ///     Removes the first occurrence of a specific object from the priority queue.
        /// </summary>
        /// <param name="item">The object to remove from the ICollection </param>
        /// <returns>
        ///     <c>true</c> if item was successfully removed from the priority queue.
        ///     This method returns false if item is not found in the collection.
        /// </returns>
        public bool Remove(KeyValuePair<TPriority, TValue> item)
        {
            // find element in the collection and remove it
            var elementIdx = _baseHeap.IndexOf(item);
            if (elementIdx < 0) return false;

            //remove element
            _baseHeap[elementIdx] = _baseHeap[_baseHeap.Count - 1];
            _baseHeap.RemoveAt(_baseHeap.Count - 1);

            // heapify
            var newPos = HeapifyFromEndToBeginning(elementIdx);
            if (newPos == elementIdx)
                HeapifyFromBeginningToEnd(elementIdx);

            return true;
        }


        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>Enumerator</returns>
        /// <remarks>
        ///     Returned enumerator does not iterate elements in sorted order.
        /// </remarks>
        public IEnumerator<KeyValuePair<TPriority, TValue>> GetEnumerator()
        {
            return _baseHeap.GetEnumerator();
        }


        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>Enumerator</returns>
        /// <remarks>
        ///     Returned enumerator does not iterate elements in sorted order.
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}