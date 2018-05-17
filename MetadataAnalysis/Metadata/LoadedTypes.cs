using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MetadataAnalysis.Metadata
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

        /// <summary>
        /// Initialize the loaded type cache with common primitive types.
        /// </summary>
        static LoadedTypes()
        {
            // Initialize the cache
            s_typeMetadataCache = new ConcurrentDictionary<Type, TypeMetadata>();

            // Process the common base types
            Type objectType = typeof(object);
            Type valueType = typeof(ValueType);
            Type enumType = typeof(Enum);

            var objectTypeMetadata = new ClassMetadata(
                objectType.Name,
                objectType.Namespace,
                ProtectionLevel.Public,
                null,
                null,
                GetConstructorMetadata(objectType),
                GetFieldMetadata(objectType),
                GetPropertyMetadata(objectType),
                GetMethodMetadata(objectType),
                isAbstract: false,
                isSealed: false
            )
            {
                NestedTypes = GetNestedTypeMetadata(objectType)
            };

            var valueTypeMetadata = new ClassMetadata(
                valueType.Name,
                valueType.Namespace,
                ProtectionLevel.Public,
                objectTypeMetadata,
                null,
                GetConstructorMetadata(valueType),
                GetFieldMetadata(valueType),
                GetPropertyMetadata(valueType),
                GetMethodMetadata(valueType),
                isAbstract: true,
                isSealed: false
            )
            {
                NestedTypes = GetNestedTypeMetadata(valueType)
            };

            var enumTypeMetadata = new ClassMetadata(
                enumType.Name,
                enumType.Namespace,
                ProtectionLevel.Public,
                valueTypeMetadata,
                null,
                GetConstructorMetadata(enumType),
                GetFieldMetadata(enumType),
                GetPropertyMetadata(enumType),
                GetMethodMetadata(enumType),
                isAbstract: true,
                isSealed: false
            )
            {
                NestedTypes = GetNestedTypeMetadata(enumType)
            };

            ObjectTypeMetadata = objectTypeMetadata;
            ValueTypeMetadata  = valueTypeMetadata;
            EnumTypeMetadata   = enumTypeMetadata;

            s_typeMetadataCache.TryAdd(objectType, objectTypeMetadata);
            s_typeMetadataCache.TryAdd(valueType, valueTypeMetadata);
            s_typeMetadataCache.TryAdd(enumType, enumTypeMetadata);
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
            TypeMetadata declaringType = type.BaseType == null ? null : FromType(type.BaseType);

            // We differentiate based on what kind of type
            if (type.IsEnum)
            {
                typeMetadata = new EnumMetadata(
                    type.Name,
                    type.Namespace,
                    GetTypeProtectionLevel(type),
                    declaringType,
                    Enum.GetNames(type).ToImmutableArray()
                );
            }
            else if (type.IsValueType)
            {
                typeMetadata = new StructMetadata(
                    type.Name,
                    type.Namespace,
                    GetTypeProtectionLevel(type),
                    declaringType,
                    GetConstructorMetadata(type),
                    GetFieldMetadata(type),
                    GetPropertyMetadata(type),
                    GetMethodMetadata(type)
                )
                {
                    NestedTypes = GetNestedTypeMetadata(type)
                };
            }
            else
            {
                typeMetadata = new ClassMetadata(
                    type.Name,
                    type.Namespace,
                    GetTypeProtectionLevel(type),
                    type.BaseType == null ? null : FromType(type.BaseType),
                    declaringType,
                    GetConstructorMetadata(type),
                    GetFieldMetadata(type),
                    GetPropertyMetadata(type),
                    GetMethodMetadata(type),
                    type.IsAbstract,
                    type.IsSealed
                )
                {
                    NestedTypes = GetNestedTypeMetadata(type)
                };
            }

            if (!s_typeMetadataCache.TryAdd(type, typeMetadata))
            {
                // This shouldn't happen, but throw an exception in case it does
                throw new Exception("Type already cached!");
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

        /// <summary>
        /// Get the metadata for the constructors of a type.
        /// </summary>
        /// <param name="type">the type to get the constructors of.</param>
        /// <returns>a list of the constructors of the given type.</returns>
        private static IImmutableList<ConstructorMetadata> GetConstructorMetadata(Type type)
        {
            return null;
        }

        /// <summary>
        /// Get the metadata for the fields of a type.
        /// </summary>
        /// <param name="type">the type to get the fields of.</param>
        /// <returns>a dictionary of fields on the type, keyed by name.</returns>
        private static IImmutableDictionary<string, FieldMetadata> GetFieldMetadata(Type type)
        {
            return null;
        }

        /// <summary>
        /// Get the metadata for the properties of a type.
        /// </summary>
        /// <param name="type">the type to get the properties of.</param>
        /// <returns>a dictionary of properties on the type, keyed by name.</returns>
        private static IImmutableDictionary<string, PropertyMetadata> GetPropertyMetadata(Type type)
        {
            return null;
        }

        /// <summary>
        /// Get the metadata for the methods of a type.
        /// </summary>
        /// <param name="type">the type to get the methods of.</param>
        /// <returns>a dictionary of methods on the type, keyed by name.</returns>
        private static IImmutableDictionary<string, IImmutableList<MethodMetadata>> GetMethodMetadata(Type type)
        {
            return null;
        }

        /// <summary>
        /// Get the metadata for the types defined within the type.
        /// </summary>
        /// <param name="type">the type to get the nested types of.</param>
        /// <returns>a dictionary of nested types in the given type, keyed by name.</returns>
        private static IImmutableDictionary<string, TypeMetadata> GetNestedTypeMetadata(Type type)
        {
            var nestedTypes = new Dictionary<string, TypeMetadata>();
            foreach (Type nestedType in type.GetNestedTypes())
            {
                TypeMetadata nestedTypeMetadata = FromType(nestedType);
                nestedTypes.Add(nestedType.Name, nestedTypeMetadata);
            }
            return nestedTypes.ToImmutableDictionary();
        }
    }
}