using System.Collections;
using System.Collections.Generic;

namespace MetadataHydrator.Lazy.Utilities
{
    public struct SingletonReadOnlyCollection<T> : IReadOnlyCollection<T>
    {
        public struct Enumerator : IEnumerator<T>
        {
            private readonly SingletonReadOnlyCollection<T> _singleton;

            private bool _hasEnumerated;

            public Enumerator(SingletonReadOnlyCollection<T> singleton)
            {
                _hasEnumerated = false;
                _singleton = singleton;
            }

            public T Current => _singleton._item;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_hasEnumerated || _singleton._item == null)
                {
                    return false;
                }

                _hasEnumerated = true;
                return true;
            }

            public void Reset()
            {
                _hasEnumerated = false;
            }
        }

        private readonly T _item;

        public SingletonReadOnlyCollection(T item)
        {
            _item = item;
        }

        public int Count => _item == null ? 0 : 1;

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }
    }
}