using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using MetadataAnalysis.Metadata;
using MetadataAnalysis.Metadata.Generic;
using MetadataAnalysis.Metadata.TypeProviders;

namespace MetadataAnalysis
{
    /// <summary>
    /// Class to read IL metadata into a structured object hierarchy
    /// using the System.Reflection.Metadata metadata reader.
    /// </summary>
    public class MetadataAnalyzer : IDisposable
    {
        public static TypeCode ConvertEnumUnderlyingTypeCode(PrimitiveTypeCode typeCode)
        {
            switch (typeCode)
            {
                case PrimitiveTypeCode.Boolean:
                    return TypeCode.Boolean;
                case PrimitiveTypeCode.Byte:
                    return TypeCode.Byte;
                case PrimitiveTypeCode.Char:
                    return TypeCode.Char;
                case PrimitiveTypeCode.Double:
                    return TypeCode.Double;
                case PrimitiveTypeCode.Int16:
                    return TypeCode.Int16;
                case PrimitiveTypeCode.Int32:
                    return TypeCode.Int32;
                case PrimitiveTypeCode.Int64:
                    return TypeCode.Int64;
                case PrimitiveTypeCode.Object:
                    return TypeCode.Object;
                case PrimitiveTypeCode.SByte:
                    return TypeCode.SByte;
                case PrimitiveTypeCode.Single:
                    return TypeCode.Single;
                case PrimitiveTypeCode.String:
                    return TypeCode.String;
                case PrimitiveTypeCode.UInt16:
                    return TypeCode.UInt16;
                case PrimitiveTypeCode.UInt32:
                    return TypeCode.UInt32;
                case PrimitiveTypeCode.UInt64:
                    return TypeCode.UInt64;
                default:
                    throw new Exception($"Unrepresentable enum type: '{typeCode}'");
            }
        }

        public const string DEFAULT_ENUM_MEMBER_NAME = "value__";

        /// <summary>
        /// The name of the global class the CLR uses to store
        /// standalone functions in languages that support them.
        /// </summary>
        private const string MODULE_TYPE_NAME = "<Module>";

        private const string CONSTRUCTOR_NAME = ".ctor";

        /// <summary>
        /// The portable executable reader required to
        /// read metadata (not used directly, but must not be disposed while we are reading).
        /// </summary>
        private readonly PEReader _peReader;

        /// <summary>
        /// The metadata reader to parse the IL binary metadata.
        /// </summary>
        private readonly MetadataReader _mdReader;

        private readonly ISignatureTypeProvider<TypeMetadata, TypeMetadataGenericContext> _signatureProvider;

        /// <summary>
        /// A cache of types we have already seen, which can be
        /// preloaded to emulate linked DLLs.
        /// </summary>
        private readonly Dictionary<string, TypeMetadata> _typeMetadataCache;

        /// <summary>
        /// Construct a MetadataAnalyzer around a portable executable reader instance.
        /// </summary>
        /// <param name="peReader">the portable executable reader to read metadata from.</param>
        public MetadataAnalyzer(PEReader peReader)
        {
            _peReader = peReader;

            if (!_peReader.HasMetadata)
            {
                throw new BadImageFormatException("No metadata in given portable executable reader");
            }

            _mdReader = _peReader.GetMetadataReader();
            _signatureProvider = new TypeSignatureProvider(this);
            // Start with an empty cache
            _typeMetadataCache = new Dictionary<string, TypeMetadata>();
        }

        /// <summary>
        /// Construct a MetadataAnalyzer around a portable executable reader
        /// using a pre-loaded collection of type metadata objects, so that
        /// multiple DLLs can be parsed and the information passed around.
        /// </summary>
        /// <param name="peReader">the portable executable reader wrapping the DLL to parse.</param>
        /// <param name="knownTypes">already parsed types that the IL we are parsing may depend on.</param>
        public MetadataAnalyzer(PEReader peReader, ICollection<TypeMetadata> knownTypes)
        {
            _peReader = peReader;
            _mdReader = _peReader.GetMetadataReader();
            _typeMetadataCache = knownTypes.ToDictionary(tm => tm.FullName, tm => tm);
        }

        /// <summary>
        /// Get the assembly defined by the portable executable wrapped by the metadata analyzer.
        /// </summary>
        /// <returns>
        /// a readonly metadata object representing the entirety of the assembly and
        /// all the types defined within it.
        /// </returns>
        public AssemblyMetadata GetDefinedAssembly()
        {
            AssemblyDefinition asmDef = _mdReader.GetAssemblyDefinition();
            
            string name = _mdReader.GetString(asmDef.Name);
            string culture = _mdReader.GetString(asmDef.Culture);
            ImmutableArray<byte> publicKey = _mdReader.GetBlobContent(asmDef.PublicKey);

            return new AssemblyMetadata(
                name,
                asmDef.Version,
                culture,
                asmDef.Flags,
                publicKey,
                asmDef.HashAlgorithm,
                GetAssemblyCustomAttributes(),
                GetAllTypeDefinitions()
            );
        }

