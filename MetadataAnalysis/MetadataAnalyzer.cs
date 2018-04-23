using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using MetadataAnalysis.Metadata;
using MetadataAnalysis.Metadata.ILParse;
using MetadataAnalysis.Metadata.Interface;

namespace MetadataAnalysis
{
    public class MetadataAnalyzer : IDisposable
    {
        private PEReader _peReader;
        private MetadataReader _mdReader;

        public MetadataAnalyzer(PEReader peReader)
        {
            _peReader = peReader;
            _mdReader = _peReader.GetMetadataReader();
        }

        public IAssemblyMetadata GetDefinedAssembly()
        {
            AssemblyDefinition asmDef = _mdReader.GetAssemblyDefinition();
            
            string name = _mdReader.GetString(asmDef.Name);
            string culture = _mdReader.GetString(asmDef.Culture);
            ImmutableArray<byte> publicKey = _mdReader.GetBlobContent(asmDef.PublicKey);

            return new ILAssemblyMetadata(
                name,
                asmDef.Version,
                culture,
                asmDef.Flags,
                publicKey,
                asmDef.HashAlgorithm,
                GetAssemblyCustomAttributes(),
                GetAllTypeDefinitions()
            );
        }

        public IImmutableList<ICustomAttributeMetadata> GetAssemblyCustomAttributes()
        {
            return null;
        }

        public IImmutableDictionary<string, ITypeMetadata> GetAllTypeDefinitions()
        {
            return ReadTypesFromHandles(_mdReader.TypeDefinitions);
        }

        public IImmutableDictionary<string, ITypeMetadata> GetTypeDefinitions(ICollection<string> typeNames)
        {
            return null;
        }

        public ITypeMetadata GetTypeDefinition(string typeName)
        {
            return null;
        }

        private IImmutableDictionary<string, ITypeMetadata> ReadTypesFromHandles(IEnumerable<TypeDefinitionHandle> tdHandles)
        {
            var typeDefs = new Dictionary<string, ITypeMetadata>();
            foreach (TypeDefinitionHandle tdHandle in tdHandles)
            {
                TypeDefinition typeDef = _mdReader.GetTypeDefinition(tdHandle);
                ITypeMetadata typeMetadata = ReadTypeMetadata(typeDef);
                typeDefs.Add(typeMetadata.Name, typeMetadata);
            }
            return typeDefs.ToImmutableDictionary();
        }

        private ILTypeMetadata ReadTypeMetadata(TypeDefinition typeDef)
        {
            string name = _mdReader.GetString(typeDef.Name);
            string @namespace = _mdReader.GetString(typeDef.Namespace);
            string namespaceDefinition = _mdReader.GetString(typeDef.NamespaceDefinition);

            ProtectionLevel protectionLevel = ReadProtectionLevel(typeDef.Attributes);
            ITypeMetadata baseType = ReadBaseType(typeDef.BaseType);
            bool isAbstract = ReadAbstract(typeDef.Attributes);
            bool isSealed = ReadSealed(typeDef.Attributes);
            bool isStatic = isAbstract && isSealed;

            return new ILClassMetadata(
                name,
                @namespace,
                protectionLevel,
                baseType,
                null,
                null,
                null,
                null,
                ReadTypesFromHandles(typeDef.GetNestedTypes()),
                isAbstract,
                isSealed
            );
        }

        private ProtectionLevel ReadProtectionLevel (TypeAttributes typeAttributes)
        {
            switch (typeAttributes & TypeAttributes.VisibilityMask)
            {
                case TypeAttributes.Public:
                case TypeAttributes.NestedPublic:
                    return ProtectionLevel.Public;

                case TypeAttributes.NestedAssembly:
                    return ProtectionLevel.Internal;

                case TypeAttributes.NestedFamily:
                    return ProtectionLevel.Protected;

                case TypeAttributes.NestedPrivate:
                    return ProtectionLevel.Private;

                case TypeAttributes.NotPublic:
                    return ProtectionLevel.Internal;

                default:
                    throw new Exception(String.Format("Unknown protection level: '{0}'", typeAttributes & TypeAttributes.VisibilityMask));
            }
        }

        private ITypeMetadata ReadBaseType(EntityHandle baseTypeHandle)
        {
            return null;
        }

        private bool ReadAbstract(TypeAttributes typeAttributes)
        {
            return (int)(typeAttributes & TypeAttributes.Abstract) != 0;
        }

        private bool ReadSealed(TypeAttributes typeAttributes)
        {
            return (int)(typeAttributes & TypeAttributes.Sealed) != 0;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _peReader.Dispose();
                }

                _mdReader = null;
                _peReader = null;
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}