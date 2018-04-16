using System;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using MetadataAnalysis.Metadata;
using MetadataAnalysis.Metadata.ILParse;

namespace MetadataAnalysis
{
    public class MetadataAnalyzer : IDisposable
    {
        private MetadataReader _mdReader;

        public MetadataAnalyzer(MetadataReader mdReader)
        {
            _mdReader = mdReader;
        }

        public IAssemblyMetadata GetDefinedAssembly()
        {
            return AssemblyILMetadata.FromMetadataReader(_mdReader);
        }

        public IImmutableList<ICustomAttributeMetadata> GetAssemblyCustomAttributes()
        {
            return null;
        }

        public IImmutableDictionary<string, ITypeDefinitionMetadata> GetAllTypeDefinitions()
        {
            return null;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}