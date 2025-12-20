using System.Collections.Generic;

namespace ElasticSea.Framework.Util
{
    public sealed class BiMap<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
    {
        readonly Dictionary<TKey, TValue> _fwd;
        readonly Dictionary<TValue, TKey> _rev;

        public BiMap(int capacity = 0,
            IEqualityComparer<TKey>? keyComparer = null,
            IEqualityComparer<TValue>? valueComparer = null)
        {
            _fwd = new Dictionary<TKey, TValue>(capacity, keyComparer);
            _rev = new Dictionary<TValue, TKey>(capacity, valueComparer);
        }

        public int Count => _fwd.Count;
        
        public Dictionary<TKey, TValue>.KeyCollection Keys => _fwd.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => _fwd.Values;

        public bool TryGetByKey(TKey key, out TValue value) => _fwd.TryGetValue(key, out value!);
        public bool TryGetByValue(TValue value, out TKey key) => _rev.TryGetValue(value, out key!);
        
        public TValue GetByKey(TKey key) => _fwd[key];
        public TKey GetByValue(TValue value) => _rev[value];

        public bool TryAdd(TKey key, TValue value)
        {
            if (_fwd.ContainsKey(key) || _rev.ContainsKey(value))
                return false;

            _fwd.Add(key, value);
            _rev.Add(value, key);
            return true;
        }

        // Upsert that maintains 1:1 (removes any old links)
        public void Set(TKey key, TValue value)
        {
            if (_fwd.TryGetValue(key, out var oldValue))
                _rev.Remove(oldValue);

            if (_rev.TryGetValue(value, out var oldKey))
                _fwd.Remove(oldKey);

            _fwd[key] = value;
            _rev[value] = key;
        }

        public bool RemoveByKey(TKey key)
        {
            if (!_fwd.TryGetValue(key, out var value)) return false;
            _fwd.Remove(key);
            _rev.Remove(value);
            return true;
        }

        public bool RemoveByValue(TValue value)
        {
            if (!_rev.TryGetValue(value, out var key)) return false;
            _rev.Remove(value);
            _fwd.Remove(key);
            return true;
        }
    }
}