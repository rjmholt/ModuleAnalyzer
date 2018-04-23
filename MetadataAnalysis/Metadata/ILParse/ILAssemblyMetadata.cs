using System;
using System.Collections.Immutable;
using System.Reflection;
using MetadataAnalysis.Metadata.Interface;

namespace MetadataAnalysis.Metadata.ILParse
{
    public class ILAssemblyMetadata : IAssemblyMetadata
    {
        public ILAssemblyMetadata(
            string name,
            Version version,
            string culture,
            AssemblyFlags flags,
            ImmutableArray<byte> publicKey,
            AssemblyHashAlgorithm hashAlgorithm,
            IImmutableList<ICustomAttributeMetadata> customAttributes,
            IImmutableDictionary<string, ITypeMetadata> typeDefinitions
        )
        {
            Name = name;
            Version = version;
            Culture = culture;
            Flags = flags;
            PublicKey = publicKey;
            HashAlgorithm = hashAlgorithm;
            CustomAttributes = customAttributes;
            TypeDefinitions = typeDefinitions;
        }

        public string Name { get; }

        public Version Version { get; }

        public string Culture { get; }

        public AssemblyFlags Flags { get; }

        public ImmutableArray<byte> PublicKey { get; }

        public AssemblyHashAlgorithm HashAlgorithm { get; }

        public IImmutableList<ICustomAttributeMetadata> CustomAttributes { get; }

        public IImmutableDictionary<string, ITypeMetadata> TypeDefinitions { get; }
    }
}