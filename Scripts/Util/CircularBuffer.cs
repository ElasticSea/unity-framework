using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class CircularBuffer<T>: IEnumerable<T>
    {
        private readonly T[] buffer;
        private int nextIndex;

        public CircularBuffer(int capacity)
        {
            buffer = new T[capacity];
            Capacity = capacity;
        }

        public int Capacity { get; }

        public int Count { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            var isHalfFilled = Count < Capacity;
            if (isHalfFilled)
            {
                for (var index = 0; index < nextIndex; index++)
                {
                    yield return buffer[index];
                }
            }
            else
            {
                for (var i = 0; i < Capacity; i++)
                {
                    var index = (i + nextIndex) % Capacity;
                    yield return buffer[index];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T element)
        {
            buffer[nextIndex] = element;
            Count = Mathf.Min(Count + 1, Capacity);
            nextIndex = (nextIndex + 1) % Capacity;
        }
    }
}