        /// <summary>
        /// Retrieve all the custom attributes on the wrapped assembly.
        /// </summary>
        /// <returns>the custom attributes on the wrapped assembly.</returns>
        public IImmutableList<CustomAttributeMetadata> GetAssemblyCustomAttributes()
        {
            return null;
        }

        /// <summary>
        /// Get all the metadata of all the types defined in the wrapped assembly.
        /// </summary>
        /// <returns>
        /// a dictionary of the metadata of all types defined in the wrapped assembly,
        /// keyed by the full name of each type.
        /// </returns>
        public IImmutableDictionary<string, DefinedTypeMetadata> GetAllTypeDefinitions()
        {
            return ReadTypesFromHandles(_mdReader.TypeDefinitions);
        }

        /// <summary>
        /// Get all the metadata of all the named types in the wrapped assembly.
        /// </summary>
        /// <param name="fullTypeNames">the full names of all the types to get metadata of.</param>
        /// <returns>
        /// a dictionary of the metadata of all types defined in the wrapped assembly matching the
        /// given names, keyed by the full type names.
        /// </returns>
        public IImmutableDictionary<string, DefinedTypeMetadata> GetTypeDefinitions(IReadOnlyCollection<string> fullTypeNames)
        {
            // If the parameter is bad, throw an exception
            if (fullTypeNames == null)
            {
                throw new ArgumentNullException(nameof(fullTypeNames));
            }

            int i = 0;
            foreach (string fullTypeName in fullTypeNames)
            {
                if (String.IsNullOrWhiteSpace(fullTypeName))
                {
                    throw new ArgumentException(String.Format("At element {0}: Type name cannot be null or whitespace", i));
                }
                i++;
            }
            
            // Prevent needless work if there's nothing to process
            if (fullTypeNames.Count == 0)
            {
                return ImmutableDictionary<string, DefinedTypeMetadata>.Empty;
            }
            
            // Make a set of all the types we're looking for
            // so that searching for them is easier
            var typeNameSet = fullTypeNames.ToHashSet();

            // Go through the types in the DLL until we have all the ones we're looking for
            var typeDefinitions = new Dictionary<string, DefinedTypeMetadata>();
            foreach (TypeDefinitionHandle tdHandle in _mdReader.TypeDefinitions)
            {
                // If there are no more types to look for, we're done
                if (typeNameSet.Count == 0)
                {
                    break;
                }

                // We can't read bad type handles, so we need to protect ourselves
                if (tdHandle.IsNil)
                {
                    continue;
                }

                TypeDefinition typeDef = _mdReader.GetTypeDefinition(tdHandle);
                string fullTypeName = GetFullTypeName(typeDef);
                if (typeNameSet.Contains(fullTypeName))
                {
                    // If the type is a nested type, we don't support that (yet)
                    // TODO: Come up with a better way to seek out nested types
                    if (!typeDef.GetDeclaringType().IsNil)
                    {
                        continue;
                    }

                    DefinedTypeMetadata typeMetadata = ReadTypeMetadata(typeDef);
                    typeDefinitions.Add(typeMetadata.FullName, typeMetadata);
                    // Reduce the search set since we found something
                    typeNameSet.Remove(fullTypeName);
                }
            }

            return typeDefinitions.ToImmutableDictionary();
        }

        /// <summary>
        /// Get the metadata for a single type.
        /// </summary>
        /// <param name="fullTypeName">the full name of the type to find.</param>
        /// <returns>the metadata for the named type, or null if it's not found.</returns>
        public DefinedTypeMetadata GetTypeDefinition(string fullTypeName)
        {
            // A type can't have a whitespace name
            if (String.IsNullOrWhiteSpace(fullTypeName))
            {
                throw new ArgumentException("Type name can't be null or whitespace", nameof(fullTypeName));
            }

            foreach (TypeDefinitionHandle tdHandle in _mdReader.TypeDefinitions)
            {
                // Just skip over nil type handles, although this shouldn't happen
                if (tdHandle.IsNil)
                {
                    continue;
                }

                TypeDefinition typeDef = _mdReader.GetTypeDefinition(tdHandle);
                string tdName = _mdReader.GetString(typeDef.Name);

                // If this is not the type we're looking for, move on
                if (tdName != fullTypeName)
                {
                    continue;
                }

                return ReadTypeMetadata(typeDef);
            }

            // If we didn't find the type, return null
            return null;
        }

        /// <summary>
        /// Read all the types in an enumerable of TypeDefinitionHandles into
        /// type metadata objects.
        /// </summary>
        /// <param name="tdHandles">the type definition handles to read into metadata objects.</param>
        /// <param name="declaringType">the type declaring the types we are reading, if any.</param>
        /// <returns>a dictionary of the types we are reading, keyed by the full names of types.</returns>
        internal IImmutableDictionary<string, DefinedTypeMetadata> ReadTypesFromHandles(
            IEnumerable<TypeDefinitionHandle> tdHandles,
            TypeMetadata declaringType = null)
        {
            var typeDefs = new Dictionary<string, DefinedTypeMetadata>();
            foreach (TypeDefinitionHandle tdHandle in tdHandles)
            {
                // Avoid bad type def handles. This should not occur though.
                if (tdHandle.IsNil)
                {
                    continue;
                }

                TypeDefinition typeDef = _mdReader.GetTypeDefinition(tdHandle);

                // Don't read nested types at the top level; we should pick them up when we read the parents
                if (!typeDef.GetDeclaringType().IsNil)
                {
                    continue;
                }

                DefinedTypeMetadata typeMetadata = ReadTypeMetadata(typeDef);
                typeDefs.Add(typeMetadata.FullName, typeMetadata);
            }
            return typeDefs.ToImmutableDictionary();
        }

