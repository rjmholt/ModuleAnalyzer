using System;
using System.Collections.Immutable;
using System.Reflection;

namespace MetadataAnalysis.Interface
{
    public interface IAssemblyMetadata
    {
        string Name { get; }

        Version Version { get; }

        string Culture { get; }

        AssemblyFlags AssemblyFlags { get; }

        IImmutableList<byte> PublicKey { get; }

        AssemblyHashAlgorithm HashAlgorithm { get; }

        IImmutableList<ICustomAttributeMetadata> CustomAttributes { get; }

        IImmutableDictionary<string, ITypeMetadata> DefinedTypes { get; }
    }
}