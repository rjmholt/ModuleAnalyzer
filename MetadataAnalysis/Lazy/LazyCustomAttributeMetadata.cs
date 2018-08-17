using System.Collections.Generic;
using System.Reflection.Metadata;
using MetadataAnalysis.Interface;

namespace MetadataAnalysis.Lazy
{
    public class LazyCustomAttributeMetadata : ICustomAttributeMetadata
    {
        private readonly MetadataReader _mdReader;

        private CustomAttribute _customAttribute;

        private ITypeMetadata _attributeType;

        public LazyCustomAttributeMetadata(CustomAttribute customAttribute, MetadataReader mdReader)
        {
            _customAttribute = customAttribute;
            _mdReader = mdReader;
        }

        public ITypeMetadata AttributeType
        {
            get
            {
                if (_attributeType == null)
                {
                    _attributeType = LazyTypeMetadata.Create(_customAttribute, _mdReader);
                }
                return _attributeType;
            }
        }

        public IReadOnlyList<CustomAttributeTypedArgument<ITypeMetadata>> PositionalArguments => throw new System.NotImplementedException();

        public IReadOnlyList<CustomAttributeNamedArgument<ITypeMetadata>> NamedArguments => throw new System.NotImplementedException();
    }
}