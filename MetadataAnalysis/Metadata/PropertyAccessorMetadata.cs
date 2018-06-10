using System.Collections.Immutable;

namespace MetadataAnalysis.Metadata
{
    public class PropertyAccessorMetadata : MethodMetadata
    {
        protected PropertyAccessorMetadata(
            string name,
            ProtectionLevel protectionLevel,
            bool isStatic = false)
            : base(name, protectionLevel, isStatic)
        {
        }
    }

    public class PropertyGetterMetadata : PropertyAccessorMetadata
    {
        private const string GETTER_PREFIX = "get_";

        public PropertyGetterMetadata(
            string propertyName,
            ProtectionLevel protectionLevel,
            bool isStatic = false)
            : base(GETTER_PREFIX + propertyName, protectionLevel, isStatic)
        {
            ParameterTypes = ImmutableArray<TypeMetadata>.Empty;
        }
    }

    public class PropertySetterMetadata : PropertyAccessorMetadata
    {
        private const string SETTER_PREFIX = "set_";

        public PropertySetterMetadata(
            string propertyName,
            ProtectionLevel protectionLevel,
            bool isStatic = false)
            : base(SETTER_PREFIX + propertyName, protectionLevel, isStatic)
        {
            ReturnType = LoadedTypes.VoidTypeMetadata;
        }
    }
}