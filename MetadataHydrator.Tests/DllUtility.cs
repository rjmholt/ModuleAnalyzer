using System;
using System.IO;
using System.Reflection;

namespace MetadataHydrator.Tests
{
    public static class DllUtility
    {
        private static readonly string s_currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static readonly string s_projRootPath = Path.Combine(s_currentPath, "..", "..", "..");

        private static readonly string s_assetsDirPath = Path.GetFullPath(Path.Combine(s_projRootPath, "assets"));

        private static readonly string s_asmPath = Path.Combine(s_assetsDirPath, "test.dll");

        public static Assembly LoadTestAssembly()
        {
            return Assembly.LoadFile(s_asmPath);
        }

        public static Assembly TestAssembly => s_testAssembly.Value;

        private static Lazy<Assembly> s_testAssembly = new Lazy<Assembly>(LoadTestAssembly);
    }
}