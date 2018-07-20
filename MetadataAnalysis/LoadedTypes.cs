using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection.Metadata;
using MetadataAnalysis.Metadata;
using MetadataAnalysis.Metadata.Generic;
using MetadataAnalysis.Metadata.Array;
using MetadataAnalysis.Metadata.Signature;

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
        private static readonly TypeCache s_typeCache;

        /// <summary>
        /// Initialize the loaded type cache with common primitive types.
        /// </summary>
        static LoadedTypes()
        {
            // Initialize the cache
            s_typeCache = new TypeCache();

            // Process the common base types
            Type objectType = typeof(object);
            Type valueType = typeof(ValueType);
            Type enumType = typeof(Enum);
            Type arrayType = typeof(System.Array);

            var objectTypeMetadata = new ClassMetadata(
                objectType.Name,
                objectType.Namespace,
                objectType.FullName,
                ProtectionLevel.Public,
                isAbstract: false,
                isSealed: false
            );

            var valueTypeMetadata = new ClassMetadata(
                valueType.Name,
                valueType.Namespace,
                valueType.FullName,
                ProtectionLevel.Public,
                isAbstract: true,
                isSealed: false
            )
            {
                BaseType = objectTypeMetadata
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
                BaseType = valueTypeMetadata
            };

            var arrayTypeMetadata = new ClassMetadata(
                arrayType.Name,
                arrayType.Namespace,
                arrayType.FullName,
                ProtectionLevel.Public,
                isAbstract: true,
                isSealed: false
            )
            {
                BaseType = objectTypeMetadata
            };

            ObjectTypeMetadata = objectTypeMetadata;
            ValueTypeMetadata  = valueTypeMetadata;
            EnumTypeMetadata   = enumTypeMetadata;
            ArrayTypeMetadata = arrayTypeMetadata;

            s_typeCache.TryAdd(objectType, objectTypeMetadata);
            s_typeCache.TryAdd(valueType, valueTypeMetadata);
            s_typeCache.TryAdd(enumType, enumTypeMetadata);
            s_typeCache.TryAdd(arrayType, arrayTypeMetadata);

            objectTypeMetadata.NestedTypes = GetNestedTypeMetadata(objectType);
            objectTypeMetadata.Constructors = GetConstructorMetadata(objectType);
            objectTypeMetadata.Fields = GetFieldMetadata(objectType);
            objectTypeMetadata.Properties = GetPropertyMetadata(objectType);
            objectTypeMetadata.Methods = GetMethodMetadata(objectType);

            valueTypeMetadata.NestedTypes = GetNestedTypeMetadata(valueType);
            valueTypeMetadata.Constructors = GetConstructorMetadata(valueType);
            valueTypeMetadata.Fields = GetFieldMetadata(valueType);
            valueTypeMetadata.Properties = GetPropertyMetadata(valueType);
            valueTypeMetadata.Methods = GetMethodMetadata(valueType);
            
            enumTypeMetadata.NestedTypes = GetNestedTypeMetadata(enumType);
            enumTypeMetadata.Constructors = GetConstructorMetadata(enumType);
            enumTypeMetadata.Fields = GetFieldMetadata(enumType);
            enumTypeMetadata.Properties = GetPropertyMetadata(enumType);
            enumTypeMetadata.Methods = GetMethodMetadata(enumType);

            TypeTypeMetadata = (ClassMetadata)LoadedTypes.FromType(typeof(Type));
            VoidTypeMetadata = (StructMetadata)LoadedTypes.FromType(typeof(void));

            // Primitive types to load into the type cache
            var primitiveTypes = new []
            {
                typeof(Boolean),
                typeof(Byte),
                typeof(Char),
                typeof(Double),
                typeof(Int16),
                typeof(Int32),
                typeof(Int64),
                typeof(IntPtr),
                typeof(Object),
                typeof(SByte),
                typeof(Single),
                typeof(String),
                typeof(TypedReference),
                typeof(UInt16),
                typeof(UInt32),
                typeof(UInt64),
                typeof(UIntPtr),
                typeof(void)
            };
            foreach (Type primitiveType in primitiveTypes)
            {
                FromType(primitiveType);
            }
        }

        public const string ObjectTypeFullName = "System.Object";

        public const string ValueTypeFullName = "System.ValueType";

        public const string EnumTypeFullName = "System.Enum";

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

        public static StructMetadata VoidTypeMetadata { get; }

        /// <summary>
        /// Get a type metadata object from a system type object.
        /// </summary>
        /// <param name="type">the type for which we want a metadata object.</param>
        /// <returns>an object representing the metadata of the given type.</returns>
        public static TypeMetadata FromType(Type type)
        {
            if (type == null)
            {
                return null;
            }

            // Search the cache first
            if (s_typeCache.TryGetValue(type, out TypeMetadata typeMetadata))
            {
                return typeMetadata;
            }

            // We'll want the declaring type in any case
            DefinedTypeMetadata declaringType = type.DeclaringType == null ? null : (DefinedTypeMetadata)FromType(type.DeclaringType);

            // We differentiate based on what kind of type
            if (type.IsByRef)
            {
                typeMetadata = GetByRefMetadata(type);
            }
            else if (type.IsArray)
            {
                typeMetadata = GetArrayMetadata(type);
            }
            else if (type.IsEnum)
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
            if (String.IsNullOrEmpty(fullTypeName))
            {
                typeMetadata = null;
                return false;
            }

            Type type = Type.GetType(fullTypeName);
            if (type != null)
            {
                typeMetadata = FromType(type);
                return true;
            }

            typeMetadata = null;
            return false;
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

        private static ByRefTypeMetadata GetByRefMetadata(Type byRefType)
        {
            TypeMetadata underlyingType = FromType(byRefType.GetElementType());

            return new ByRefTypeMetadata(underlyingType);
        }

        private static ArrayTypeMetadata GetArrayMetadata(Type arrayType)
        {
            TypeMetadata underlyingType = FromType(arrayType.GetElementType());

            var arrayMetadata = new ArrayTypeMetadata(
                arrayType.Name,
                arrayType.Namespace,
                GetLongTypeName(arrayType),
                GetProtectionLevel(arrayType),
                arrayType.GetArrayRank(),
                lowerBounds: ImmutableArray<int>.Empty,
                sizes: ImmutableArray<int>.Empty)
            {
                UnderlyingType = underlyingType,
                BaseType = LoadedTypes.ArrayTypeMetadata,
            };

            s_typeCache[arrayType] = arrayMetadata;

            arrayMetadata.Constructors = GetConstructorMetadata(arrayType);
            arrayMetadata.CustomAttributes = GetCustomAttributes(arrayType.GetCustomAttributesData());
            arrayMetadata.Fields = GetFieldMetadata(arrayType);
            arrayMetadata.Properties = GetPropertyMetadata(arrayType);
            arrayMetadata.Methods = GetMethodMetadata(arrayType);

            return arrayMetadata;
        }

        private static ClassMetadata GetClassMetadata(Type classType)
        {
            var classMetadata = new ClassMetadata(
                classType.Name,
                classType.Namespace,
                GetLongTypeName(classType),
                GetProtectionLevel(classType),
                classType.IsAbstract,
                classType.IsSealed
            );
            
            s_typeCache[classType] = classMetadata;

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
                GetLongTypeName(structType),
                GetProtectionLevel(structType)
            )
            {
            };

            s_typeCache[structType] = structMetadata;

            structMetadata.DeclaringType = (DefinedTypeMetadata)FromType(structType.DeclaringType);
            structMetadata.Constructors = GetConstructorMetadata(structType);
            structMetadata.Fields = GetFieldMetadata(structType);
            structMetadata.Properties = GetPropertyMetadata(structType);
            structMetadata.Methods = GetMethodMetadata(structType);

            return structMetadata;
        }

        private static EnumMetadata GetEnumMetadata(Type enumType)
        {
            object value = Enum.GetValues(enumType).GetValue(0);
            MethodInfo typeCodeMethod = enumType.GetMethod("GetTypeCode");

            var typeCode = (TypeCode)typeCodeMethod.Invoke(value, new object[0]);

            var enumMetadata = new EnumMetadata(
                enumType.Name,
                enumType.Namespace,
                GetLongTypeName(enumType),
                GetProtectionLevel(enumType),
                typeCode
            );

            s_typeCache[enumType] = enumMetadata;

            enumMetadata.DeclaringType = (DefinedTypeMetadata)FromType(enumType.DeclaringType);
            enumMetadata.Members = GetEnumMemberMetadata(enumType, enumMetadata);
            enumMetadata.Fields = GetFieldMetadata(enumType);
            enumMetadata.Properties = GetPropertyMetadata(enumType);
            enumMetadata.Methods = GetMethodMetadata(enumType);
            enumMetadata.CustomAttributes = GetCustomAttributes(enumType.GetCustomAttributesData());

            return enumMetadata;
        }

        private static string GetLongTypeName(Type type)
        {
            if (type.DeclaringType != null)
            {
                return GetLongTypeName(type.DeclaringType) + "." + type.Name;
            }

            if (String.IsNullOrEmpty(type.Namespace))
            {
                return type.Name;
            }

            return type.Namespace + "." + type.Name;
        }

        /// <summary>
        /// Get the accessibility level of a type.
        /// </summary>
        /// <param name="type">the type to get the accessibility of.</param>
        /// <returns>the accessibility level of the type.</returns>
        private static ProtectionLevel GetProtectionLevel(Type type)
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

        private static ProtectionLevel GetProtectionLevel(MethodAttributes methodAttributes)
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

        private static ProtectionLevel GetProtectionLevel(FieldAttributes fieldAttributes)
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
                    GetProtectionLevel(ctor.Attributes),
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
                    GetProtectionLevel(field.Attributes),
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

                var getterMetadata = getter == null ? null : new PropertyGetterMetadata(
                    property.Name,
                    GetProtectionLevel(getter.Attributes),
                    getter.IsStatic)
                {
                    CustomAttributes = GetCustomAttributes(getter.GetCustomAttributesData()),
                    ReturnType = propertyType
                };

                var setterMetadata = setter == null ? null : new PropertySetterMetadata(
                    property.Name,
                    GetProtectionLevel(setter.Attributes),
                    setter.IsStatic)
                {
                    CustomAttributes = GetCustomAttributes(setter.GetCustomAttributesData()),
                    ParameterTypes = new [] { propertyType }.ToImmutableArray()
                };

                var propertyMetadata = new PropertyMetadata(
                    property.Name,
                    ProtectionLevel.Public, // TODO: Correct this later
                    ReadPropertyIsStatic(getter, setter))
                {
                    CustomAttributes = GetCustomAttributes(property.GetCustomAttributesData()),
                    Getter = getterMetadata,
                    Setter = setterMetadata,
                    Type = propertyType,
                    GenericParameters = ImmutableArray<GenericParameterMetadata>.Empty
                };

                properties.Add(property.Name, propertyMetadata);
            }

            return properties.ToImmutableDictionary();
        }

        private static bool ReadPropertyIsStatic(MethodInfo getter, MethodInfo setter)
        {
            if (getter != null)
            {
                return getter.IsStatic;
            }

            if (setter != null)
            {
                return setter.IsStatic;
            }

            return false;
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
                    GetProtectionLevel(method.Attributes),
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

        private class TypeCache
        {
            private readonly Dictionary<Type, TypeMetadata> _typeTable;

            private readonly Dictionary<string, Type> _typeNameTable;

            private readonly object _lockObj;

            public TypeCache()
            {
                _lockObj = new object();
                _typeTable = new Dictionary<Type, TypeMetadata>();
                _typeNameTable = new Dictionary<string, Type>();
            }

            public bool TryAdd(Type type, TypeMetadata typeMetadata)
            {
                lock(_lockObj)
                {
                    if (_typeNameTable.TryAdd(GetLongTypeName(type), type))
                    {
                        return _typeTable.TryAdd(type, typeMetadata);
                    }
                }

                return false;
            }

            public bool TryGetValue(Type type, out TypeMetadata typeMetadata)
            {
                lock (_lockObj)
                {
                    if (_typeTable.TryGetValue(type, out typeMetadata))
                    {
                        return true;
                    }

                    if (_typeNameTable.TryGetValue(GetLongTypeName(type), out Type precedingType))
                    {
                        typeMetadata = _typeTable[precedingType];
                        return true;
                    }
                }

                return false;
            }

            public TypeMetadata this[Type type]
            {
                get
                {
                    string longName;
                    lock (_lockObj)
                    {
                        if (_typeTable.ContainsKey(type))
                        {
                            return _typeTable[type];
                        }

                        longName = GetLongTypeName(type);
                        if (_typeNameTable.ContainsKey(longName))
                        {
                            return _typeTable[_typeNameTable[longName]];
                        }
                    }

                    throw new KeyNotFoundException($"No such type in cache: '{longName}'");
                }

                set
                {
                    lock (_lockObj)
                    {
                        _typeTable[type] = value;
                        _typeNameTable[GetLongTypeName(type)] = type;
                    }
                }
            }
        }
    }
}