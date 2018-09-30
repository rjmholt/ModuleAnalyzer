using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MetadataHydrator.Lazy.LoadedTypes
{
    public class LoadedAssemblyResolver
    {
        private readonly IDictionary<Assembly, LoadedAssemblyMetadata> _asmTable;

        public LoadedAssemblyResolver()
        {
            _asmTable = new Dictionary<Assembly, LoadedAssemblyMetadata>();
        }

        public bool TryGetAssemblyMetadata(string assemblyName, out LoadedAssemblyMetadata asmMetadata)
        {
            return TryGetAssemblyMetadata(new AssemblyName(assemblyName), out asmMetadata);
        }

        public bool TryGetAssemblyMetadata(
            string simpleName,
            out LoadedAssemblyMetadata asmMetadata,
            Version version = null,
            string culture = null,
            byte[] publicKeyToken = null,
            bool skipConstraints = false)
        {
            if (simpleName == null)
            {
                asmMetadata = null;
                return false;
            }

            if (skipConstraints)
            {
                return TryGetAssemblyMetadata(new AssemblyName(simpleName), out asmMetadata);
            }

            var sb = new StringBuilder(simpleName);

            if (version != null)
            {
                sb.Append(", Version=");
                sb.Append(version);
            }

            if (culture != null)
            {
                sb.Append(", Culture=");
                sb.Append(culture);
            }

            if (publicKeyToken != null)
            {
                sb.Append(", PublicKeyToken=");
                var tokenBuilder = new StringBuilder(publicKeyToken.Length * 2);
                foreach (byte b in publicKeyToken)
                {
                    tokenBuilder.AppendFormat("{0:x2}", b);
                }
                sb.Append(tokenBuilder);
            }

            var asmName = new AssemblyName(sb.ToString());
            return TryGetAssemblyMetadata(asmName, out asmMetadata);
        }

        public bool TryGetAssemblyMetadata(AssemblyName assemblyName, out LoadedAssemblyMetadata asmMetadata)
        {
            Assembly asm = null;
            try
            {
                asm = Assembly.Load(assemblyName);
            }
            catch (Exception)
            {
                // Do nothing -- just leave asm null
            }

            if (asm == null)
            {
                asmMetadata = null;
                return false;
            }

            if (!_asmTable.TryGetValue(asm, out LoadedAssemblyMetadata loadedAsmMetadata))
            {
                loadedAsmMetadata = new LoadedAssemblyMetadata(asm);
                _asmTable.Add(asm, loadedAsmMetadata);
            }

            asmMetadata = loadedAsmMetadata;
            return true;
        }

        public ITypeDefinitionMetadata ResolveType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (_asmTable.TryGetValue(type.Assembly, out LoadedAssemblyMetadata asm))
            {
                return asm.LookupType(type);
            }

            if (TryGetAssemblyMetadata(type.Assembly.GetName(), out LoadedAssemblyMetadata asmMetadata))
            {
                return asmMetadata.LookupType(type);
            }

            throw new Exception($"Unable to resolve assembly {type.Assembly.FullName} for type {type.FullName}");
        }
    }
}