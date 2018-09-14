using System;
using System.Collections.Generic;

namespace MetadataHydrator.Lazy.SignatureDecoding
{
    internal struct LazyGenericContext
    {
        private readonly Lazy<IReadOnlyCollection<IGenericParameterMetadata>> _classGenericParameters;

        public LazyGenericContext(
            Lazy<IReadOnlyCollection<IGenericParameterMetadata>> classGenericParameters)
        {
            _classGenericParameters = classGenericParameters;
        }

        public IReadOnlyCollection<IGenericParameterMetadata> ClassGenericParameters => _classGenericParameters.Value;
    }
}