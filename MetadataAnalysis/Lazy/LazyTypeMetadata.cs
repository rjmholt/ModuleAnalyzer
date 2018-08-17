using System.Collections.Generic;
using System.Reflection.Metadata;
using MetadataAnalysis.Interface;

namespace MetadataAnalysis.Lazy
{
    public class LazyTypeMetadata : ITypeMetadata
    {
        public string Name => throw new System.NotImplementedException();

        public string Namespace => throw new System.NotImplementedException();

        public string FullName => throw new System.NotImplementedException();

        public TypeKind TypeKind => throw new System.NotImplementedException();

        public ProtectionLevel ProtectionLevel => throw new System.NotImplementedException();

        public ITypeMetadata BaseType => throw new System.NotImplementedException();

        public IReadOnlyList<IConstructorMetadata> Constructors => throw new System.NotImplementedException();

        public IReadOnlyDictionary<string, IFieldMetadata> Fields => throw new System.NotImplementedException();

        public IReadOnlyDictionary<string, IPropertyMetadata> Properties => throw new System.NotImplementedException();

        public IReadOnlyDictionary<string, IReadOnlyList<IMethodMetadata>> Methods => throw new System.NotImplementedException();

        public IReadOnlyList<IGenericParameterMetadata> GenericParameters => throw new System.NotImplementedException();

        public IReadOnlyList<ICustomAttributeMetadata> CustomAttributes => throw new System.NotImplementedException();
    }
}