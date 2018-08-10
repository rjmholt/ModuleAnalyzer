using System;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using MetadataAnalysis.Interface;

namespace MetadataAnalysis.Lazy
{
    public class LazyMetadataAnalyzer : IMetadataAnalyzer, IDisposable
    {
        public LazyMetadataAnalyzer Create(string dllPath)
        {
            var memoryStream = new MemoryStream();
            using (var fileStream = File.OpenRead(dllPath))
            {
                fileStream.CopyTo(memoryStream);
            }

            var peReader = new PEReader(memoryStream);
            MetadataReader mdReader = peReader.GetMetadataReader();

            return new LazyMetadataAnalyzer(dllPath, memoryStream, peReader, mdReader);
        }

        private Stream _dllStream;

        private PEReader _peReader;

        private readonly MetadataReader _mdReader;

        private readonly string _asmPath;

        private LazyMetadataAnalyzer(string asmPath, Stream dllStream, PEReader peReader, MetadataReader mdReader)
        {
            _dllStream = dllStream;
            _peReader = peReader;
            _mdReader = mdReader;
            _asmPath = asmPath;
        }

        public IAssemblyMetadata GetDefinedAssembly()
        {
            return new LazyAssemblyMetadata(_asmPath, _mdReader);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _peReader?.Dispose();
                    _dllStream.Dispose();
                }

                _peReader = null;
                _dllStream = null;

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