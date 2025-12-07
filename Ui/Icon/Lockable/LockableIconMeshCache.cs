using System.Collections.Generic;

namespace ElasticSea.Framework.Ui.Icon.Lockable
{
    public class LockableIconMeshCache
    {
        private static Dictionary<string, LockableIconData> globalCacheInstance = new();

        private readonly Dictionary<string, LockableIconData> cache;
        private readonly string prefix;

        public LockableIconMeshCache(Dictionary<string, LockableIconData> cache = null, string prefix = null)
        {
            this.cache = cache ?? globalCacheInstance;
            this.prefix = prefix;
        }

        public LockableIconData GetCachedMeshData(LockableIconDataFactory factory)
        {
            if (cache != null && factory.Id != null )
            {
                var key = prefix + factory.Id;
                if (cache.TryGetValue(key, out var meshDatav))
                {
                    return meshDatav;
                }
            }

            return factory.Factory();
        }

        public void SetCachedMeshData(string id, LockableIconData data)
        {
            if (cache != null && id != null)
            {
                var key = prefix + id;
                cache[key] = data;
            }
        }
    }
}