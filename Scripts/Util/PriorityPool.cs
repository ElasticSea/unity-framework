using System;

namespace ElasticSea.Framework.Util
{
    public class PriorityPool<T>
    {
        private readonly Func<T> creator;
        private readonly PriorityQueue<T, T> stack = new();
        private readonly Action<T> activate;
        private readonly Action<T> deactivate;

        public PriorityPool(int initialCapacity, Func<T> creator, Action<T> activate = null, Action<T> deactivate = null)
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

            var element = stack.Dequeue();
            activate?.Invoke(element);
            return element;
        }

        public void Put(T element)
        {
            stack.Enqueue(element, element);
            deactivate?.Invoke(element);
        }
    }
}