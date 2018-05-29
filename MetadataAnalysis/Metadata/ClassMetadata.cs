using System.Collections.Immutable;

namespace MetadataAnalysis.Metadata
{
    public class ClassMetadata : TypeMetadata
    {
        public ClassMetadata(
            string name,
            string @namespace,
            ProtectionLevel protectionLevel,
            TypeMetadata baseType,
            TypeMetadata declaringType,
            IImmutableList<ConstructorMetadata> constructors,
            IImmutableDictionary<string, FieldMetadata> fields,
            IImmutableDictionary<string, PropertyMetadata> properties,
            IImmutableDictionary<string, IImmutableList<MethodMetadata>> methods,
            IImmutableList<GenericParameterMetadata> genericParameters = null,
            IImmutableList<CustomAttributeMetadata> customAttributes = null,
            bool isAbstract = false,
            bool isSealed = false
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
            methods,
            genericParameters,
            customAttributes)
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