using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MetadataHydrator
{
    public interface IAssemblyDefinitionMetadata
    {
        string Culture { get; }

        AssemblyFlags Flags { get; }

        AssemblyHashAlgorithm HashAlgorithm { get; }

        string Name { get; }

        IReadOnlyCollection<byte> PublicKey { get; }

        Version Version { get; }

        IReadOnlyDictionary<string, IAssemblyDefinitionMetadata> RequiredAssemblies { get; }

        FileInfo File { get; }

        IReadOnlyDictionary<string, ITypeDefinitionMetadata> TypeDefinitions { get; }

        IReadOnlyDictionary<string, ITypeDefinitionMetadata> TypeReferences { get; }

        IReadOnlyCollection<ICustomAttributeMetadata> CustomAttributes { get; }
    }
}