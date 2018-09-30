using System;
using System.Collections.Generic;

namespace MetadataHydrator.Lazy.Utilities
{
    public class DualKeyDictionary<K1, K2, V>
    {
        private readonly IDictionary<K1, K2> _keyToKeyTable;

        private readonly IDictionary<K2, V> _lookupTable;

        public DualKeyDictionary()
        {
            _keyToKeyTable = new Dictionary<K1, K2>();
            _lookupTable = new Dictionary<K2, V>();
        }

        public IEnumerable<K1> PrimaryKeys => _keyToKeyTable.Keys;

        public IEnumerable<K2> SecondaryKeys => _lookupTable.Keys;

        public IEnumerable<V> Values => _lookupTable.Values;

        public IEnumerable<KeyValuePair<K1, K2>> KeyPairs => _keyToKeyTable;

        public IEnumerable<KeyValuePair<K2, V>> SecondaryKeyValues => _lookupTable;

        public IEnumerable<KeyValuePair<K1, V>> PrimaryKeyValues
        {
            get
            {
                foreach (KeyValuePair<K1, K2> keyPair in KeyPairs)
                {
                    yield return new KeyValuePair<K1, V>(keyPair.Key, _lookupTable[keyPair.Value]);
                }
            }
        }

        public V this[K1 primaryKey]
        {
            get
            {
                if (!_keyToKeyTable.TryGetValue(primaryKey, out K2 secondaryKey))
                {
                    throw new KeyNotFoundException();
                }

                if (!_lookupTable.TryGetValue(secondaryKey, out V value))
                {
                    throw new KeyNotFoundException($"Secondary key {secondaryKey} not found");
                }

                return value;
            }
        }

        public V this[K2 secondaryKey]
        {
            get
            {
                if (!_lookupTable.TryGetValue(secondaryKey, out V value))
                {
                    throw new KeyNotFoundException();
                }

                return value;
            }
        }

        public V this[K1 primaryKey, K2 secondaryKey]
        {
            set
            {
                _keyToKeyTable[primaryKey] = secondaryKey;
                _lookupTable[secondaryKey] = value;
            }
        }

        public void Add(K1 k1, K2 k2, V v)
        {
            _keyToKeyTable.Add(k1, k2);
            _lookupTable.Add(k2, v);
        }

        public void Add(K1 k1, Func<K1, K2> f, V v)
        {
            K2 k2 = f(k1);
            Add(k1, k2, v);
        }

        public void Add(K2 k2, Func<K2, K1> f, V v)
        {
            K1 k1 = f(k2);
            Add(k1, k2, v);
        }

        public V Get(K1 k1)
        {
            if (!_keyToKeyTable.TryGetValue(k1, out K2 k2))
            {
                throw new KeyNotFoundException();
            }

            return _lookupTable[k2];
        }

        public V Get(K2 k2)
        {
            if (!_lookupTable.TryGetValue(k2, out V v))
            {
                throw new KeyNotFoundException();
            }

            return v;
        }

        public bool TryGetValue(K1 key, out V value)
        {
            value = default(V);
            return _keyToKeyTable.TryGetValue(key, out K2 secondaryKey)
                && _lookupTable.TryGetValue(secondaryKey, out value);
        }

        public bool ContainsKey(K1 key)
        {
            return _keyToKeyTable.ContainsKey(key);
        }

        public bool ContainsKey(K2 key)
        {
            return _lookupTable.ContainsKey(key);
        }
    }
}