        /// <summary>
        /// Read the metadata from a given type definition
        /// </summary>
        /// <param name="typeDef">the type definition entry from the metadata reader</param>
        /// <param name="declaringType">the parent type of the type definition being read</param>
        /// <returns>the type metadata object describing the type definition read in</returns>
        internal DefinedTypeMetadata ReadTypeMetadata(TypeDefinition typeDef, TypeMetadata declaringType = null)
        { 
            string fullTypeName = ReadFullTypeName(typeDef, out string name, out string @namespace);

            // Look for the type in the cache first to see if we know it
            if (_typeMetadataCache.ContainsKey(fullTypeName))
            {
                return (DefinedTypeMetadata)_typeMetadataCache[fullTypeName];
            }

            TypeKind typeKind = GetTypeKindFromBase(typeDef.BaseType);
            switch (typeKind)
            {
                case TypeKind.Class:
                    return ReadClassDefinition(typeDef, name, @namespace, fullTypeName, declaringType);
                case TypeKind.Struct:
                    return ReadStructDefinition(typeDef, name, @namespace, fullTypeName, declaringType);
                case TypeKind.Enum:
                    return ReadEnumDefinition(typeDef, name, @namespace, fullTypeName, declaringType);
                default:
                    throw new Exception($"Unable to parse metadata for type of kind: {typeKind}");
            }
        }

        private ClassMetadata ReadClassDefinition(
            TypeDefinition typeDef,
            string name,
            string @namespace,
            string fullName,
            TypeMetadata declaringType = null)
        {
            ProtectionLevel protectionLevel = ReadProtectionLevel(typeDef.Attributes);
            bool isAbstract = ReadAbstract(typeDef.Attributes);
            bool isSealed = ReadSealed(typeDef.Attributes);

            var classMetadata = new ClassMetadata(
                name,
                @namespace,
                fullName,
                protectionLevel,
                isAbstract,
                isSealed)
            {
                DeclaringType = (DefinedTypeMetadata)declaringType
            };

            _typeMetadataCache.Add(fullName, classMetadata);

            classMetadata.BaseType = ReadBaseType(typeDef.BaseType);
            classMetadata.Fields = ReadFieldMetadata(typeDef.GetFields());
            classMetadata.Properties = ReadPropertyMetadata(typeDef.GetProperties());
            (classMetadata.Constructors, classMetadata.Methods) = ReadMethodMetadata(typeDef.GetMethods());
            classMetadata.NestedTypes = ReadTypesFromHandles(typeDef.GetNestedTypes(), declaringType: classMetadata);
            classMetadata.GenericParameters = ReadGenericParameters(typeDef.GetGenericParameters());
            classMetadata.CustomAttributes = ReadCustomAttributes(typeDef.GetCustomAttributes());

            return classMetadata;
        }

        private StructMetadata ReadStructDefinition(
            TypeDefinition typeDef,
            string name,
            string @namespace,
            string fullName,
            TypeMetadata declaringType = null)
        {
            ProtectionLevel protectionLevel = ReadProtectionLevel(typeDef.Attributes);
            bool isAbstract = ReadAbstract(typeDef.Attributes);
            bool isSealed = ReadSealed(typeDef.Attributes);

            var structMetadata = new StructMetadata(
                name,
                @namespace,
                fullName,
                protectionLevel)
            {
                DeclaringType = (DefinedTypeMetadata)declaringType
            };

            _typeMetadataCache.Add(fullName, structMetadata);

            structMetadata.BaseType = ReadBaseType(typeDef.BaseType);
            structMetadata.Fields = ReadFieldMetadata(typeDef.GetFields());
            structMetadata.Properties = ReadPropertyMetadata(typeDef.GetProperties());
            (structMetadata.Constructors, structMetadata.Methods) = ReadMethodMetadata(typeDef.GetMethods());
            structMetadata.NestedTypes = ReadTypesFromHandles(typeDef.GetNestedTypes(), declaringType: structMetadata);
            structMetadata.GenericParameters = ReadGenericParameters(typeDef.GetGenericParameters());
            structMetadata.CustomAttributes = ReadCustomAttributes(typeDef.GetCustomAttributes());

            return structMetadata;
        }

