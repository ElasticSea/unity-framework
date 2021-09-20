using System;
using System.Collections.Generic;
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
    }
}