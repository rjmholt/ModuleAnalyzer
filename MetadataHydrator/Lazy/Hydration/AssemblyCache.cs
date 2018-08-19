using System.Collections.Generic;
using MetadataHydrator.Lazy.Metadata;

namespace MetadataHydrator.Lazy
{
    internal class AssemblyCache
    {
        private readonly IDictionary<string, IList<IAssemblyMetadata>> _pathTable;

        private readonly IDictionary<string, IAssemblyMetadata> _nameTable;

        private readonly IList<string> _paths;

        public AssemblyCache()
        {
            _pathTable = new Dictionary<string, IList<IAssemblyMetadata>>();
            _nameTable = new Dictionary<string, IAssemblyMetadata>();
            _paths = new List<string>();
        }

        public void AddAssembly(IAssemblyMetadata assemblyDefinition)
        {
            if (!_pathTable.ContainsKey(assemblyDefinition.Path))
            {
                _paths.Add(assemblyDefinition.Path);
                _pathTable.Add(assemblyDefinition.Path, new List<IAssemblyMetadata>());
            }

            _pathTable[assemblyDefinition.Path].Add(assemblyDefinition);
            _nameTable.Add(assemblyDefinition.Name, assemblyDefinition);
        }

        public bool TryGetAssemblyByName(string assemblyName, out IAssemblyMetadata assemblyDefinition)
        {
            if (!_nameTable.ContainsKey(assemblyName))
            {
                assemblyDefinition = null;
                return false;
            }

            assemblyDefinition = _nameTable[assemblyName];
            return true;
        }
    }
}