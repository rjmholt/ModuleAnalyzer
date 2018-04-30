using System;
using System.Reflection;
using System.Collections.Immutable;

namespace MetadataAnalysis.Metadata.Interface
{
    /// <summary>
    /// Describes the metadata on an assembly.
    /// </summary>
    public interface IAssemblyMetadata
    {
        /// <summary>
        /// The name of the assembly.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The version of the assembly.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// The culture of the assembly.
        /// </summary>
        string Culture { get; }

        /// <summary>
        /// Any flags on the assembly.
        /// </summary>
        AssemblyFlags Flags { get; }

        /// <summary>
        /// The public key on the assembly if any.
        /// </summary>
        ImmutableArray<byte> PublicKey { get; }

        /// <summary>
        /// The hash algorithm used for the assembly.
        /// </summary>
        AssemblyHashAlgorithm HashAlgorithm { get; }

        /// <summary>
        /// Any custom attributes on the assembly.
        /// </summary>
        IImmutableList<ICustomAttributeMetadata> CustomAttributes { get; }

        /// <summary>
        /// The types defined in the assembly.
        /// </summary>
        IImmutableDictionary<string, ITypeMetadata> TypeDefinitions { get; }
    }
}