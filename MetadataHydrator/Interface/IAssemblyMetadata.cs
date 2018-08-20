using System;
using System.Collections.Generic;
using System.IO;
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

        FileInfo File { get; }

        IReadOnlyDictionary<string, ITypeMetadata> TypeDefinitions { get; }

        IReadOnlyDictionary<string, ITypeMetadata> TypeReferences { get; }

        IReadOnlyCollection<ICustomAttributeMetadata> CustomAttributes { get; }
    }
}