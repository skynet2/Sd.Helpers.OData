using System;
using System.Web.OData.Formatter.Serialization;
using Microsoft.OData;
using Microsoft.OData.Edm;

namespace Sd.Helpers.OData.Serializers
{
    public class CustomODataSerializerProvider : DefaultODataSerializerProvider
    {
        private readonly CustomEnumSerializer _customEnumSerializer;

        public override ODataEdmTypeSerializer GetEdmTypeSerializer(IEdmTypeReference edmType)
        {
            if (edmType.TypeKind() == EdmTypeKind.Enum)
                return _customEnumSerializer;

            return base.GetEdmTypeSerializer(edmType);
        }

        public CustomODataSerializerProvider(IServiceProvider rootContainer) : base(rootContainer)
        {
            _customEnumSerializer = new CustomEnumSerializer(this);
        }
    }

    internal class CustomEnumSerializer : ODataEdmTypeSerializer
    {
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="serializerProvider"></param>
        public CustomEnumSerializer(ODataSerializerProvider serializerProvider)
            : base(ODataPayloadKind.Property, serializerProvider)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="expectedType"></param>
        /// <param name="writeContext"></param>
        /// <returns></returns>
        public override ODataValue CreateODataValue(object graph, IEdmTypeReference expectedType, ODataSerializerContext writeContext)
        {
            try
            {
                var val = Convert.ToInt64(graph);
                return new ODataUntypedValue() { RawValue = val.ToString() };

            }
            catch (Exception)
            {
                return new ODataUntypedValue() { RawValue = graph.ToString() };
            }

        }
    }
}
