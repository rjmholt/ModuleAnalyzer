using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Linq;
using Xunit;

namespace MetadataHydrator.Tests
{
    public abstract class MetadataHydratorTests
    {
        protected IAssemblyDefinitionMetadata _assemblyMetadata;

        protected MetadataHydratorTests(IAssemblyDefinitionMetadata assemblyMetadata)
        {
            _assemblyMetadata = assemblyMetadata;
        }

        public IEnumerable<KeyValuePair<string, TypeInfo>> AssemblyTypes { get; }

        [Fact]
        public void HasCorrectName()
        {
            Assert.Equal(TestDll.TestAssembly.GetName().Name, _assemblyMetadata.Name);
        }

        [Fact]
        public void HasCorrectVersion()
        {
            Assert.Equal(TestDll.TestAssembly.GetName().Version, _assemblyMetadata.Version);
        }

        [Fact]
        public void HasCorrectCulture()
        {
            Assert.Equal(TestDll.TestAssembly.GetName().CultureName, _assemblyMetadata.Culture);
        }

        [Fact]
        public void HasCorrectPublicKey()
        {
            Assert.Equal(TestDll.TestAssembly.GetName().GetPublicKey(), _assemblyMetadata.PublicKey);
        }

        [Fact]
        public void HasCorrectHashAlgorithm()
        {
            // The types are different, but should have the same names and values
            Assert.Equal((AssemblyHashAlgorithm)TestDll.TestAssembly.GetName().HashAlgorithm, _assemblyMetadata.HashAlgorithm);
            Assert.Equal(TestDll.TestAssembly.GetName().HashAlgorithm.ToString(), _assemblyMetadata.HashAlgorithm.ToString(), ignoreCase: true);
        }

        [Fact]
        public void HasCorrectNumberOfTypes()
        {
            // Deduct one for the "<Module>" type
            Assert.Equal(DefinedTypes.Count(), _assemblyMetadata.TypeDefinitions.Count - 1);
        }

        [Theory]
        [MemberData(nameof(DefinedTypes))]
        public void HasDefinedType(TypeInfo assemblyType)
        {
            Assert.Contains(assemblyType.FullName, _assemblyMetadata.TypeDefinitions.Keys);
        }

        [Theory]
        [MemberData(nameof(NestedDefinedTypes))]
        public void HasNestedDefinedTypes(TypeInfo nestedType)
        {
            IReadOnlyList<Type> nestedTypePath = GetNestedTypePath(nestedType);
            
            ITypeDefinitionMetadata currentType = _assemblyMetadata.TypeDefinitions[nestedTypePath[0].FullName];
            for (int i = 1; i < nestedTypePath.Count; i++)
            {
                currentType = currentType.NestedTypes[nestedTypePath[i].FullName];
            }

            Assert.Equal(nestedType.FullName, currentType.FullName);
        }

        public static IEnumerable<object[]> DefinedTypes => TestDll.TestAssembly.DefinedTypes
            .Where(t => !t.IsNested)
            .Select(t => new object[] { t });

        public static IEnumerable<object[]> NestedDefinedTypes = TestDll.TestAssembly.DefinedTypes
            .Where(t => t.IsNested)
            .Select(t => new object[] { t });

        private static IReadOnlyList<Type> GetNestedTypePath(Type nestedType)
        {
            var path = new List<Type>();
            while (nestedType != null)
            {
                path.Add(nestedType);
                nestedType = nestedType.DeclaringType;
            }

            path.Reverse();

            return path;
        }

        private static void DEBUG_HERE()
        {
            if (Debugger.IsAttached)
            {
                return;
            }

            Console.WriteLine($"PID: {Process.GetCurrentProcess().Id}");
            Console.ReadKey();
        }
    }
}