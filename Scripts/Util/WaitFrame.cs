using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace ElasticSea.Framework.Util
{
    public static class WaitFrame
    {
        private static bool _installed;
        private static List<(TaskCompletionSource<bool>, int)> _queue = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Install()
        {
            if (_installed) return;
            var loop = PlayerLoop.GetCurrentPlayerLoop();
            for (int i = 0; i < loop.subSystemList.Length; i++)
            {
                if (loop.subSystemList[i].type != typeof(Update)) continue;
                var parent = loop.subSystemList[i];
                var list = new List<PlayerLoopSystem>(parent.subSystemList ?? Array.Empty<PlayerLoopSystem>());
                list.Add(new PlayerLoopSystem { type = typeof(WaitFrame), updateDelegate = Tick });
                parent.subSystemList = list.ToArray();
                loop.subSystemList[i] = parent;
                PlayerLoop.SetPlayerLoop(loop);
                _installed = true;
                break;
            }
        }

        static void Tick()
        {
            // decrement frame counters and complete any that hit zero
            for (int i = _queue.Count - 1; i >= 0; i--)
            {
                var (tcs, frames) = _queue[i];
                frames--;
                if (frames <= 0)
                {
                    tcs.TrySetResult(true);
                    _queue.RemoveAt(i);
                }
                else
                {
                    _queue[i] = (tcs, frames);
                }
            }
        }

        /// <summary>
        /// Waits for exactly <paramref name="frames"/> update frames before completing.
        /// </summary>
        public static Task Wait(int frames = 1)
        {
            if (!_installed) Install();
            if (frames <= 0) return Task.CompletedTask;

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _queue.Add((tcs, frames));
            return tcs.Task;
        }
    }
}