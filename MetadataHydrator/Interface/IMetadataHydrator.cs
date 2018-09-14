using System.IO;

namespace MetadataHydrator
{
    public interface IMetadataHydrator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        /// <remarks>
        /// The API must take a path in some way, since assembly resolution occurs based on paths.
        /// </remarks>
        IAssemblyDefinitionMetadata ReadAssembly(string assemblyPath);
    }
}