        private EnumMetadata ReadEnumDefinition(
            TypeDefinition typeDef,
            string name,
            string @namespace,
            string fullName,
            TypeMetadata declaringType = null)
        {
            ProtectionLevel protectionLevel = ReadProtectionLevel(typeDef.Attributes);
            bool isAbstract = ReadAbstract(typeDef.Attributes);
            bool isSealed = ReadSealed(typeDef.Attributes);

            var members = new List<EnumMemberMetadata>();
            PrimitiveTypeCode underlyingEnumType = PrimitiveTypeCode.Int32;
            foreach (FieldMetadata field in ReadFieldMetadata(typeDef.GetFields()).Values)
            {
                if (field.Name == DEFAULT_ENUM_MEMBER_NAME)
                {
                    underlyingEnumType = LoadedTypes.GetPrimitiveTypeCode(Type.GetType(field.Type.FullName));
                    continue;
                }

                members.Add(new EnumMemberMetadata(field.Name));
            }

            var enumMetadata = new EnumMetadata(
                name,
                @namespace,
                fullName,
                protectionLevel,
                ConvertEnumUnderlyingTypeCode(underlyingEnumType))
            {
                DeclaringType = (DefinedTypeMetadata)declaringType
            };

            _typeMetadataCache.Add(fullName, enumMetadata);
            enumMetadata.BaseType = LoadedTypes.EnumTypeMetadata;
            enumMetadata.Fields = ReadFieldMetadata(typeDef.GetFields());
            enumMetadata.Properties = ReadPropertyMetadata(typeDef.GetProperties());
            (enumMetadata.Constructors, enumMetadata.Methods) = ReadMethodMetadata(typeDef.GetMethods());
            enumMetadata.NestedTypes = ReadTypesFromHandles(typeDef.GetNestedTypes(), declaringType: enumMetadata);
            enumMetadata.GenericParameters = ReadGenericParameters(typeDef.GetGenericParameters());
            enumMetadata.CustomAttributes = ReadCustomAttributes(typeDef.GetCustomAttributes());
            enumMetadata.Members = members.ToImmutableArray();

            return enumMetadata;
        }

        private TypeKind GetTypeKindFromBase(EntityHandle baseTypeHandle)
        {
            if (baseTypeHandle.IsNil)
            {
                return TypeKind.Class;
            }

            switch (baseTypeHandle.Kind)
            {
                case HandleKind.TypeDefinition:
                    return GetTypeKindFromBase(_mdReader.GetTypeDefinition((TypeDefinitionHandle)baseTypeHandle));
                
                case HandleKind.TypeReference:
                    return GetTypeKindFromBase(_mdReader.GetTypeReference((TypeReferenceHandle)baseTypeHandle));

                default:
                    throw new Exception($"Unable to get base type kind from handle of kind: {baseTypeHandle.Kind}");
            }
        }

        private TypeKind GetTypeKindFromBase(TypeDefinition typeDef)
        {
            return GetTypeKindFromBase(typeDef.BaseType);
        }

        private TypeKind GetTypeKindFromBase(TypeReference typeRef)
        {
            string fullName = GetFullTypeName(typeRef);

            if (String.Equals(fullName, LoadedTypes.ObjectTypeMetadata.FullName))
            {
                return TypeKind.Class;
            }
            else if (String.Equals(fullName, LoadedTypes.ValueTypeMetadata.FullName))
            {
                return TypeKind.Struct;
            }
            else if (String.Equals(fullName, LoadedTypes.EnumTypeMetadata.FullName))
            {
                return TypeKind.Enum;
            }

            if (TryGetCachedType(fullName, out TypeMetadata typeMetadata))
            {
                return GetBaseTypeKind(typeMetadata);
            }

            throw new Exception($"Unable to find base type of type: {fullName}");
        }

        /// <summary>
        /// Try to find a type reference in the caches
        /// </summary>
        /// <param name="typeRef">the type reference to find a metadata entry for</param>
        /// <param name="typeMetadata">the type metadata entry to pass out</param>
        /// <returns>true if the type was found, false otherwise</returns>
        internal bool TryLookupTypeReference(TypeReference typeRef, out TypeMetadata typeMetadata)
        {
            // The full type name
            string typeName = _mdReader.GetString(typeRef.Name);
            string typeNamespace = _mdReader.GetString(typeRef.Namespace);
            string fullTypeName = typeNamespace + "." + typeName;

            if (TryGetCachedType(fullTypeName, out typeMetadata))
            {
                return true;
            }
            return false;
        }

        internal TypeMetadata GetGenericInstantiation(
            TypeMetadata type,
            IImmutableList<TypeMetadata> genericArguments)
        {
            string genericName = GetGenericTypeFullName(type, genericArguments);

            if (TryGetCachedType(genericName, out TypeMetadata genericInstantiation))
            {
                return genericInstantiation;
            }

            genericInstantiation = type.InstantiateGenerics(genericArguments);
            _typeMetadataCache.Add(genericName, genericInstantiation);
            return genericInstantiation;
        }

        private string GetGenericTypeFullName(
            TypeMetadata type,
            IImmutableList<TypeMetadata> genericArguments)
        {
            if (!genericArguments.Any())
            {
                return type.FullName;
            }

            var sb = new StringBuilder();
            sb.Append(type.FullName);
            sb.Append("[");
            for (int i = 0; i < genericArguments.Count - 1; i++)
            {
                sb.Append(genericArguments[i].FullName);
                sb.Append(",");
            }
            sb.Append(genericArguments[genericArguments.Count-1]);
            sb.Append("]");

            return sb.ToString();
        }

