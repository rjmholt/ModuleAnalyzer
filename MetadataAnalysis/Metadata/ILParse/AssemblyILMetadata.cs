using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;

namespace MetadataAnalysis.Metadata.ILParse
{
    public class AssemblyILMetadata : IAssemblyMetadata
    {
        public static AssemblyILMetadata FromMetadataReader(MetadataReader mdReader)
        {
            AssemblyDefinition asmDef = mdReader.GetAssemblyDefinition();
            string name = mdReader.GetString(asmDef.Name);
            Version version = asmDef.Version;
            string culture = mdReader.GetString(asmDef.Culture);
            AssemblyFlags flags = asmDef.Flags;
            AssemblyHashAlgorithm hashAlgorithm = asmDef.HashAlgorithm;
            ImmutableArray<byte> publicKey = mdReader.GetBlobContent(asmDef.PublicKey);
            IImmutableList<ICustomAttributeMetadata> customAttributes = GetAssemblyCustomAttributes();
            IImmutableDictionary<string, ITypeDefinitionMetadata> typeDefinitions = GetAllTypeDefinitions();

            return new AssemblyILMetadata(
                name,
                version,
                culture,
                flags,
                publicKey,
                hashAlgorithm,
                customAttributes,
                typeDefinitions
            );
        }

        public AssemblyILMetadata(
            string name,
            Version version,
            string culture,
            AssemblyFlags flags,
            ImmutableArray<byte> publicKey,
            AssemblyHashAlgorithm hashAlgorithm,
            IImmutableList<ICustomAttributeMetadata> customAttributes,
            IImmutableDictionary<string, ITypeDefinitionMetadata> typeDefinitions
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

        private AssemblyILMetadata()
        {
        }

        public string Name { get; private set; }

        public Version Version { get; private set; }

        public string Culture { get; private set; }

        public AssemblyFlags Flags { get; private set; }

        public ImmutableArray<byte> PublicKey { get; private set; }

        public AssemblyHashAlgorithm HashAlgorithm { get; private set; }

        public IImmutableList<ICustomAttributeMetadata> CustomAttributes { get; private set; }

        public IImmutableDictionary<string, ITypeDefinitionMetadata> TypeDefinitions { get; private set; }
    }
}