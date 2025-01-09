using System;
using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class FastPoolWithCallback<T>
    {
        private readonly Func<T> creator;
        private readonly Action<T> putCallback;
        private readonly Action<T> getCallback;
        
        private T[] pool;
        private int count;
        private int poolSize;

        public FastPoolWithCallback(int initialCapacity, Func<T> creator, Action<T> putCallback = null, Action<T> getCallback = null)
        {
            pool = new T[initialCapacity];
            poolSize = pool.Length;
            this.creator = creator;
            this.putCallback = putCallback;
            this.getCallback = getCallback;
            
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

            var element = pool[--count];
            getCallback?.Invoke(element);
            return element;
        }

        public void Put(T element)
        {
            putCallback?.Invoke(element);
            
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