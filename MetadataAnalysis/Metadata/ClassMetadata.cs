using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata
{
    public class ClassMetadata : DefinedTypeMetadata
    {
        public ClassMetadata(
            string name,
            string @namespace,
            ProtectionLevel protectionLevel,
            TypeMetadata baseType,
            DefinedTypeMetadata declaringType,
            bool isAbstract = false,
            bool isSealed = false)
                : base(
                    name,
                    @namespace,
                    TypeKind.Class,
                    protectionLevel,
                    baseType,
                    declaringType)
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

        internal override TypeMetadata InstantiateGenerics(IImmutableList<TypeMetadata> genericArguments)
        {
            return new ClassMetadata(
                Name,
                Namespace,
                ProtectionLevel,
                BaseType,
                DeclaringType)
            {
                Constructors = Constructors,
                Fields = Fields,
                Properties = Properties,
                Methods = Methods,
                GenericParameters = InstantiateGenericList(genericArguments),
                CustomAttributes = CustomAttributes
            };
        }
    }
}