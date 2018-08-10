using System;
using System.Collections.Generic;
using System.Reflection;

namespace MetadataAnalysis.Interface
{
    public interface IAssemblyMetadata
    {
        string Name { get; }

        Version Version { get; }

        string Culture { get; }

        AssemblyFlags AssemblyFlags { get; }

        IReadOnlyList<byte> PublicKey { get; }

        AssemblyHashAlgorithm HashAlgorithm { get; }

        IReadOnlyList<ICustomAttributeMetadata> CustomAttributes { get; }

        IReadOnlyDictionary<string, ITypeMetadata> DefinedTypes { get; }

        string Path { get; }

        IReadOnlyDictionary<string, IAssemblyMetadata> ReferencedAssemblies { get; }
    }
}