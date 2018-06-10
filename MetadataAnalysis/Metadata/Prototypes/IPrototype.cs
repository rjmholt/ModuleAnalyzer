using System.Collections.Generic;
using MetadataAnalysis.Metadata.Array;
using MetadataAnalysis.Metadata.Generic;
using MetadataAnalysis.Metadata.Signature;

namespace MetadataAnalysis.Metadata.Prototypes
{
    internal interface IPrototype<out T>
    {
        T Get();
    }

    internal interface ITypeMetadataPrototype<out TType> : IPrototype<TType>
    {
        string Name { get; }

        string Namespace { get; }

        TypeKind TypeKind { get; }

        ProtectionLevel ProtectionLevel { get; }

        ITypeMetadataPrototype<TypeMetadata> BaseType { get; set; }

        IList<IConstructorMetadataPrototype> Constructors { get; }

        IDictionary<string, IFieldMetadataPrototype> Fields { get; }

        IDictionary<string, IPropertyMetadataPrototype> Properties { get; }

        IDictionary<string, IList<IMethodMetadataPrototype>> Methods { get; }

        IList<IGenericParameterMetadataPrototype> GenericParameters { get; }

        IList<ICustomAttributeMetadataPrototype> CustomAttributes { get; }
    }

    internal interface IDefinedTypeMetadataPrototype<out TType> : ITypeMetadataPrototype<TType>
    {

    }
    
    internal interface IClassMetadataPrototype : IDefinedTypeMetadataPrototype<ClassMetadata>
    {

    }

    internal interface IStructMetadataPrototype : IDefinedTypeMetadataPrototype<StructMetadata>
    {

    }

    internal interface IEnumMetadataPrototype : IDefinedTypeMetadataPrototype<EnumMetadata>
    {

    }

    internal interface IMemberMetadataPrototype<out TMember> : IPrototype<TMember>
    {

    }

    internal interface IFieldMetadataPrototype : IMemberMetadataPrototype<FieldMetadata>
    {

    }

    internal interface IPropertyMetadataPrototype : IMemberMetadataPrototype<PropertyMetadata>
    {

    }

    internal interface IMethodMetadataPrototype<out TMethod> : IMemberMetadataPrototype<TMethod>
    {

    }

    internal interface IMethodMetadataPrototype : IMethodMetadataPrototype<MethodMetadata>
    {

    }

    internal interface IConstructorMetadataPrototype : IMethodMetadataPrototype<ConstructorMetadata>
    {

    }

    internal interface IGenericParameterMetadataPrototype<out TParameter> : IPrototype<TParameter>
    {

    }

    internal interface IGenericParameterMetadataPrototype : IGenericParameterMetadataPrototype<GenericParameterMetadata>
    {

    }

    internal interface IConcreteGenericParameterMetadataPrototype
        : IGenericParameterMetadataPrototype<ConcreteGenericParameterMetadata>
    {

    }

    internal interface IUninstantiatedGenericParameterMetadataPrototype
        : IGenericParameterMetadataPrototype<UninstantiatedGenericParameterMetadata>
    {

    }

    internal interface ICustomAttributeMetadataPrototype : IPrototype<CustomAttributeMetadata>
    {

    }
}