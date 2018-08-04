using System;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection.PortableExecutable;
using System.Collections.Immutable;
using MetadataAnalysis;
using MetadataAnalysis.Metadata;

namespace MetadataDump
{
    [Cmdlet(VerbsData.Export,"AssemblyMetadata")]
    [OutputType(typeof(AssemblyMetadata))]
    public class ExportAssemblyMetadataCommand : PSCmdlet
    {
        [ValidateNotNullOrEmpty()]
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string DllPath { get; set; }

        [Parameter(Position = 1)]
        public string[] SearchPath { get; set; }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            using (FileStream fileStream = File.OpenRead(DllPath))
            using (var mdAnalyzer = MetadataAnalyzer.Create(fileStream, dllPaths: SearchPath))
            {
                WriteObject(mdAnalyzer.GetDefinedAssembly(
                    out IImmutableDictionary<string, AssemblyMetadata> unused_asms));
            }
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
        }
    }
}
