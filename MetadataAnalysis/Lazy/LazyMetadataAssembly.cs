using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using MetadataAnalysis.Interface;

namespace MetadataAnalysis.Lazy
{
    public class LazyAssemblyMetadata : IAssemblyMetadata
    {
        private readonly MetadataReader _mdReader;

        private readonly AssemblyDefinition _asmDef;

        private string _name;

        private string _culture;

        private IReadOnlyList<byte> _publicKey;

        private IReadOnlyList<ICustomAttributeMetadata> _customAttributes;

        public LazyAssemblyMetadata(string asmPath, MetadataReader mdReader)
        {
            _mdReader = mdReader;
            Path = asmPath;
            _asmDef = mdReader.GetAssemblyDefinition();
        }

        public string Name
        {
            get
            {
                if (_name == null)
                {
                    _name = _mdReader.GetString(_asmDef.Name);
                }
                return _name;
            }
        }

        public Version Version
        {
            get
            {
                return _asmDef.Version;
            }
        }

        public string Culture
        {
            get
            {
                if (_culture == null)
                {
                    _culture = _mdReader.GetString(_asmDef.Culture);
                }
                return _culture;
            }
        }

        public AssemblyFlags AssemblyFlags
        {
            get
            {
                return _asmDef.Flags;
            }
        }

        public IReadOnlyList<byte> PublicKey
        {
            get
            {
                if (_publicKey == null)
                {
                    _publicKey = _mdReader.GetBlobContent(_asmDef.PublicKey);
                }
                return _publicKey;
            }
        }

        public AssemblyHashAlgorithm HashAlgorithm
        {
            get
            {
                return _asmDef.HashAlgorithm;
            }
        }

        public IReadOnlyList<ICustomAttributeMetadata> CustomAttributes
        {
            get
            {
                if (_customAttributes == null)
                {
                    _customAttributes = new LazyCustomAttributeMetadataList(_asmDef.GetCustomAttributes(), _mdReader);
                }
                return _customAttributes;
            }
        }

        public IReadOnlyDictionary<string, ITypeMetadata> DefinedTypes => throw new NotImplementedException();

        public string Path { get; }

        public IReadOnlyDictionary<string, IAssemblyMetadata> ReferencedAssemblies => throw new NotImplementedException();
    }
}