        private string ReadFullTypeName(TypeDefinition typeDef, out string name, out string @namespace)
        {
            name = _mdReader.GetString(typeDef.Name);


            TypeDefinitionHandle declaringTypeHandle = typeDef.GetDeclaringType();
            if (!declaringTypeHandle.IsNil)
            {
                string declaringName = ReadFullTypeName(declaringTypeHandle);
                @namespace  = null;
                return declaringName + "." + name;
            }

            @namespace = _mdReader.GetString(typeDef.Namespace);
            return String.IsNullOrEmpty(@namespace) ? name : @namespace + "." + name;
        }

        private string ReadFullTypeName(EntityHandle baseTypeHandle)
        {
            switch (baseTypeHandle.Kind)
            {
                case HandleKind.TypeDefinition:
                    return ReadFullTypeName(_mdReader.GetTypeDefinition((TypeDefinitionHandle)baseTypeHandle));
                
                case HandleKind.TypeReference:
                    return ReadFullTypeName(_mdReader.GetTypeReference((TypeReferenceHandle)baseTypeHandle));

                default:
                    throw new Exception($"Unable to read type from handle of kind: {baseTypeHandle.Kind}");
            }
        }

        private string ReadFullTypeName(TypeDefinition typeDef)
        {
            if (!typeDef.BaseType.IsNil)
            {
                string baseName = ReadFullTypeName(typeDef.BaseType);
                return baseName + "." + _mdReader.GetString(typeDef.Name);
            }

            return _mdReader.GetString(typeDef.Namespace) + "." + _mdReader.GetString(typeDef.Name);
        }

        private string ReadFullTypeName(TypeReference typeRef)
        {
            return _mdReader.GetString(typeRef.Namespace) + "." + _mdReader.GetString(typeRef.Name);
        }


        /// <summary>
        /// Read the type metadata of the base type of a type definition.
        /// The handle for this can come in different types, so we do some logic here.
        /// </summary>
        /// <param name="baseTypeHandle">the handle of the base type to read.</param>
        /// <returns>a type metadata object describing the base type.</returns>
        private TypeMetadata ReadBaseType(EntityHandle baseTypeHandle)
        {
            // If the handle is nil, there is no base type
            if (baseTypeHandle.IsNil)
            {
                return null;
            }

            switch (baseTypeHandle.Kind)
            {
                case HandleKind.TypeDefinition:
                    TypeDefinition typeDef = _mdReader.GetTypeDefinition((TypeDefinitionHandle)baseTypeHandle);
                    return ReadTypeMetadata(typeDef);

                case HandleKind.TypeReference:
                    TypeReference typeRef = _mdReader.GetTypeReference((TypeReferenceHandle)baseTypeHandle);
                    // Look up type references in the cache
                    // TODO: Handle this better if we don't find the reference
                    TryLookupTypeReference(typeRef, out TypeMetadata typeMetadata);
                    return typeMetadata;

                default:
                    throw new Exception(String.Format("Unrecognized entity handle for type: '{0}'", baseTypeHandle));
            }
        }

        private IImmutableList<GenericParameterMetadata> ReadGenericParameters(
            IEnumerable<GenericParameterHandle> genericParameterHandles)
        {
            var uninstantiatedParameters = new List<GenericParameterMetadata>();
            foreach (GenericParameterHandle gpHandle in genericParameterHandles)
            {
                GenericParameter genericParameter = _mdReader.GetGenericParameter(gpHandle);

                string parameterName = _mdReader.GetString(genericParameter.Name);

                GenericParameterMetadata genericParameterMetadata;
                if (TryGetCachedType(parameterName, out TypeMetadata typeMetadata))
                {
                    genericParameterMetadata = new ConcreteGenericParameterMetadata(
                        typeMetadata,
                        genericParameter.Attributes);
                }
                else
                {
                    genericParameterMetadata = new UninstantiatedGenericParameterMetadata(
                        parameterName,
                        genericParameter.Attributes);
                }

                uninstantiatedParameters.Add(genericParameterMetadata);
            }

            return uninstantiatedParameters.ToImmutableArray();
        }

        /// <summary>
        /// Read the metadata information of a collection of field handles.
        /// </summary>
        /// <param name="fieldHandles">the field handles to read metadata information of.</param>
        /// <returns>a dictionary of field metadata information keyed by field name.</returns>
        private IImmutableDictionary<string, FieldMetadata> ReadFieldMetadata(IEnumerable<FieldDefinitionHandle> fieldHandles)
        {
            var fields = new Dictionary<string, FieldMetadata>();
            foreach (FieldDefinitionHandle fdHandle in fieldHandles)
            {
                // Ignore bad field handles
                if (fdHandle.IsNil)
                {
                    continue;
                }

                FieldDefinition fieldDef = _mdReader.GetFieldDefinition(fdHandle);

                var fieldMetadata = new FieldMetadata(
                    _mdReader.GetString(fieldDef.Name),
                    ReadProtectionLevel(fieldDef.Attributes),
                    ReadStatic(fieldDef.Attributes)
                )
                {
                    // TODO: Decode the field signature. This may be of the object's type itself...
                    Type = fieldDef.DecodeSignature(_signatureProvider, null)
                };
                fields.Add(fieldMetadata.Name, fieldMetadata);
            }

            return fields.ToImmutableDictionary();
        }

