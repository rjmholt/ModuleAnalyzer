using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Interface;

namespace MetadataAnalysis.Metadata.ILParse
{
    public class ILClassMetadata : ILTypeMetadata, IClassMetadata
    {
        public ILClassMetadata(
            string name,
            string @namespace,
            ProtectionLevel protectionLevel,
            ITypeMetadata baseType,
            ITypeMetadata declaringType,
            IImmutableList<IConstructorMetadata> constructors,
            IImmutableDictionary<string, IFieldMetadata> fields,
            IImmutableDictionary<string, IPropertyMetadata> properties,
            IImmutableDictionary<string, IImmutableList<IMethodMetadata>> methods,
            bool isAbstract,
            bool isSealed
        ) : base(
            name,
            @namespace,
            TypeKind.Class,
            protectionLevel,
            baseType,
            declaringType,
            constructors,
            fields,
            properties,
            methods)
        {
            IsAbstract = isAbstract;
            IsSealed = isSealed;
        }

        public bool IsStatic
        {
            get
            {
                return IsAbstract && IsSealed;
            }
        }

        public bool IsAbstract { get; }

        public bool IsSealed { get; }
    }
}