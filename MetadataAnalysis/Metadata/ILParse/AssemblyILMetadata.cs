using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Linq;
using System.Collections.Generic;

namespace MetadataAnalysis.Metadata.ILParse
{
    public class AssemblyILMetadata : IAssemblyMetadata
    {
        public static AssemblyILMetadata FromMetadataReader(MetadataReader mdReader)
        {
            AssemblyDefinition asmDef = mdReader.GetAssemblyDefinition();

            var customAttributes = CustomAttributeILMetadata.FromHandleCollectionWithMetadataReader(mdReader, mdReader.CustomAttributes)
                                                            .OfType<ICustomAttributeMetadata>()
                                                            .ToImmutableArray();

            var typeDefinitions = new Dictionary<string, ITypeMetadata>();
            foreach (TypeILMetadata typeDef in TypeILMetadata.FromMetadataReader(mdReader))
            {
                typeDefinitions.Add(typeDef.Name, typeDef);
            }

            return new AssemblyILMetadata()
            {
                Name             = mdReader.GetString(asmDef.Name),
                Version          = asmDef.Version,
                Culture          = mdReader.GetString(asmDef.Culture),
                Flags            = asmDef.Flags,
                HashAlgorithm    = asmDef.HashAlgorithm,
                PublicKey        = mdReader.GetBlobContent(asmDef.PublicKey),
                CustomAttributes = customAttributes,
                TypeDefinitions  = typeDefinitions.ToImmutableDictionary()
            };
        }

        public AssemblyILMetadata(
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

        public IImmutableDictionary<string, ITypeMetadata> TypeDefinitions { get; private set; }
    }
}