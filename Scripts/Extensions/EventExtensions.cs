using System;
using UnityEngine;

namespace ElasticSea.Framework.Extensions
{
    public static class EventExtensions
    {
        public static void RaiseSafe(this Action evt)
        {
            var snapshot = evt; 
            if (snapshot == null) return;

            foreach (Delegate d in snapshot.GetInvocationList())
            {
                try
                {
                    ((Action)d).Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError(
                        $"[Event Exception] Handler {d.Method.DeclaringType}.{d.Method.Name} threw:\n{ex}");
                }
            }
        }

        public static void RaiseSafe<T>(this Action<T> evt, T arg)
        {
            var snapshot = evt;
            if (snapshot == null) return;

            foreach (Delegate d in snapshot.GetInvocationList())
            {
                try
                {
                    ((Action<T>)d).Invoke(arg);
                }
                catch (Exception ex)
                {
                    Debug.LogError(
                        $"[Event Exception] Handler {d.Method.DeclaringType}.{d.Method.Name} threw:\n{ex}");
                }
            }
        }
    }
}