using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Linq;
using MetadataAnalysis.Metadata.ILParse;
using MetadataAnalysis.Metadata.Interface;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MetadataAnalysis.Metadata
{
    public static class LoadedTypes
    {
        private static ConcurrentDictionary<Type, ITypeMetadata> s_typeMetadataCache;

        static LoadedTypes()
        {
            s_typeMetadataCache = new ConcurrentDictionary<Type, ITypeMetadata>();

            Type objectType = typeof(object);
            Type valueType = typeof(ValueType);
            Type enumType = typeof(Enum);

            var objectTypeMetadata = new ILClassMetadata(
                objectType.Name,
                objectType.Namespace,
                ProtectionLevel.Public,
                null,
                GetConstructorMetadata(objectType),
                GetFieldMetadata(objectType),
                GetPropertyMetadata(objectType),
                GetMethodMetadata(objectType),
                GetNestedTypeMetadata(objectType),
                isAbstract: false,
                isSealed: false
            );

            var valueTypeMetadata = new ILClassMetadata(
                valueType.Name,
                valueType.Namespace,
                ProtectionLevel.Public,
                objectTypeMetadata,
                GetConstructorMetadata(valueType),
                GetFieldMetadata(valueType),
                GetPropertyMetadata(valueType),
                GetMethodMetadata(valueType),
                GetNestedTypeMetadata(valueType),
                isAbstract: true,
                isSealed: false
            );

            var enumTypeMetadata = new ILClassMetadata(
                enumType.Name,
                enumType.Namespace,
                ProtectionLevel.Public,
                valueTypeMetadata,
                GetConstructorMetadata(enumType),
                GetFieldMetadata(enumType),
                GetPropertyMetadata(enumType),
                GetMethodMetadata(enumType),
                GetNestedTypeMetadata(enumType),
                isAbstract: true,
                isSealed: false
            );

            ObjectTypeMetadata = objectTypeMetadata;
            ValueTypeMetadata  = valueTypeMetadata;
            EnumTypeMetadata   = enumTypeMetadata;

            s_typeMetadataCache.TryAdd(objectType, objectTypeMetadata);
            s_typeMetadataCache.TryAdd(valueType, valueTypeMetadata);
            s_typeMetadataCache.TryAdd(enumType, enumTypeMetadata);
        }

        public static IClassMetadata ObjectTypeMetadata { get; }

        public static IClassMetadata ValueTypeMetadata { get; }

        public static IClassMetadata EnumTypeMetadata { get; }

        public static ITypeMetadata FromType(Type type)
        {
            ITypeMetadata typeMetadata;
            if (s_typeMetadataCache.TryGetValue(type, out typeMetadata))
            {
                return typeMetadata;
            }

            if (type.IsEnum)
            {
                typeMetadata = new ILEnumMetadata(
                    type.Name,
                    type.Namespace,
                    GetTypeProtectionLevel(type),
                    Enum.GetNames(type).ToImmutableArray()
                );
            }
            else if (type.IsValueType)
            {
                typeMetadata = new ILStructMetadata(
                    type.Name,
                    type.Namespace,
                    GetTypeProtectionLevel(type),
                    GetConstructorMetadata(type),
                    GetFieldMetadata(type),
                    GetPropertyMetadata(type),
                    GetMethodMetadata(type),
                    GetNestedTypeMetadata(type)
                );
            }
            else
            {
                typeMetadata = new ILClassMetadata(
                    type.Name,
                    type.Namespace,
                    GetTypeProtectionLevel(type),
                    FromType(type.BaseType),
                    GetConstructorMetadata(type),
                    GetFieldMetadata(type),
                    GetPropertyMetadata(type),
                    GetMethodMetadata(type),
                    GetNestedTypeMetadata(type),
                    type.IsAbstract,
                    type.IsSealed
                );
            }

            if (!s_typeMetadataCache.TryAdd(type, typeMetadata))
            {
                throw new Exception("Type already cached!");
            }

            return typeMetadata;
        }

        public static bool TryFindByName(string typeName, out ITypeMetadata typeMetadata)
        {
            Type type = Type.GetType(typeName);
            if (type == null)
            {
                typeMetadata = null;
                return false;
            }

            typeMetadata = FromType(type);
            return true;
        }

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

        private static IImmutableList<IConstructorMetadata> GetConstructorMetadata(Type type)
        {
            return null;
        }

        private static IImmutableDictionary<string, IFieldMetadata> GetFieldMetadata(Type type)
        {
            return null;
        }

        private static IImmutableDictionary<string, IPropertyMetadata> GetPropertyMetadata(Type type)
        {
            return null;
        }

        private static IImmutableDictionary<string, IImmutableList<IMethodMetadata>> GetMethodMetadata(Type type)
        {
            return null;
        }

        private static IImmutableDictionary<string, ITypeMetadata> GetNestedTypeMetadata(Type type)
        {
            var nestedTypes = new Dictionary<string, ITypeMetadata>();
            foreach (Type nestedType in type.GetNestedTypes())
            {
                ITypeMetadata nestedTypeMetadata = FromType(nestedType);
                nestedTypes.Add(nestedType.Name, nestedTypeMetadata);
            }
            return nestedTypes.ToImmutableDictionary();
        }
    }
}