using System;
using System.Collections.Generic;
using System.IO;
using MetadataHydrator.Lazy.LoadedTypes;
using MetadataHydrator.Lazy.Metadata;

namespace MetadataHydrator.Lazy
{
    internal class AssemblyDirectoryResolver
    {
        private static readonly ISet<string> s_peExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".dll",
            ".exe"
        };

        public static AssemblyDirectoryResolver Create(
            AssemblyResolver assemblyResolver,
            LoadedTypeResolver loadedTypeResolver,
            FileInfo initialAssemblyFile)
        {
            var assembliesByName = new Dictionary<string, LazyAssemblyDefinitionMetadata>();
            var assembliesByFileName = new Dictionary<string, LazyAssemblyDefinitionMetadata>();
            var asmDirResolver = new AssemblyDirectoryResolver(
                initialAssemblyFile.Directory,
                assemblyResolver,
                loadedTypeResolver,
                assembliesByName,
                assembliesByFileName);

            LazyAssemblyHydrator assemblyHydrator = LazyAssemblyHydrator.Create(
                initialAssemblyFile,
                asmDirResolver);

            LazyAssemblyDefinitionMetadata initialAssembly = assemblyHydrator.Assembly;
            assembliesByName.Add(initialAssembly.Name, initialAssembly);
            assembliesByFileName.Add(initialAssembly.File.Name, initialAssembly);

            return asmDirResolver;
        }

        private readonly DirectoryInfo _directory;

        private readonly AssemblyResolver _assemblyResolver;

        private readonly LoadedTypeResolver _loadedTypeResolver;

        private IDictionary<string, LazyAssemblyDefinitionMetadata> _assembliesByName;

        private IDictionary<string, LazyAssemblyDefinitionMetadata> _assembliesByFileName;

        private bool _hasScannedDirectory;

        internal AssemblyDirectoryResolver(
            DirectoryInfo directory,
            AssemblyResolver assemblyResolver,
            LoadedTypeResolver loadedTypeResolver,
            IDictionary<string, LazyAssemblyDefinitionMetadata> assembliesByName,
            IDictionary<string, LazyAssemblyDefinitionMetadata> assembliesByFileName)
        {
            _hasScannedDirectory = false;
            _directory = directory;
            _assemblyResolver = assemblyResolver;
            _loadedTypeResolver = loadedTypeResolver;
            _assembliesByName = assembliesByName;
            _assembliesByFileName = assembliesByFileName;
        }

        public LazyAssemblyDefinitionMetadata GetAssemblyByFileName(string fileName)
        {
            do
            {
                if (_assembliesByFileName.TryGetValue(fileName, out LazyAssemblyDefinitionMetadata assembly))
                {
                    return assembly;
                }
            } while (ScanDirectory());

            throw new Exception($"Unable to find assembly with file name {fileName} in directory {_directory.FullName}");
        }

        public LazyAssemblyDefinitionMetadata GetAssemblyByName(string name)
        {
            do
            {
                if (_assembliesByName.TryGetValue(name, out LazyAssemblyDefinitionMetadata assembly))
                {
                    return assembly;
                }
            } while (ScanDirectory());

            throw new Exception($"Unable to find assembly with name {name} in directory {_directory.FullName}");
        }

        private bool ScanDirectory()
        {
            if (_hasScannedDirectory)
            {
                return false;
            }

            foreach (FileInfo file in _directory.GetFiles())
            {
                if (!s_peExtensions.Contains(file.Extension))
                {
                    continue;
                }

                LazyAssemblyHydrator assemblyHydrator = LazyAssemblyHydrator.Create(file, this);
                LazyAssemblyDefinitionMetadata assembly = assemblyHydrator.Assembly;

                _assembliesByFileName.Add(file.Name, assembly);
                _assembliesByName.Add(assembly.Name, assembly);
            }

            _hasScannedDirectory = true;
            return true;
        }
    }
}