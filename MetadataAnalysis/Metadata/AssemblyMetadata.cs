using System;
using System.Collections.Immutable;
using System.Reflection;

namespace MetadataAnalysis.Metadata
{
    public class AssemblyMetadata
    {
        public AssemblyMetadata(
            string name,
            Version version,
            string culture,
            AssemblyFlags flags,
            ImmutableArray<byte> publicKey,
            AssemblyHashAlgorithm hashAlgorithm,
            IImmutableList<CustomAttributeMetadata> customAttributes,
            IImmutableDictionary<string, DefinedTypeMetadata> typeDefinitions
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

        public IImmutableList<CustomAttributeMetadata> CustomAttributes { get; }

        public IImmutableDictionary<string, DefinedTypeMetadata> TypeDefinitions { get; }
    }
}