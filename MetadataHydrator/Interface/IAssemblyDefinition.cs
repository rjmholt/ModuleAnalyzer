namespace MetadataHydrator
{
    public interface IAssemblyDefinition
    {
        string Culture { get; }

        AssemblyFlags Flags { get; }

        AssemblyHashAlgorithm HashAlgorithm { get; }

        string Name { get; }

        IReadOnlyCollection<byte> PublicKey { get; }

        Version Version { get; }

        IReadOnlyDictionary<string, IAssemblyDefinition> RequiredAssemblies { get; }

        string Path { get; }

        IReadOnlyDictionary<string, ITypeMetadata> DefinedTypes { get; }

        IReadOnlyCollection<ICustomAttribute> CustomAttributes { get; }
    }
}