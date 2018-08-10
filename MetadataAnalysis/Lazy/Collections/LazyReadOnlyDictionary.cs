using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MetadataAnalysis.Lazy.Collections
{
    public abstract class LazyReadOnlyDictionary<K, V> : IReadOnlyDictionary<K, V>
    {
        private IDictionary<K, V> _dictionary;

        protected LazyReadOnlyDictionary()
        {
        }

        public V this[K key]
        {
            get
            {
                Evaluate();
                return _dictionary[key];
            }
        }

        public IEnumerable<K> Keys
        {
            get
            {
                Evaluate();
                return _dictionary.Keys;
            }
        }

        public IEnumerable<V> Values
        {
            get
            {
                Evaluate();
                return _dictionary.Values;
            }
        }

        public int Count
        {
            get
            {
                Evaluate();
                return _dictionary.Count;
            }
        }

        public bool ContainsKey(K key)
        {
            Evaluate();
            return _dictionary.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            Evaluate();
            return _dictionary.GetEnumerator();
        }

        public bool TryGetValue(K key, out V value)
        {
            Evaluate();
            return _dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            Evaluate();
            return _dictionary.GetEnumerator();
        }

        protected abstract IEnumerable<KeyValuePair<K, V>> Generate();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Evaluate()
        {
            if (_dictionary == null)
            {
                _dictionary = new Dictionary<K, V>();
                foreach (KeyValuePair<K, V> entry in Generate())
                {
                    _dictionary.Add(entry.Key, entry.Value);
                }
            }
        }
    }
}