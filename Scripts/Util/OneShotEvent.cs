using System;
using Blocks;

namespace ElasticSea.Framework.Scripts.Util
{
    public class OneShotEvent<T>
    {
        private event Action<T> internalEvent;

        public event Action<T> SomeEvent = obj => { };

        public OneShotEvent()
        {
            internalEvent += obj =>
            {
                foreach (Action<T> del in SomeEvent.GetInvocationList())
                {
                    del.DynamicInvoke(obj);
                    SomeEvent -= del;
                }
            };
        }

        public void Invoke(T value)
        {
            internalEvent.Invoke(value);
        }
    }
}