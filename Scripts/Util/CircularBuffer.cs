using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class CircularBuffer<T>: IEnumerable<T>
    {
        private readonly T[] buffer;
        private int nextIndex;
        private int slotsFilled;

        public CircularBuffer(int capacity)
        {
            buffer = new T[capacity];
        }

        public int Count => buffer.Length;

        public IEnumerator<T> GetEnumerator()
        {
            if (buffer.Length == slotsFilled)
            {
                for (var i = 0; i < buffer.Length; i++)
                {
                    yield return buffer[(i + nextIndex) % buffer.Length];
                }
            }
            else
            {
                for (var i = 0; i < nextIndex; i++)
                {
                    yield return buffer[i];
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
            slotsFilled = Mathf.Min(slotsFilled + 1, buffer.Length);
            nextIndex = (nextIndex + 1) % buffer.Length;
        }
    }
}