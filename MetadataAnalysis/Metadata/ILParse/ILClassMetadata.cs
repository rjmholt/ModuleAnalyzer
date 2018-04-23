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
            IImmutableList<IConstructorMetadata> constructors,
            IImmutableDictionary<string, IFieldMetadata> fields,
            IImmutableDictionary<string, IPropertyMetadata> properties,
            IImmutableDictionary<string, IImmutableList<IMethodMetadata>> methods,
            IImmutableDictionary<string, ITypeMetadata> nestedTypes,
            bool isAbstract,
            bool isSealed
        ) : base(name, @namespace, TypeKind.Class, protectionLevel, baseType, constructors, fields, properties, methods, nestedTypes)
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