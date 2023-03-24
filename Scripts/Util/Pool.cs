using System;
using System.Collections.Generic;

namespace ElasticSea.Framework.Util
{
    public class Pool<T>
    {
        private readonly Func<T> creator;
        private readonly Stack<T> stack = new Stack<T>();
        private readonly Action<T> activate;
        private readonly Action<T> deactivate;

        public Pool(int initialCapacity, Func<T> creator, Action<T> activate = null, Action<T> deactivate = null)
        {
            this.creator = creator;
            this.activate = activate;
            this.deactivate = deactivate;
            for (var i = 0; i < initialCapacity; i++) Put(creator());
        }

        public T Get()
        {
            if (stack.Count == 0)
            {
                return creator();
            }

            var element = stack.Pop();
            activate?.Invoke(element);
            return element;
        }

        public void Put(T element)
        {
            stack.Push(element);
            deactivate?.Invoke(element);
        }
    }
}