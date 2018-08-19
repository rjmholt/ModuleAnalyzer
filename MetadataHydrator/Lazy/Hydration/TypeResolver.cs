using System.Collections.Generic;
using MetadataHydrator.Lazy.Metadata;

namespace MetadataHydrator.Lazy
{
    internal class TypeResolver
    {
        private readonly IDictionary<string, LazyTypeMetadata> _typeMetadataCache;

        public TypeResolver()
        {
            _typeMetadataCache = new Dictionary<string, LazyTypeMetadata>();
        }

        public void AddType(LazyTypeMetadata typeMetadata)
        {
            _typeMetadataCache.Add(typeMetadata.FullName, typeMetadata);
        }

        public bool TryGetByName(string typeName, out LazyTypeMetadata type)
        {
            if (_typeMetadataCache.TryGetValue(typeName, out type))
            {
                return true;
            }

            type = null;
            return false;
        }
    }
}