﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Data.Linq;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using System.Web.Http.OData.Properties;
using System.Xml.Linq;
using Microsoft.Data.Edm;
using Microsoft.Data.OData;

namespace System.Web.Http.OData.Formatter.Serialization
{
    /// <summary>
    /// ODataSerializer for serializing <see cref="IEdmPrimitiveType" />'s.
    /// </summary>
    internal class ODataPrimitiveSerializer : ODataEntrySerializer
    {
        public ODataPrimitiveSerializer(IEdmPrimitiveTypeReference edmPrimitiveType)
            : base(edmPrimitiveType, ODataPayloadKind.Property)
        {
        }

        public override void WriteObject(object graph, ODataMessageWriter messageWriter, ODataSerializerContext writeContext)
        {
            if (messageWriter == null)
            {
                throw Error.ArgumentNull("messageWriter");
            }

            if (writeContext == null)
            {
                throw Error.ArgumentNull("writeContext");
            }

            messageWriter.WriteProperty(CreateProperty(graph, writeContext.RootElementName, writeContext));
        }

        public override ODataProperty CreateProperty(object graph, string elementName, ODataSerializerContext writeContext)
        {
            if (String.IsNullOrWhiteSpace(elementName))
            {
                throw Error.ArgumentNullOrEmpty("elementName");
            }

            ODataValue value = CreatePrimitive(graph);

            // TODO: Bug 467598: validate the type of the object being passed in here with the underlying primitive type. 
            return new ODataProperty() { Name = elementName, Value = value };
        }

        private static ODataValue CreatePrimitive(object value)
        {
            if (value == null)
            {
                return new ODataNullValue();
            }

            object supportedValue = ConvertUnsupportedPrimitives(value);
            ODataPrimitiveValue primitive = new ODataPrimitiveValue(supportedValue);

            // Required to support JSON light full metadata mode.
            if (!CanTypeBeInferredInJson(supportedValue))
            {
                Contract.Assert(supportedValue != null); // Null values can be inferred
                Type valueType = supportedValue.GetType();
                Contract.Assert(valueType != null);
                IEdmPrimitiveType primitiveType = EdmLibHelpers.GetEdmPrimitiveTypeOrNull(valueType);

                if (primitiveType == null)
                {
                    throw new SerializationException(Error.Format(SRResources.UnsupportedPrimitiveType,
                        valueType.FullName));
                }

                string typeName = primitiveType.FullName();
                Contract.Assert(typeName != null);
                primitive.SetAnnotation<SerializationTypeNameAnnotation>(
                    new SerializationTypeNameAnnotation { TypeName = typeName });
            }

            return primitive;
        }

        internal static object ConvertUnsupportedPrimitives(object value)
        {
            if (value != null)
            {
                Type type = value.GetType();

                // Note that type cannot be a nullable type as value is not null and it is boxed.
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Char:
                        return new String((char)value, 1);

                    case TypeCode.UInt16:
                        return (int)(ushort)value;

                    case TypeCode.UInt32:
                        return (long)(uint)value;

                    case TypeCode.UInt64:
                        return checked((long)(ulong)value);

                    default:
                        if (type == typeof(char[]))
                        {
                            return new String(value as char[]);
                        }
                        else if (type == typeof(XElement))
                        {
                            return ((XElement)value).ToString();
                        }
                        else if (type == typeof(Binary))
                        {
                            return ((Binary)value).ToArray();
                        }
                        else if (type.IsEnum)
                        {
                            // Enums are treated as strings
                            return value.ToString();
                        }
                        break;
                }
            }

            return value;
        }

        private static bool CanTypeBeInferredInJson(object value)
        {
            if (value == null)
            {
                return true;
            }

            TypeCode typeCode = Type.GetTypeCode(value.GetType());

            switch (typeCode)
            {
                // The type for a Boolean, Int32 or String can always be inferred in JSON.
                case TypeCode.Boolean:
                case TypeCode.Int32:
                case TypeCode.String:
                    return true;
                // The type for a Double can be inferred in JSON ...
                case TypeCode.Double:
                    double doubleValue = (double)value;
                    // ... except for NaN or Infinity (positive or negative).
                    if (Double.IsNaN(doubleValue) || Double.IsInfinity(doubleValue))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                default:
                    return false;
            }
        }
    }
}
