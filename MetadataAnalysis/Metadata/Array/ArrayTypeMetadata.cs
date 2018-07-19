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

            return new ArrayTypeMetadata(typeMetadata)
            {
                NestedTypes = ImmutableDictionary<string, DefinedTypeMetadata>.Empty,
                Constructors = constructors.ToImmutableArray(),
                CustomAttributes = ImmutableArray<CustomAttributeMetadata>.Empty,
                Fields = fields.ToImmutableDictionary(),
                Properties = properties.ToImmutableDictionary(),
                Methods = methods.ToImmutableDictionary(),
                GenericParameters = ImmutableArray<GenericParameterMetadata>.Empty
            };
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
                fieldTemplate.IsStatic)
            {
                GenericParameters = InstantiateGenericParameterListWithType(fieldTemplate.GenericParameters, typeMetadata)
            };
        }

        private static PropertyMetadata InstantiatePropertyWithType(
            PropertyMetadata propertyTemplate,
            TypeMetadata typeMetadata)
        {
            PropertyGetterMetadata getter = InstantiateGetterWithType(propertyTemplate, typeMetadata);
            PropertySetterMetadata setter = InstantiateSetterWithType(propertyTemplate, typeMetadata);

            return new PropertyMetadata(
                propertyTemplate.Name,
                propertyTemplate.ProtectionLevel,
                propertyTemplate.IsStatic)
            {
                Type = propertyTemplate.Type == s_arrayProbeTypeMetadata ? typeMetadata : propertyTemplate.Type,
                Getter = getter,
                Setter = setter,
                GenericParameters = InstantiateGenericParameterListWithType(propertyTemplate.GenericParameters, typeMetadata)
            };
        }

        private static PropertyGetterMetadata InstantiateGetterWithType(
            PropertyMetadata propertyTemplate,
            TypeMetadata typeMetadata)
        {
            if (propertyTemplate.Getter == null)
            {
                return null;
            }

            return new PropertyGetterMetadata(
                propertyTemplate.Name,
                propertyTemplate.Getter.ProtectionLevel,
                propertyTemplate.Getter.IsStatic);
        }

        private static PropertySetterMetadata InstantiateSetterWithType(
            PropertyMetadata propertyTemplate,
            TypeMetadata typeMetadata)
        {
            if (propertyTemplate.Setter == null)
            {
                return null;
            }

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
                methodTemplate.IsStatic)
            {
                ReturnType = methodTemplate.ReturnType == s_arrayProbeTypeMetadata ? typeMetadata : methodTemplate.ReturnType,
                GenericParameters = InstantiateGenericParameterListWithType(methodTemplate.GenericParameters, typeMetadata)
            };
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
            if (genericParameterTemplates == null)
            {
                return ImmutableArray<GenericParameterMetadata>.Empty;
            }

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

        internal ArrayTypeMetadata(
            string name,
            string @namespace,
            string fullName,
            ProtectionLevel protectionLevel)
            : base(name, @namespace, fullName, TypeKind.ArrayType, protectionLevel)
        {
            BaseType = LoadedTypes.ArrayTypeMetadata;
        }

        private ArrayTypeMetadata(
            TypeMetadata underlyingType)
            : this(
                underlyingType.Name + "[]",
                underlyingType.Namespace,
                underlyingType.FullName + "[]",
                underlyingType.ProtectionLevel)
        {
            UnderlyingType = underlyingType;
            BaseType = LoadedTypes.ArrayTypeMetadata;
        }

        public TypeMetadata UnderlyingType { get; internal set; }

        internal override TypeMetadata InstantiateGenerics(IImmutableList<TypeMetadata> genericArguments)
        {
            throw new System.NotImplementedException();
        }
    }
}