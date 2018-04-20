using System;
using System.Reflection;
using System.Collections.Immutable;

namespace MetadataAnalysis.Metadata
{
    public interface IAssemblyMetadata
    {
        string Name { get; }

        Version Version { get; }

        string Culture { get; }

        AssemblyFlags Flags { get; }

        ImmutableArray<byte> PublicKey { get; }

        AssemblyHashAlgorithm HashAlgorithm { get; }

        IImmutableList<ICustomAttributeMetadata> CustomAttributes { get; }

        IImmutableDictionary<string, ITypeMetadata> TypeDefinitions { get; }
    }
}