        /// <summary>
        /// Read the metadata information of a collection of property handles.
        /// </summary>
        /// <param name="propertyHandles">the property handles to read metadata from.</param>
        /// <returns>a dictionary of the property metadata objects, keyed by property name.</returns>
        private IImmutableDictionary<string, PropertyMetadata> ReadPropertyMetadata(IEnumerable<PropertyDefinitionHandle> propertyHandles)
        {
            var properties = new Dictionary<string, PropertyMetadata>();
            foreach (PropertyDefinitionHandle propHandle in propertyHandles)
            {
                PropertyDefinition propertyDef = _mdReader.GetPropertyDefinition(propHandle);
                PropertyAccessors accessors = propertyDef.GetAccessors();

                string propertyName = _mdReader.GetString(propertyDef.Name);

                MethodSignature<TypeMetadata> propertySignature = propertyDef.DecodeSignature(_signatureProvider, null);

                PropertyGetterMetadata getter = null;
                if (!accessors.Getter.IsNil)
                {
                    MethodDefinition getterDef = _mdReader.GetMethodDefinition(accessors.Getter);

                    getter = new PropertyGetterMetadata(
                        propertyName,
                        ReadProtectionLevel(getterDef.Attributes),
                        ReadStatic(getterDef.Attributes)
                    );
                }

                PropertySetterMetadata setter = null;
                if (!accessors.Setter.IsNil)
                {
                    MethodDefinition setterDef = _mdReader.GetMethodDefinition(accessors.Setter);

                    setter = new PropertySetterMetadata(
                        propertyName,
                        ReadProtectionLevel(setterDef.Attributes),
                        ReadStatic(setterDef.Attributes)
                    );
                }

                bool isPropertyStatic = false;
                if (getter != null)
                {
                    isPropertyStatic |= getter.IsStatic;
                }
                if (setter != null)
                {
                    isPropertyStatic &= setter.IsStatic;
                }

                var propertyMetadata = new PropertyMetadata(
                    propertyName,
                    GetHighestProtectionLevel(getter?.ProtectionLevel ?? ProtectionLevel.Private, setter?.ProtectionLevel ?? ProtectionLevel.Private),
                    isPropertyStatic)
                {
                    Getter = getter,
                    Setter = setter,
                    CustomAttributes = ReadCustomAttributes(propertyDef.GetCustomAttributes()),
                };

                properties.Add(propertyMetadata.Name, propertyMetadata);
            }

            return properties.ToImmutableDictionary();
        }

        /// <summary>
        /// Read the constructor and method metadata information from a collection of method handles.
        /// </summary>
        /// <param name="methodHandles">the method handles to read metadata information from.</param>
        /// <returns>
        /// a tuple with the first element being a list of constructor metadata object and the second being
        /// a dictionary of method metadata objects keyed by method name.
        /// </returns>
        private (IImmutableList<ConstructorMetadata>, IImmutableDictionary<string, IImmutableList<MethodMetadata>>)
            ReadMethodMetadata(IEnumerable<MethodDefinitionHandle> methodHandles)
        {
            var ctors = new List<ConstructorMetadata>();
            var methods = new Dictionary<string, List<MethodMetadata>>();

            foreach (MethodDefinitionHandle methodHandle in methodHandles)
            {
                MethodDefinition methodDef = _mdReader.GetMethodDefinition(methodHandle);
                string methodName = _mdReader.GetString(methodDef.Name);
                MethodSignature<TypeMetadata> signature = methodDef.DecodeSignature(_signatureProvider, null);

                if (methodName == CONSTRUCTOR_NAME)
                {
                    var ctor = new ConstructorMetadata(
                        ReadProtectionLevel(methodDef.Attributes),
                        ReadStatic(methodDef.Attributes),
                        ReadCustomAttributes(methodDef.GetCustomAttributes()))
                    {
                        ReturnType = signature.ReturnType,
                        ParameterTypes = signature.ParameterTypes
                    };

                    ctors.Add(ctor);
                    continue;
                }

                if (!methods.ContainsKey(methodName))
                {
                    methods.Add(methodName, new List<MethodMetadata>());
                }

                var method = new MethodMetadata(
                    methodName,
                    ReadProtectionLevel(methodDef.Attributes),
                    ReadStatic(methodDef.Attributes))
                {
                    CustomAttributes = ReadCustomAttributes(methodDef.GetCustomAttributes()),
                    ParameterTypes = signature.ParameterTypes,
                    ReturnType = signature.ReturnType,
                };

                methods[method.Name].Add(method);
            }

            var immutableMethods = new Dictionary<string, IImmutableList<MethodMetadata>>(methods.Count);
            foreach (KeyValuePair<string, List<MethodMetadata>> methodEntry in methods)
            {
                immutableMethods.Add(methodEntry.Key, methodEntry.Value.ToImmutableArray());
            }

            return (
                ctors.ToImmutableArray(),
                immutableMethods.ToImmutableDictionary()
            );
        }

