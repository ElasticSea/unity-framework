using System;
using System.Collections.Generic;
using ElasticSea.Framework.Util;

namespace ElasticSea.Framework.Extensions
{
    public static class BiMapExtensions
    {
        /// <summary>
        /// Creates a BiMap from an IEnumerable according to specified key selector and value selector functions.
        /// Throws ArgumentException if duplicate keys or values are encountered.
        /// </summary>
        public static BiMap<TKey, TValue> ToBiMap<TSource, TKey, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TValue> valueSelector,
            IEqualityComparer<TKey>? keyComparer = null,
            IEqualityComparer<TValue>? valueComparer = null)
            where TKey : notnull
            where TValue : notnull
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            if (valueSelector == null) throw new ArgumentNullException(nameof(valueSelector));

            // Try to pre-allocate capacity if source is a collection to reduce resizing
            int capacity = source is ICollection<TSource> collection ? collection.Count : 0;

            var biMap = new BiMap<TKey, TValue>(capacity, keyComparer, valueComparer);

            foreach (var element in source)
            {
                var key = keySelector(element);
                var value = valueSelector(element);

                // TryAdd returns false if either Key or Value already exists
                if (!biMap.TryAdd(key, value))
                {
                    throw new ArgumentException($"Duplicate mapping detected. Key: '{key}', Value: '{value}'. BiMap requires unique keys AND unique values.");
                }
            }

            return biMap;
        }
    }
}