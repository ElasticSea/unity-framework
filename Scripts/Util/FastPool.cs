using System;
using System.Collections.Generic;

namespace ElasticSea.Framework.Util
{
    public class FastPool<T>
    {
        private readonly Func<FastPool<T>, T> creator;
        
        private T[] pool;
        private int count;

        public FastPool(int initialCapacity, int maxCapacity, Func<FastPool<T>, T> creator)
        {
            pool = new T[maxCapacity];
            this.creator = creator;
            
            for (var i = 0; i < initialCapacity; i++)
            {
                Put(creator(this));
            }
        }

        public T Get()
        {
            if (count == 0)
            {
                Put(creator(this));
            }

            return pool[--count];
        }

        public void Put(T element)
        {
            pool[count++] = element;
        }
    }
}