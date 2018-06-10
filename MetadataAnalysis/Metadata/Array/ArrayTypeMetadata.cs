using System.Collections.Generic;
using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata.Array
{
    public class ArrayTypeMetadata : DefinedTypeMetadata
    {
        private class ArrayProbeType
        {
        }

        static ArrayTypeMetadata()
        {
            s_arrayProbeTypeMetadata = (ClassMetadata)LoadedTypes.FromType(typeof(ArrayProbeType));
            s_arrayTypeMetadata = (DefinedTypeMetadata)LoadedTypes.FromType(typeof(ArrayProbeType[]));
        }

        private static ClassMetadata s_arrayProbeTypeMetadata;

        private static DefinedTypeMetadata s_arrayTypeMetadata;

        public static ArrayTypeMetadata CreateArrayFromType(TypeMetadata typeMetadata)
        {
            var constructors = new List<ConstructorMetadata>();
            foreach (ConstructorMetadata constructorTemplate in s_arrayTypeMetadata.Constructors)
            {
                constructors.Add(InstantiateConstructorWithType(constructorTemplate, typeMetadata));
            }

            var fields = new Dictionary<string, FieldMetadata>();
            foreach (KeyValuePair<string, FieldMetadata> fieldEntry in s_arrayTypeMetadata.Fields)
            {
                fields.Add(fieldEntry.Key, InstantiateFieldWithType(fieldEntry.Value, typeMetadata));
            }

            var properties = new Dictionary<string, PropertyMetadata>();
            foreach (KeyValuePair<string, PropertyMetadata> propertyEntry in s_arrayTypeMetadata.Properties)
            {
                properties.Add(propertyEntry.Key, InstantiatePropertyWithType(propertyEntry.Value, typeMetadata));
            }

            var methods = new Dictionary<string, IImmutableList<MethodMetadata>>();
            foreach (KeyValuePair<string, IImmutableList<MethodMetadata>> methodEntry in s_arrayTypeMetadata.Methods)
            {
                var overloadList = new List<MethodMetadata>();
                foreach (MethodMetadata methodTemplate in methodEntry.Value)
                {
                    overloadList.Add(InstantiateMethodWithType(methodTemplate, typeMetadata));
                }

                methods.Add(methodEntry.Key, overloadList.ToImmutableArray());
            }

            return new ArrayTypeMetadata(
                typeMetadata,
                constructors.ToImmutableArray(),
                fields.ToImmutableDictionary(),
                properties.ToImmutableDictionary(),
                methods.ToImmutableDictionary()
            );
        }

        private static ConstructorMetadata InstantiateConstructorWithType(
            ConstructorMetadata constructorTemplate,
            TypeMetadata typeMetadata)
        {
            return new ConstructorMetadata(
                constructorTemplate.ProtectionLevel,
                constructorTemplate.IsStatic,
                constructorTemplate.CustomAttributes);
        }

        private static FieldMetadata InstantiateFieldWithType(
            FieldMetadata fieldTemplate,
            TypeMetadata typeMetadata)
        {
            return new FieldMetadata(
                fieldTemplate.Name,
                fieldTemplate.ProtectionLevel,
                fieldTemplate.IsStatic,
                InstantiateGenericParameterListWithType(fieldTemplate.GenericParameters, typeMetadata));
        }

        private static PropertyMetadata InstantiatePropertyWithType(
            PropertyMetadata propertyTemplate,
            TypeMetadata typeMetadata)
        {
            PropertyGetterMetadata getter = InstantiateGetterWithType(propertyTemplate, typeMetadata);
            PropertySetterMetadata setter = InstantiateSetterWithType(propertyTemplate, typeMetadata);

            return new PropertyMetadata(
                propertyTemplate.Name,
                propertyTemplate.Type == s_arrayProbeTypeMetadata ? typeMetadata : propertyTemplate.Type,
                propertyTemplate.ProtectionLevel,
                getter,
                setter,
                propertyTemplate.IsStatic,
                InstantiateGenericParameterListWithType(propertyTemplate.GenericParameters, typeMetadata));
        }

        private static PropertyGetterMetadata InstantiateGetterWithType(
            PropertyMetadata propertyTemplate,
            TypeMetadata typeMetadata)
        {
            return new PropertyGetterMetadata(
                propertyTemplate.Name,
                propertyTemplate.Getter.ProtectionLevel,
                propertyTemplate.Getter.IsStatic);
        }

        private static PropertySetterMetadata InstantiateSetterWithType(
            PropertyMetadata propertyTemplate,
            TypeMetadata typeMetadata)
        {
            return new PropertySetterMetadata(
                propertyTemplate.Name,
                propertyTemplate.Setter.ProtectionLevel,
                propertyTemplate.Setter.IsStatic);
        }

        private static MethodMetadata InstantiateMethodWithType(
            MethodMetadata methodTemplate,
            TypeMetadata typeMetadata)
        {
            return new MethodMetadata(
                methodTemplate.Name,
                methodTemplate.ProtectionLevel,
                methodTemplate.IsStatic,
                InstantiateGenericParameterListWithType(methodTemplate.GenericParameters, typeMetadata));
        }

        private static CustomAttributeMetadata InstantiateCustomAttributeWithType(
            CustomAttributeMetadata customAttributeTemplate,
            TypeMetadata typeMetadata)
        {
            return new CustomAttributeMetadata(
                customAttributeTemplate.AttributeType,
                customAttributeTemplate.NamedArguments,
                customAttributeTemplate.PositionalArguments);
        }

        private static IImmutableList<GenericParameterMetadata> InstantiateGenericParameterListWithType(
            IEnumerable<GenericParameterMetadata> genericParameterTemplates,
            TypeMetadata typeMetadata)
        {
            var genericParameters = new List<GenericParameterMetadata>();
            foreach (GenericParameterMetadata genericParameterTemplate in genericParameterTemplates)
            {
                genericParameters.Add(InstantiateGenericParameterWithType(genericParameterTemplate, typeMetadata));
            }
            return genericParameters.ToImmutableArray();
        }

        private static GenericParameterMetadata InstantiateGenericParameterWithType(
            GenericParameterMetadata genericParameterTemplate,
            TypeMetadata typeMetadata)
        {
            if (genericParameterTemplate is ConcreteGenericParameterMetadata concreteParameterTemplate)
            {
                // TODO: Check if this generic instantiation is of the template
                // type and switch it if it is
                return concreteParameterTemplate;
            }

            return genericParameterTemplate;
        }


        private ArrayTypeMetadata(
            TypeMetadata underlyingType,
            IImmutableList<ConstructorMetadata> constructors,
            IImmutableDictionary<string, FieldMetadata> fields,
            IImmutableDictionary<string, PropertyMetadata> properties,
            IImmutableDictionary<string, IImmutableList<MethodMetadata>> methods,
            IImmutableList<GenericParameterMetadata> genericParameters = null,
            IImmutableList<CustomAttributeMetadata> customAttributes = null)
                : base(
                    underlyingType.Name + "[]",
                    underlyingType.Namespace,
                    TypeKind.ArrayType,
                    underlyingType.ProtectionLevel,
                    LoadedTypes.ArrayTypeMetadata,
                    constructors,
                    fields,
                    properties,
                    methods,
                    genericParameters: null,
                    customAttributes: customAttributes)
        {
            UnderlyingType = underlyingType;
        }

        public TypeMetadata UnderlyingType { get; }

        internal override TypeMetadata InstantiateGenerics(IImmutableList<TypeMetadata> genericArguments)
        {
            throw new System.NotImplementedException();
        }
    }
}