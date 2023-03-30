using System;
using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class FastPool<T>
    {
        private readonly Func<T> creator;
        
        private T[] pool;
        private int count;
        private int poolSize;

        public FastPool(int initialCapacity, Func<T> creator)
        {
            pool = new T[initialCapacity];
            poolSize = pool.Length;
            this.creator = creator;
            
            for (var i = 0; i < initialCapacity; i++)
            {
                Put(creator());
            }
        }

        public T Get()
        {
            if (count == 0)
            {
                return creator();
            }

            return pool[--count];
        }

        public void Put(T element)
        {
            if (count == poolSize)
            {
                var oldPool = pool;
                pool = new T[Mathf.Max(poolSize, 1) * 2];
                Array.Copy(oldPool, pool, poolSize);
                poolSize = pool.Length;
            }
            pool[count++] = element;
        }
    }
}