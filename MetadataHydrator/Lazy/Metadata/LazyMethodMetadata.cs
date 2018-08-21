using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis;

namespace MetadataHydrator.Lazy.Metadata
{
    internal class LazyMethodMetadata : LazyMemberMetadata, IMethodMetadata
    {
        private readonly MethodDefinition _methodDefinition;

        public LazyMethodMetadata(
            string name,
            Accessibility accessibility,
            MethodDefinition methodDefinition,
            LazyAssemblyHydrator assemblyHydrator)
            : base(name, assemblyHydrator)
        {
            Accessibility = accessibility;
            _methodDefinition = methodDefinition;
        }

        public ITypeMetadata ReturnType => throw new System.NotImplementedException();

        public Accessibility Accessibility { get; }

        public bool IsStatic => _methodDefinition.Attributes.HasFlag(MethodAttributes.Static);

        public bool isAbstract => _methodDefinition.Attributes.HasFlag(MethodAttributes.Abstract);

        public IReadOnlyCollection<ITypeMetadata> ParameterTypes => throw new System.NotImplementedException();

        public IReadOnlyCollection<IGenericParameterMetadata> GenericParameters => throw new System.NotImplementedException();

        public override IReadOnlyCollection<ICustomAttributeMetadata> CustomAttributes => throw new System.NotImplementedException();

        protected override void ResolveSignature()
        {
            throw new System.NotImplementedException();
        }
    }
}