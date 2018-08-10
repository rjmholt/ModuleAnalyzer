using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MetadataAnalysis.Lazy.Collections
{
    public abstract class LazyReadOnlyList<T> : IReadOnlyList<T>
    {
        private IList<T> _list;

        protected LazyReadOnlyList()
        {
        }

        public T this[int index]
        {
            get
            {
                Evaluate();
                return _list[index];
            }
        }

        public int Count
        {
            get
            {
                Evaluate();
                return _list.Count;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            Evaluate();
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            Evaluate();
            return _list.GetEnumerator();
        }

        protected abstract IEnumerable<T> Generate();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Evaluate()
        {
            if (_list == null)
            {
                _list = Generate().ToArray();
            }
        }
    }
}