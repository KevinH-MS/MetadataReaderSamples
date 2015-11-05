using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Decoding;
using System.Reflection.PortableExecutable;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace ReadAssemblyAttributes
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var stream = File.OpenRead(@"C:\Program Files (x86)\Microsoft SDKs\F#\4.0\Framework\v4.0\Fsc.exe"))
            using (var peReader = new PEReader(stream))
            {
                var reader = peReader.GetMetadataReader();
                foreach (var attributeHandle in reader.CustomAttributes)
                {
                    var attr = reader.GetCustomAttribute(attributeHandle);
                    if (attr.Parent.Kind != HandleKind.AssemblyDefinition)
                    {
                        continue;
                    }
                    var ctor = reader.GetMemberReference((MemberReferenceHandle)attr.Constructor);
                    var type = reader.GetTypeReference((TypeReferenceHandle)ctor.Parent);
                    var nsName = reader.GetString(type.Namespace);
                    var typeName = reader.GetString(type.Name);
                    if (nsName == "System.Reflection" && typeName == "AssemblyCompanyAttribute")          
                    {
                        var decoder = CustomAttributeDecoder.DecodeCustomAttribute(attributeHandle, new DummyProvider(reader));
                        Console.WriteLine(nsName + "." + typeName + "(" + decoder.FixedArguments.Single().Value + ")");
                    }
                    else
                    {
                        Console.WriteLine(nsName + "." + typeName + "(?)");
                    }
                }         
            }
        }
    }

    class DummyProvider : ICustomAttributeTypeProvider<object>
    {
        private readonly MetadataReader _reader;

        public DummyProvider(MetadataReader reader)
        {
            _reader = reader;
        }

        MetadataReader ISignatureTypeProvider<object>.Reader
        {
            get
            {
                return _reader;
            }
        }

        object ITypeProvider<object>.GetArrayType(object elementType, ArrayShape shape)
        {
            return null;
        }

        object ITypeProvider<object>.GetByReferenceType(object elementType)
        {
            return null;
        }

        object ISignatureTypeProvider<object>.GetFunctionPointerType(MethodSignature<object> signature)
        {
            return null;
        }

        object ITypeProvider<object>.GetGenericInstance(object genericType, ImmutableArray<object> typeArguments)
        {
            return null;
        }

        object ISignatureTypeProvider<object>.GetGenericMethodParameter(int index)
        {
            return null;
        }

        object ISignatureTypeProvider<object>.GetGenericTypeParameter(int index)
        {
            return null;
        }

        object ISignatureTypeProvider<object>.GetModifiedType(object unmodifiedType, ImmutableArray<CustomModifier<object>> customModifiers)
        {
            return null;
        }

        object ISignatureTypeProvider<object>.GetPinnedType(object elementType)
        {
            return null;
        }

        object ITypeProvider<object>.GetPointerType(object elementType)
        {
            return null;
        }

        object ISignatureTypeProvider<object>.GetPrimitiveType(PrimitiveTypeCode typeCode)
        {
            return null;
        }

        object ICustomAttributeTypeProvider<object>.GetSystemType()
        {
            return null; 
        }

        object ITypeProvider<object>.GetSZArrayType(object elementType)
        {
            return null;
        }

        object ISignatureTypeProvider<object>.GetTypeFromDefinition(TypeDefinitionHandle handle, bool? isValueType)
        {
            return null;
        }

        object ISignatureTypeProvider<object>.GetTypeFromReference(TypeReferenceHandle handle, bool? isValueType)
        {
            return null;
        }

        object ICustomAttributeTypeProvider<object>.GetTypeFromSerializedName(string name)
        {
            return null;
        }

        PrimitiveTypeCode ICustomAttributeTypeProvider<object>.GetUnderlyingEnumType(object type)
        {
            return PrimitiveTypeCode.Int32;
        }

        bool ICustomAttributeTypeProvider<object>.IsSystemType(object type)
        {
            return false;
        }
    }
}
