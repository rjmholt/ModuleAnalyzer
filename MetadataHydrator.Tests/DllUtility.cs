using System.CodeDom.Compiler;
using System.IO;
using System.Environment;
using System.Reflection;
using Microsoft.CSharp;

namespace MetadataAnalysis.Tests
{
    public static class DllUtility
    {
        private static readonly string s_assetsDirPath = Path.Combine("..", "..", "assets");

        private static readonly string s_sourcePath = Path.Combine(s_assetsDirPath, "testsource.cs");

        public static Assembly CompileSource()
        {
            ICodeCompiler compiler = new CSharpCodeProvider().CreateCompiler();

            var parameters = new CompilerParameters()
            {
                GenerateInMemory = false,
                CompilerOptions = "/unsafe"
            };

            CompilerResults compilationResults = compiler.CompileAssemblyFromFile(parameters, s_sourcePath);

            if (compilationResults.Errors.Count > 0)
            {
                Console.WriteLine($"Errors building {s_sourcePath} into {compilationResults.PathToAssembly}:");
                foreach (CompilerError err in compilationResults.Errors)
                {
                    Console.WriteLine($"\t{err.ToString()}{Environment.NewLine}");
                }

                throw new Exception("Test files failed to compile");
            }

            return Assembly.LoadFile(compilationResults.PathToAssembly);
        }
    }
}