        /// <summary>
        /// Read the custom attributes pointed to by a custom attribute handle collection.
        /// </summary>
        /// <param name="customAttributeHandles">The handles for custom attributes to be read.</param>
        /// <returns>An immutable list of metadata objects describing the custom attributes in the handle collection.</returns>
        private IImmutableList<CustomAttributeMetadata> ReadCustomAttributes(
            IEnumerable<CustomAttributeHandle> customAttributeHandles)
        {
            var customAttributes = new List<CustomAttributeMetadata>();
            var typeProvider = new CustomAttributeTypeMetadataProvider(this);

            foreach (var customAttributeHandle in customAttributeHandles)
            {
                CustomAttribute customAttribute = _mdReader.GetCustomAttribute(customAttributeHandle);
                TypeMetadata customAttributeType = GetCustomAttributeFromCtor(customAttribute.Constructor);
                CustomAttributeValue<TypeMetadata> customAttributeValue = customAttribute.DecodeValue<TypeMetadata>(typeProvider);

                var customAttributeMetadata = new CustomAttributeMetadata(
                    customAttributeType,
                    customAttributeValue.NamedArguments.ToImmutableDictionary(na => na.Name),
                    customAttributeValue.FixedArguments.ToImmutableArray()
                );
                customAttributes.Add(customAttributeMetadata);
            }

            return customAttributes.ToImmutableArray();
        }

        /// <summary>
        /// Get the custom attribute metadata from a handle to one of its constructors.
        /// </summary>
        /// <param name="ctorHandle">A constructor handle for the custom attribute to be read.</param>
        /// <returns>A metadata object describing the custom attribute to which the constructor belongs.</returns>
        private TypeMetadata GetCustomAttributeFromCtor(EntityHandle ctorHandle)
        {
            switch (ctorHandle.Kind)
            {
                case HandleKind.MemberReference:
                    MemberReference caCtor = _mdReader.GetMemberReference((MemberReferenceHandle)ctorHandle);
                    return GetTypeFromEntityHandle(caCtor.Parent);

                default:
                    throw new Exception($"Unhandled handle kind: '{ctorHandle.Kind}'");
            }
        }

        /// <summary>
        /// Get a type metadata object from an entity handle known to refer to a type.
        /// </summary>
        /// <param name="typeHandle">An entity handle referring to a type.</param>
        /// <returns>A metadata object describing the type referred to by the handle.</returns>
        private TypeMetadata GetTypeFromEntityHandle(EntityHandle typeHandle)
        {
            switch (typeHandle.Kind)
            {
                case HandleKind.TypeDefinition:
                    // TODO: Make this work with nested types?
                    TypeDefinition typeDefinition = _mdReader.GetTypeDefinition((TypeDefinitionHandle)typeHandle);
                    return ReadTypeMetadata(typeDefinition);

                case HandleKind.TypeReference:
                    TypeReference typeReference = _mdReader.GetTypeReference((TypeReferenceHandle)typeHandle);
                    string fullTypeName = GetFullTypeName(typeReference);
                    TryGetCachedType(fullTypeName, out TypeMetadata typeMetadata);
                    return typeMetadata;

                default:
                    throw new Exception($"Unhandled handle kind: '{typeHandle.Kind}'");
            }
        }

        /// <summary>
        /// Read the accessibility level from type attributes flags.
        /// </summary>
        /// <param name="typeAttributes">the attributes of the type to read.</param>
        /// <returns>the accessibility level encoded by the given type attributes</returns>
        private ProtectionLevel ReadProtectionLevel(TypeAttributes typeAttributes)
        {
            switch (typeAttributes & TypeAttributes.VisibilityMask)
            {
                case TypeAttributes.Public:
                case TypeAttributes.NestedPublic:
                    return ProtectionLevel.Public;

                case TypeAttributes.NestedFamORAssem:
                case TypeAttributes.NestedAssembly:
                    return ProtectionLevel.Internal;

                case TypeAttributes.NestedFamANDAssem:
                case TypeAttributes.NestedFamily:
                    return ProtectionLevel.Protected;

                case TypeAttributes.NestedPrivate:
                    return ProtectionLevel.Private;

                // Top level types can't be private or protected, so not public means internal
                case TypeAttributes.NotPublic:
                    return ProtectionLevel.Internal;

                default:
                    throw new Exception(String.Format("Unknown protection level: '{0}'", typeAttributes & TypeAttributes.VisibilityMask));
            }
        }

        /// <summary>
        /// Read the accessibility level encoded by field attributes flags.
        /// </summary>
        /// <param name="fieldAttributes">the field attributes flags to decode.</param>
        /// <returns>the accessibility level of the field decoded.</returns>
        private ProtectionLevel ReadProtectionLevel(FieldAttributes fieldAttributes)
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
                    return ProtectionLevel.Private;

