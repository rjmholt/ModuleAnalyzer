using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection.Metadata;
using MetadataAnalysis.Metadata;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis
{
    /// <summary>
    /// Static class to deal with getting and caching type metadata from
    /// types that are loaded in the runtime while parsing.
    /// </summary>
    public static class LoadedTypes
    {
        /// <summary>
        /// A cache of the types fetched from the .NET runtime.
        /// </summary>
        private static ConcurrentDictionary<Type, TypeMetadata> s_typeMetadataCache;

        private static ConcurrentDictionary<Type, PrimitiveTypeCode> s_primitiveTypeCodes;

        /// <summary>
        /// Initialize the loaded type cache with common primitive types.
        /// </summary>
        static LoadedTypes()
        {
            // Primitive types to preload into both a correspondance table
            // with the PrimitiveTypeCode enum and also into the metadata cache
            var primitiveTypeCodes = new Dictionary<Type, PrimitiveTypeCode>()
            {
                { typeof(Boolean),        PrimitiveTypeCode.Boolean        },
                { typeof(Byte),           PrimitiveTypeCode.Byte           },
                { typeof(Char),           PrimitiveTypeCode.Char           },
                { typeof(Double),         PrimitiveTypeCode.Double         },
                { typeof(Int16),          PrimitiveTypeCode.Int16          },
                { typeof(Int32),          PrimitiveTypeCode.Int32          },
                { typeof(Int64),          PrimitiveTypeCode.Int64          },
                { typeof(IntPtr),         PrimitiveTypeCode.IntPtr         },
                { typeof(Object),         PrimitiveTypeCode.Object         },
                { typeof(SByte),          PrimitiveTypeCode.SByte          },
                { typeof(Single),         PrimitiveTypeCode.Single         },
                { typeof(String),         PrimitiveTypeCode.String         },
                { typeof(TypedReference), PrimitiveTypeCode.TypedReference },
                { typeof(UInt16),         PrimitiveTypeCode.UInt16         },
                { typeof(UInt32),         PrimitiveTypeCode.UInt32         },
                { typeof(UInt64),         PrimitiveTypeCode.UInt64         },
                { typeof(UIntPtr),        PrimitiveTypeCode.UIntPtr        },
                { typeof(void),           PrimitiveTypeCode.Void           }
            };

            s_primitiveTypeCodes = new ConcurrentDictionary<Type, PrimitiveTypeCode>();
            foreach (KeyValuePair<Type, PrimitiveTypeCode> primitiveType in primitiveTypeCodes)
            {
                s_primitiveTypeCodes[primitiveType.Key] = primitiveType.Value;
            }

            // Initialize the cache
            s_typeMetadataCache = new ConcurrentDictionary<Type, TypeMetadata>();

            // Process the common base types
            Type objectType = typeof(object);
            Type valueType = typeof(ValueType);
            Type enumType = typeof(Enum);
            Type typeType = typeof(Type);

            var objectTypeMetadata = new ClassMetadata(
                objectType.Name,
                objectType.Namespace,
                objectType.FullName,
                ProtectionLevel.Public,
                isAbstract: false,
                isSealed: false
            )
            {
                NestedTypes = GetNestedTypeMetadata(objectType),
                Constructors = GetConstructorMetadata(objectType),
                Fields = GetFieldMetadata(objectType),
                Properties = GetPropertyMetadata(objectType),
                Methods = GetMethodMetadata(objectType)
            };

            var valueTypeMetadata = new ClassMetadata(
                valueType.Name,
                valueType.Namespace,
                valueType.FullName,
                ProtectionLevel.Public,
                isAbstract: true,
                isSealed: false
            )
            {
                BaseType = objectTypeMetadata,
                NestedTypes = GetNestedTypeMetadata(valueType),
                Constructors = GetConstructorMetadata(valueType),
                Fields = GetFieldMetadata(valueType),
                Properties = GetPropertyMetadata(valueType),
                Methods = GetMethodMetadata(valueType)
            };

            var enumTypeMetadata = new ClassMetadata(
                enumType.Name,
                enumType.Namespace,
                enumType.FullName,
                ProtectionLevel.Public,
                isAbstract: true,
                isSealed: false
            )
            {
                BaseType = valueTypeMetadata,
                NestedTypes = GetNestedTypeMetadata(enumType),
                Constructors = GetConstructorMetadata(enumType),
                Fields = GetFieldMetadata(enumType),
                Properties = GetPropertyMetadata(enumType),
                Methods = GetMethodMetadata(enumType),
            };

            ObjectTypeMetadata = objectTypeMetadata;
            ValueTypeMetadata  = valueTypeMetadata;
            EnumTypeMetadata   = enumTypeMetadata;

            s_typeMetadataCache.TryAdd(objectType, objectTypeMetadata);
            s_typeMetadataCache.TryAdd(valueType, valueTypeMetadata);
            s_typeMetadataCache.TryAdd(enumType, enumTypeMetadata);

            TypeTypeMetadata = (ClassMetadata)LoadedTypes.FromType(typeof(Type));
            ArrayTypeMetadata = (ClassMetadata)LoadedTypes.FromType(typeof(System.Array));
            VoidTypeMetadata = (ClassMetadata)LoadedTypes.FromType(typeof(void));

            // Add primitive types to the cache
            foreach (Type primitiveType in primitiveTypeCodes.Keys)
            {
                FromType(primitiveType);
            }
        }

        /// <summary>
        /// The type metadata for System.Object.
        /// </summary>
        public static ClassMetadata ObjectTypeMetadata { get; }

        /// <summary>
        /// The type metadata for System.ValueType (the struct base type).
        /// </summary>
        public static ClassMetadata ValueTypeMetadata { get; }

        /// <summary>
        /// The type metadata for System.Enum (the enum base type).
        /// </summary>
        public static ClassMetadata EnumTypeMetadata { get; }

        public static ClassMetadata TypeTypeMetadata { get; }

        public static ClassMetadata ArrayTypeMetadata { get; }

        public static ClassMetadata VoidTypeMetadata { get; }

        /// <summary>
        /// Get a type metadata object from a system type object.
        /// </summary>
        /// <param name="type">the type for which we want a metadata object.</param>
        /// <returns>an object representing the metadata of the given type.</returns>
        public static TypeMetadata FromType(Type type)
        {
            // Search the cache first
            TypeMetadata typeMetadata;
            if (s_typeMetadataCache.TryGetValue(type, out typeMetadata))
            {
                return typeMetadata;
            }

            // We'll want the declaring type in any case
            DefinedTypeMetadata declaringType = type.DeclaringType == null ? null : (DefinedTypeMetadata)FromType(type.DeclaringType);

            // We differentiate based on what kind of type
            if (type.IsEnum)
            {
                typeMetadata = GetEnumMetadata(type);
            }
            else if (type.IsValueType)
            {
                typeMetadata = GetStructMetadata(type);
            }
            else
            {
                typeMetadata = GetClassMetadata(type);
            }

            return typeMetadata;
        }

        /// <summary>
        /// Try and find a type in the runtime based on its full name.
        /// </summary>
        /// <param name="fullTypeName">the full name of the type to search for.</param>
        /// <param name="typeMetadata">the metadata of the type when its found.</param>
        /// <returns>true if the type is found, false otherwise.</returns>
        public static bool TryFindByName(string fullTypeName, out TypeMetadata typeMetadata)
        {
            // Look for the type by name, and if we find it, use our type function
            Type type = Type.GetType(fullTypeName);
            if (type == null)
            {
                typeMetadata = null;
                return false;
            }

            typeMetadata = FromType(type);
            return true;
        }

        public static TypeMetadata GetByName(string fullTypeName)
        {
            if (!TryFindByName(fullTypeName, out TypeMetadata typeMetadata))
            {
                throw new Exception($"Unable to find loaded type: '{fullTypeName}'");
            }

            return typeMetadata;
        }

        public static bool IsTypeLoaded(string fullTypeName)
        {
            return Type.GetType(fullTypeName) != null;
        }

        public static PrimitiveTypeCode GetPrimitiveTypeCode(Type type)
        {
            if (!s_primitiveTypeCodes.TryGetValue(type, out PrimitiveTypeCode typeCode))
            {
                throw new ArgumentException("Type is not a primitive type", nameof(type));
            }

            return typeCode;
        }

        private static ClassMetadata GetClassMetadata(Type classType)
        {
            var classMetadata = new ClassMetadata(
                classType.Name,
                classType.Namespace,
                classType.FullName,
                GetTypeProtectionLevel(classType),
                classType.IsAbstract,
                classType.IsSealed
            );

            if (!s_typeMetadataCache.TryAdd(classType, classMetadata))
            {
                throw new Exception($"Loaded class type already in cache: {classMetadata.FullName}");
            }

            classMetadata.BaseType = FromType(classType.BaseType);
            classMetadata.DeclaringType = (DefinedTypeMetadata)FromType(classType.DeclaringType);
            classMetadata.Constructors = GetConstructorMetadata(classType);
            classMetadata.Fields = GetFieldMetadata(classType);
            classMetadata.Properties = GetPropertyMetadata(classType);
            classMetadata.Methods = GetMethodMetadata(classType);

            return classMetadata;
        }

        private static StructMetadata GetStructMetadata(Type structType)
        {
            var structMetadata = new StructMetadata(
                structType.Name,
                structType.Namespace,
                structType.FullName,
                GetTypeProtectionLevel(structType)
            )
            {
            };

            if (!s_typeMetadataCache.TryAdd(structType, structMetadata))
            {
                throw new Exception($"Loaded struct type already in cache: {structMetadata.FullName}");
            }

            structMetadata.DeclaringType = (DefinedTypeMetadata)FromType(structType.DeclaringType);
            structMetadata.Constructors = GetConstructorMetadata(structType);
            structMetadata.Fields = GetFieldMetadata(structType);
            structMetadata.Properties = GetPropertyMetadata(structType);
            structMetadata.Methods = GetMethodMetadata(structType);

            return structMetadata;
        }

        private static EnumMetadata GetEnumMetadata(Type enumType)
        {
            var enumMetadata = new EnumMetadata(
                enumType.Name,
                enumType.Namespace,
                enumType.FullName,
                GetTypeProtectionLevel(enumType),
                s_primitiveTypeCodes[enumType.UnderlyingSystemType]
            );

            if (!s_typeMetadataCache.TryAdd(enumType, enumMetadata))
            {
                throw new Exception($"Loaded enum type already in cache: {enumMetadata.FullName}");
            }

            enumMetadata.DeclaringType = (DefinedTypeMetadata)FromType(enumType.DeclaringType);
            enumMetadata.Members = GetEnumMemberMetadata(enumType, enumMetadata);
            enumMetadata.Fields = GetFieldMetadata(enumType);
            enumMetadata.Properties = GetPropertyMetadata(enumType);
            enumMetadata.Methods = GetMethodMetadata(enumType);
            enumMetadata.CustomAttributes = GetCustomAttributes(enumType.GetCustomAttributesData());

            return enumMetadata;
        }

        /// <summary>
        /// Get the accessibility level of a type.
        /// </summary>
        /// <param name="type">the type to get the accessibility of.</param>
        /// <returns>the accessibility level of the type.</returns>
        private static ProtectionLevel GetTypeProtectionLevel(Type type)
        {
            if (type.IsPublic || type.IsNestedPublic)
            {
                return ProtectionLevel.Public;
            }

            if (type.IsNestedAssembly || type.IsNestedFamORAssem)
            {
                return ProtectionLevel.Internal;
            }

            if (type.IsNestedFamily || type.IsNestedFamANDAssem)
            {
                return ProtectionLevel.Protected;
            }

            if (type.IsNestedPrivate)
            {
                return ProtectionLevel.Private;
            }

            return ProtectionLevel.Internal;
        }

        private static ProtectionLevel GetMethodProtectionLevel(MethodAttributes methodAttributes)
        {
            switch (methodAttributes & MethodAttributes.MemberAccessMask)
            {
                case MethodAttributes.Public:
                    return ProtectionLevel.Public;

                case MethodAttributes.Assembly:
                case MethodAttributes.FamORAssem:
                    return ProtectionLevel.Internal;

                case MethodAttributes.Family:
                case MethodAttributes.FamANDAssem:
                    return ProtectionLevel.Protected;

                case MethodAttributes.Private:
                case MethodAttributes.PrivateScope:
                    return ProtectionLevel.Private;

                default:
                    return ProtectionLevel.Internal;
            }
        }

        private static ProtectionLevel GetFieldProtectionLevel(FieldAttributes fieldAttributes)
        {
            switch (fieldAttributes & FieldAttributes.FieldAccessMask)
            {
                case FieldAttributes.Public:
                    return ProtectionLevel.Public;

                case FieldAttributes.Assembly:
                case FieldAttributes.FamORAssem:
                    return ProtectionLevel.Internal;

                case FieldAttributes.Family:
                case FieldAttributes.FamANDAssem:
                    return ProtectionLevel.Protected;

                case FieldAttributes.Private:
                case FieldAttributes.PrivateScope:
                    return ProtectionLevel.Private;

                default:
                    return ProtectionLevel.Internal;
            }
        }

        /// <summary>
        /// Get the metadata for the constructors of a type.
        /// </summary>
        /// <param name="type">the type to get the constructors of.</param>
        /// <returns>a list of the constructors of the given type.</returns>
        private static IImmutableList<ConstructorMetadata> GetConstructorMetadata(Type type)
        {
            var constructors = new List<ConstructorMetadata>();
            foreach (ConstructorInfo ctor in type.GetConstructors())
            {
                var ctorMetadata = new ConstructorMetadata(
                    GetMethodProtectionLevel(ctor.Attributes),
                    ctor.IsStatic)
                {
                    CustomAttributes = GetCustomAttributes(type.GetCustomAttributesData())
                };
            }

            return constructors.ToImmutableArray();
        }

        /// <summary>
        /// Get the metadata for the fields of a type.
        /// </summary>
        /// <param name="type">the type to get the fields of.</param>
        /// <returns>a dictionary of fields on the type, keyed by name.</returns>
        private static IImmutableDictionary<string, FieldMetadata> GetFieldMetadata(Type type)
        {
            var fields = new Dictionary<string, FieldMetadata>();
            foreach (FieldInfo field in type.GetFields())
            {
                var fieldMetadata = new FieldMetadata(
                    field.Name,
                    GetFieldProtectionLevel(field.Attributes),
                    field.IsStatic)
                {
                    CustomAttributes = GetCustomAttributes(field.GetCustomAttributesData()),
                };
                fields.Add(fieldMetadata.Name, fieldMetadata);
            }
            return fields.ToImmutableDictionary();
        }

        /// <summary>
        /// Get the metadata for the properties of a type.
        /// </summary>
        /// <param name="type">the type to get the properties of.</param>
        /// <returns>a dictionary of properties on the type, keyed by name.</returns>
        private static IImmutableDictionary<string, PropertyMetadata> GetPropertyMetadata(Type type)
        {
            var properties = new Dictionary<string, PropertyMetadata>();
            foreach (PropertyInfo property in type.GetProperties())
            {
                MethodInfo getter = property.GetGetMethod();
                MethodInfo setter = property.GetSetMethod();

                TypeMetadata propertyType = FromType(property.PropertyType);

                var getterMetadata = new PropertyGetterMetadata(
                    property.Name,
                    GetMethodProtectionLevel(getter.Attributes),
                    getter.IsStatic)
                {
                    CustomAttributes = GetCustomAttributes(getter.GetCustomAttributesData()),
                    ReturnType = propertyType
                };

                var setterMetadata = new PropertySetterMetadata(
                    property.Name,
                    GetMethodProtectionLevel(setter.Attributes),
                    setter.IsStatic)
                {
                    CustomAttributes = GetCustomAttributes(setter.GetCustomAttributesData()),
                    ParameterTypes = new [] { propertyType }.ToImmutableArray()
                };

                var propertyMetadata = new PropertyMetadata(
                    property.Name,
                    ProtectionLevel.Public, // TODO: Correct this later
                    getter.IsStatic || setter.IsStatic)
                {
                    CustomAttributes = GetCustomAttributes(property.GetCustomAttributesData()),
                    Getter = getterMetadata,
                    Setter = setterMetadata,
                    Type = propertyType,
                    GenericParameters = ImmutableArray<GenericParameterMetadata>.Empty
                };
            }

            return properties.ToImmutableDictionary();
        }

        /// <summary>
        /// Get the metadata for the methods of a type.
        /// </summary>
        /// <param name="type">the type to get the methods of.</param>
        /// <returns>a dictionary of methods on the type, keyed by name.</returns>
        private static IImmutableDictionary<string, IImmutableList<MethodMetadata>> GetMethodMetadata(Type type)
        {
            var methods = new Dictionary<string, List<MethodMetadata>>();
            foreach (MethodInfo method in type.GetMethods())
            {
                if (!methods.ContainsKey(method.Name))
                {
                    methods.Add(method.Name, new List<MethodMetadata>());
                }

                var parameters = new List<TypeMetadata>();
                foreach (ParameterInfo parameter in method.GetParameters())
                {
                    parameters.Add(FromType(parameter.ParameterType));
                }

                var methodMetadata = new MethodMetadata(
                    method.Name,
                    GetMethodProtectionLevel(method.Attributes),
                    method.IsStatic)
                {
                    CustomAttributes = GetCustomAttributes(method.GetCustomAttributesData()),
                    ReturnType = FromType(method.ReturnType),
                    ParameterTypes = parameters.ToImmutableArray()
                };

                methods[method.Name].Add(methodMetadata);
            }

            var immutableMethods = new Dictionary<string, IImmutableList<MethodMetadata>>();
            foreach (KeyValuePair<string, List<MethodMetadata>> methodEntry in methods)
            {
                immutableMethods.Add(methodEntry.Key, methodEntry.Value.ToImmutableArray());
            }

            return immutableMethods.ToImmutableDictionary();
        }

        private static IImmutableList<EnumMemberMetadata> GetEnumMemberMetadata(Type type, EnumMetadata definingEnum)
        {
            var members = new List<EnumMemberMetadata>();
            foreach (FieldInfo field in type.GetFields())
            {
                if (field.Name == MetadataAnalyzer.DEFAULT_ENUM_MEMBER_NAME)
                {
                    continue;
                }

                IImmutableList<CustomAttributeMetadata> customAttributes = GetCustomAttributes(field.CustomAttributes);
                var enumMember = new EnumMemberMetadata(field.Name)
                {
                    Type = definingEnum,
                    CustomAttributes = customAttributes
                };
                members.Add(enumMember);
            }

            return members.ToImmutableArray();
        }

        private static IImmutableList<GenericParameterMetadata> GetGenericParameters(Type type)
        {
            return null;
        }

        private static IImmutableList<CustomAttributeMetadata> GetCustomAttributes(IEnumerable<CustomAttributeData> customAttributeData)
        {
            var customAttributes = new List<CustomAttributeMetadata>();
            foreach (CustomAttributeData customAttribute in customAttributeData)
            {
                var customAttributeMetadata = new CustomAttributeMetadata(
                    FromType(customAttribute.AttributeType),
                    GetNamedArguments(customAttribute),
                    GetPositionalArguments(customAttribute)
                );

                customAttributes.Add(customAttributeMetadata);
            }

            return customAttributes.ToImmutableArray();
        }

        private static IImmutableDictionary<string, CustomAttributeNamedArgument<TypeMetadata>> 
            GetNamedArguments(CustomAttributeData customAttribute)
        {
            var namedArgs = new Dictionary<string, CustomAttributeNamedArgument<TypeMetadata>>();
            foreach (CustomAttributeNamedArgument namedArg in customAttribute.NamedArguments)
            {
                var na = new CustomAttributeNamedArgument<TypeMetadata>(
                    namedArg.MemberName,
                    namedArg.IsField ? CustomAttributeNamedArgumentKind.Field : CustomAttributeNamedArgumentKind.Property,
                    FromType(namedArg.TypedValue.ArgumentType),
                    namedArg.TypedValue.Value
                );

                namedArgs.Add(na.Name, na);
            }

            return namedArgs.ToImmutableDictionary();
        }

        private static IImmutableList<CustomAttributeTypedArgument<TypeMetadata>> GetPositionalArguments(CustomAttributeData customAttribute)
        {
            var positionalArguments = new List<CustomAttributeTypedArgument<TypeMetadata>>();
            foreach (CustomAttributeTypedArgument positionalArg in customAttribute.ConstructorArguments)
            {
                var pa = new CustomAttributeTypedArgument<TypeMetadata>(
                    FromType(positionalArg.ArgumentType),
                    positionalArg.Value
                );

                positionalArguments.Add(pa);
            }

            return positionalArguments.ToImmutableArray();
        }

        /// <summary>
        /// Get the metadata for the types defined within the type.
        /// </summary>
        /// <param name="type">the type to get the nested types of.</param>
        /// <returns>a dictionary of nested types in the given type, keyed by name.</returns>
        private static IImmutableDictionary<string, DefinedTypeMetadata> GetNestedTypeMetadata(Type type)
        {
            var nestedTypes = new Dictionary<string, DefinedTypeMetadata>();
            foreach (Type nestedType in type.GetNestedTypes())
            {
                DefinedTypeMetadata nestedTypeMetadata = (DefinedTypeMetadata)FromType(nestedType);
                nestedTypes.Add(nestedType.Name, nestedTypeMetadata);
            }
            return nestedTypes.ToImmutableDictionary();
        }
    }
}