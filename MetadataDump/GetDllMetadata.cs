using System;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection.PortableExecutable;
using MetadataAnalysis;
using MetadataAnalysis.Metadata;

namespace MetadataDump
{
    [Cmdlet(VerbsCommon.Get,"DllMetadata")]
    [OutputType(typeof(AssemblyMetadata))]
    public class GetDllMetadataCommand : PSCmdlet
    {
        [ValidateNotNullOrEmpty()]
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string DllPath { get; set; }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            using (FileStream fileStream = File.OpenRead(DllPath))
            using (var peReader = new PEReader(fileStream))
            using (var mdAnalyzer = new MetadataAnalyzer(peReader))
            {
                WriteObject(mdAnalyzer.GetDefinedAssembly());
            }
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
        }
    }
}
