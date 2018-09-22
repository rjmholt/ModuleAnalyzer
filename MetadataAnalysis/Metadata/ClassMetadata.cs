using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata
{
    public class ClassMetadata : DefinedTypeMetadata
    {
        public ClassMetadata(
            string name,
            string @namespace,
            string fullName,
            ProtectionLevel protectionLevel,
            bool isAbstract = false,
            bool isSealed = false)
                : base(
                    name,
                    @namespace,
                    fullName,
                    TypeKind.Class,
                    protectionLevel)
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
                FullName,
                ProtectionLevel)
            {
                BaseType = BaseType,
                DeclaringType = DeclaringType,
                NestedTypes = NestedTypes,
                Constructors = Constructors,
                Fields = Fields,
                Properties = Properties,
                Indexers = Indexers,
                Methods = Methods,
                GenericParameters = InstantiateGenericList(genericArguments),
                CustomAttributes = CustomAttributes
            };
        }
    }
}