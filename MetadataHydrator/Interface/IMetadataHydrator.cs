namespace MetadataHydrator
{
    public interface IMetadataHydrator
    {
        IAssemblyDefinition ReadAssembly(string asmPath);
    }
}