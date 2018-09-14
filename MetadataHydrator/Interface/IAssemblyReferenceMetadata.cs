using System;
using System.Collections.Generic;
using System.Reflection;

namespace MetadataHydrator
{
    public interface IAssemblyReferenceMetadata
    {
        string Culture { get; }

        AssemblyFlags Flags { get; }

        string Name { get; }

        IReadOnlyCollection<byte> HashValue { get; }

        IReadOnlyCollection<byte> PublicKeyOrToken { get; }

        Version Version { get; }
    }
}