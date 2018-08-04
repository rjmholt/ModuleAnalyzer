using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace MetadataAnalysis
{
    internal interface IDllPathCacheEntry
    {
        bool TryGetDllPath(string dllName, out FileInfo dllFile);
    }

    internal class DllFileCacheEntry : IDllPathCacheEntry
    {

        private readonly FileInfo _dllFile;

        private string _dllName;

        public DllFileCacheEntry(string dllPath)
        {
            _dllFile = new FileInfo(dllPath);
        }

        internal string DllName
        {
            get
            {
                if (_dllName == null)
                {
                    using (FileStream dllFileStream = _dllFile.OpenRead())
                    using (var peReader = new PEReader(dllFileStream))
                    {
                        MetadataReader mdReader = peReader.GetMetadataReader();
                        AssemblyDefinition asmDef = mdReader.GetAssemblyDefinition();
                        _dllName = mdReader.GetString(asmDef.Name);
                    }
                }
                return _dllName;
            }
        }

        public bool TryGetDllPath(string dllName, out FileInfo dllFile)
        {
            if (String.Equals(dllName, DllName))
            {
                dllFile = _dllFile;
                return true;
            }

            dllFile = null;
            return false;
        }
    }

    internal class DllDirectoryCacheEntry : IDllPathCacheEntry
    {
        private bool _initialized;

        private readonly DirectoryInfo _dir;

        private IDictionary<string, DllFileCacheEntry> _directoryDllFiles;

        public DllDirectoryCacheEntry(string dirPath)
        {
            _initialized = false;
            _dir = new DirectoryInfo(dirPath);
        }

        private IDictionary<string, DllFileCacheEntry> DirectoryDllFiles
        {
            get
            {
                if (_directoryDllFiles == null)
                {
                    _directoryDllFiles = new Dictionary<string, DllFileCacheEntry>();
                }
                return _directoryDllFiles;
            }
        }

        public bool TryGetDllPath(string dllName, out FileInfo dllFile)
        {
            if (!_initialized)
            {
                foreach (FileInfo file in _dir.EnumerateFiles("*.dll"))
                {
                    var fileEntry = new DllFileCacheEntry(file.Name);
                    DirectoryDllFiles.Add(fileEntry.DllName, fileEntry);
                }
                _initialized = true;
            }

            if (DirectoryDllFiles.TryGetValue(dllName, out DllFileCacheEntry fileCacheEntry))
            {
                return fileCacheEntry.TryGetDllPath(dllName, out dllFile);
            }

            dllFile = null;
            return false;
        }
    }

    public class DllPathCache
    {
        private readonly IImmutableList<string> _searchPaths;

        private IList<IDllPathCacheEntry> _cacheEntries;

        public DllPathCache(IImmutableList<string> searchPaths)
        {
            _searchPaths = searchPaths ?? ImmutableArray<string>.Empty;
        }

        private IEnumerable<IDllPathCacheEntry> CacheEntries
        {
            get
            {
                if (_searchPaths.Count == 0)
                {
                    return Enumerable.Empty<IDllPathCacheEntry>();
                }

                if (_cacheEntries == null)
                {
                    _cacheEntries = new List<IDllPathCacheEntry>();
                    foreach (string path in _searchPaths)
                    {
                        FileAttributes attrs = File.GetAttributes(path);

                        if (attrs.HasFlag(FileAttributes.Directory))
                        {
                            _cacheEntries.Add(new DllDirectoryCacheEntry(path));
                        }
                        else
                        {
                            _cacheEntries.Add(new DllFileCacheEntry(path));
                        }
                    }

                }
                return _cacheEntries;
            }
        }

        public bool TryFindDll(string dllName, out FileInfo dllFile)
        {
            foreach (IDllPathCacheEntry cacheEntry in CacheEntries)
            {
                if (cacheEntry.TryGetDllPath(dllName, out dllFile))
                {
                    return true;
                }
            }

            dllFile = null;
            return false;
        }
    }
}