                default:
                    throw new Exception(String.Format("Unknown protection level: '{0}'", fieldAttributes & FieldAttributes.FieldAccessMask));
            }
        }

        private ProtectionLevel ReadProtectionLevel(MethodAttributes methodAttributes)
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
                    return ProtectionLevel.Private;
                
                default:
                    throw new Exception($"Unknown protection level: '{methodAttributes & MethodAttributes.MemberAccessMask}'");
            }
        }

        private ProtectionLevel GetHighestProtectionLevel(ProtectionLevel p1, ProtectionLevel p2)
        {
            return p1 >= p2 ? p1 : p2;
        }

        /// <summary>
        /// Read whether a field is static based on its field attribute flags.
        /// </summary>
        /// <param name="fieldAttributes">the attribute flags on the type to decode.</param>
        /// <returns>true if the field is static, false otherwise.</returns>
        private bool ReadStatic(FieldAttributes fieldAttributes)
        {
            return (int)(fieldAttributes & FieldAttributes.Static) != 0;
        }

        private bool ReadStatic(MethodAttributes methodAttributes)
        {
            return (int)(methodAttributes & MethodAttributes.Static) != 0;
        }

        /// <summary>
        /// Read whether a type is abstract based on its type attribute flags.
        /// </summary>
        /// <param name="typeAttributes">the type attribute flags to decode.</param>
        /// <returns>true if the type is abstract, false otherwise.</returns>
        private bool ReadAbstract(TypeAttributes typeAttributes)
        {
            return (int)(typeAttributes & TypeAttributes.Abstract) != 0;
        }

        /// <summary>
        /// Read whether a type is sealed based on its type attribute flags.
        /// </summary>
        /// <param name="typeAttributes">the attribute flags on the type to decode.</param>
        /// <returns>true if the type is sealed, false otherwise</returns>
        private bool ReadSealed(TypeAttributes typeAttributes)
        {
            return (int)(typeAttributes & TypeAttributes.Sealed) != 0;
        }

        private TypeKind GetBaseTypeKind(TypeMetadata baseType)
        {
            if (baseType == null || baseType == LoadedTypes.ObjectTypeMetadata)
            {
                return TypeKind.Class;
            }

            if (baseType == LoadedTypes.ValueTypeMetadata)
            {
                return TypeKind.Struct;
            }

            if (baseType == LoadedTypes.EnumTypeMetadata)
            {
                return TypeKind.Enum;
            }

            return GetBaseTypeKind(baseType.BaseType);
        }

        /// <summary>
        /// Search the type caches for a given type by full name.
        /// </summary>
        /// <param name="fullTypeName">the full name of the type to search for.</param>
        /// <param name="typeMetadata">the type metadata object found in the cache.</param>
        /// <returns>true if metadata for the type name was found, false otherwise</returns>
        private bool TryGetCachedType(string fullTypeName, out TypeMetadata typeMetadata)
        {
            // Search the parsed cache first because it's likely to be faster
            if (_typeMetadataCache.TryGetValue(fullTypeName, out typeMetadata))
            {
                return true;
            }

            if (LoadedTypes.TryFindByName(fullTypeName, out typeMetadata))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get the full, namespace-qualified name of a type from a type reference.
        /// </summary>
        /// <param name="typeReference">the type reference to get the full name of.</param>
        /// <returns>the full, namespace-qualified name of the type referenced.</returns>
        private string GetFullTypeName(TypeReference typeReference)
        {
            if (typeReference.Name.IsNil)
            {
                return null;
            }

            string typeName = _mdReader.GetString(typeReference.Name);
            string typeNamespace;
            if (typeReference.Namespace.IsNil)
            {
                typeNamespace = String.Empty;
            }
            else
            {
                typeNamespace = _mdReader.GetString(typeReference.Namespace);
            }

            return GetFullTypeName(typeNamespace, typeName);
        }

        /// <summary>
        /// Get the full, namespace-qualified name of the type in a given type definition.
        /// </summary>
        /// <param name="typeDef">the type definition to get the full name of.</param>
        /// <returns>the namespace-qualified name of the type defined.</returns>
        private string GetFullTypeName(TypeDefinition typeDef)
        {
            if (typeDef.Name.IsNil)
            {
                return null;
            }

            string typeName = _mdReader.GetString(typeDef.Name);
            string typeNamespace;
            if (typeDef.Namespace.IsNil)
            {
                typeNamespace = String.Empty;
            }
            else
            {
                typeNamespace = _mdReader.GetString(typeDef.Namespace);
            }

            return GetFullTypeName(typeName, typeNamespace);
        }

        /// <summary>
        /// Get the full, namespace-qualified name of a nested type with a given declaring type.
        /// </summary>
        /// <param name="declaringType">the declaring type of the nested type to get the full name of.</param>
        /// <param name="typeName">the name of the nested type to get the full name of.</param>
        /// <returns>the full, namespace-qualified name of the nested type.</returns>
        private string GetFullTypeName(TypeMetadata declaringType, string typeName)
        {
            return declaringType.FullName + "." + typeName;
        }

        /// <summary>
        /// Get the full, namespace-qualified name of a type from its namespace and name.
        /// </summary>
        /// <param name="typeNamespace">the namespace of the type.</param>
        /// <param name="typeName">the name of the type.</param>
        /// <returns>the full, namespace-qualified name of the type</returns>
        private string GetFullTypeName(string typeNamespace, string typeName)
        {
            if (String.IsNullOrWhiteSpace(typeNamespace))
            {
                return typeName;
            }

            return typeNamespace + "." + typeName;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _peReader.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}