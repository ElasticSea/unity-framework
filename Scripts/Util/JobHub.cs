using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticSea.Framework.Util
{
    public sealed class JobHub
    {
        private readonly ConcurrentDictionary<string, Lazy<Task<object>>> _inFlight = new();
        private readonly SemaphoreSlim _pool;

        /// <param name="maxConcurrency">
        /// Optional limiter for how many jobs can start at once.  
        /// Set 0 or less to disable the limit.
        /// </param>
        public JobHub(int maxConcurrency = 0)
        {
            _pool = (maxConcurrency > 0)
                ? new SemaphoreSlim(maxConcurrency, maxConcurrency)
                : new SemaphoreSlim(int.MaxValue, int.MaxValue);
        }

        /// <summary>
        /// Runs or returns an existing async job identified by <paramref name="key"/>.
        /// You decide how the work executes (inline, Task.Run, job system, etc.).
        /// </summary>
        public Task<T> GetOrRun<T>(string key, Func<Task<T>> factory)
        {
            var lazy = _inFlight.GetOrAdd(key, _ =>
                new Lazy<Task<object>>(async () =>
                {
                    await _pool.WaitAsync();
                    try
                    {
                        var result = await factory();
                        return (object)result!;
                    }
                    finally
                    {
                        _pool.Release();
                    }
                }, isThreadSafe: true));

            return AwaitAndCleanup<T>(key, lazy);
        }

        private async Task<T> AwaitAndCleanup<T>(string key, Lazy<Task<object>> lazy)
        {
            try
            {
                var obj = await lazy.Value;
                return (T)obj;
            }
            catch
            {
                // Remove failed job so it can be retried later
                _inFlight.TryRemove(key, out _);
                throw;
            }
        }

        /// <summary>Removes a finished or invalid entry so it can be recomputed.</summary>
        public void Invalidate(string key) => _inFlight.TryRemove(key, out _);

        /// <summary>Clears all entries.</summary>
        public void Clear() => _inFlight.Clear();

        /// <summary>Try to peek an existing job if it’s already started.</summary>
        public bool TryGetExisting<T>(string key, out Task<T>? task)
        {
            if (_inFlight.TryGetValue(key, out var lazy) && lazy.IsValueCreated)
            {
                task = lazy.Value.ContinueWith(
                    t => (T)t.Result,
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
                return true;
            }

            task = null;
            return false;
        }
    }
}
