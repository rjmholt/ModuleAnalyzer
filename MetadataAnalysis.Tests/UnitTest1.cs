using System;
using Xunit;
using MetadataAnalysis;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Reflection.Metadata;

namespace MetadataAnalysis.Tests
{
    public class TypeTest : IDisposable
    {
        private static readonly string s_assetPath = "../../../assets/test.dll";

        private FileStream _dllFile;
        private PEReader _dllReader;
        private MetadataAnalyzer _metadataAnalyzer;

        public TypeTest()
        {
            _dllFile = File.OpenRead(s_assetPath);
            _metadataAnalyzer = MetadataAnalyzer.Create(_dllFile);

            Console.WriteLine("PID: " + System.Diagnostics.Process.GetCurrentProcess().Id);
            Console.ReadKey();
        }


        [Fact]
        public void AllTypesAreFound()
        {
            _metadataAnalyzer.GetAllTypeDefinitions();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _metadataAnalyzer.Dispose();
                    _dllReader.Dispose();
                    _dllFile.Dispose();
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
