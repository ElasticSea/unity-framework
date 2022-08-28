using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class CircularBuffer<T> : IEnumerable<T>
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
            int start;
            int count;

            var underFilled = Count < Capacity;
            if (underFilled)
            {
                start = 0;
                count = nextIndex;
            }
            else
            {
                start = nextIndex;
                count = start + Capacity;
            }

            for (var i = start; i < count; i++)
            {
                yield return buffer[i % Capacity];
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