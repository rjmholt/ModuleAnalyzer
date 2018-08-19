using System;
using System.Collections.Generic;
using System.Reflection;

namespace MetadataHydrator
{
    public interface IAssemblyMetadata
    {
        string Culture { get; }

        AssemblyFlags Flags { get; }

        AssemblyHashAlgorithm HashAlgorithm { get; }

        string Name { get; }

        IReadOnlyCollection<byte> PublicKey { get; }

        Version Version { get; }

        IReadOnlyDictionary<string, IAssemblyMetadata> RequiredAssemblies { get; }

        string Path { get; }

        IReadOnlyDictionary<string, ITypeMetadata> DefinedTypes { get; }

        IReadOnlyCollection<ICustomAttributeMetadata> CustomAttributes { get; }
    }
}