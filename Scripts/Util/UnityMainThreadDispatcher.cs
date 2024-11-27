using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class UnityMainThreadDispatcher : MonoBehaviour
    {
        private readonly Queue<Action> executionQueue = new Queue<Action>();

        private void Update()
        {
            while (executionQueue.Count > 0)
            {
                executionQueue.Dequeue().Invoke();
            }
        }

        public void Enqueue(Action action)
        {
            executionQueue.Enqueue(action);
        }

        public Task<T> EnqueueAsync<T>(Func<T> action)
        {
            var tcs = new TaskCompletionSource<T>();
            executionQueue.Enqueue(() =>
            {
                try
                {
                    var result = action();
                    tcs.SetResult(result);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
            return tcs.Task;
        }
    }
}