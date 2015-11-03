using Halcyon.HAL.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Halcyon.HAL
{
    public class HalContractResolver : DefaultContractResolver
    {
        private HalModelConverter Converter { get; set; } = new HalModelConverter();

        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (objectType == typeof(HalModel)) {
                return Converter;
            }

            return base.ResolveContractConverter(objectType);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (
                (property.AttributeProvider.GetAttributes(true).Any(a => a is HalIgnoreAttribute || a is HalLink || a is HalEmbeddedValuesAttribute)) ||
                (property.DeclaringType == typeof(Attribute) && property.UnderlyingName == "TypeId")
            )
            {
                property.ShouldSerialize = (instance) => {
                    return false;
                };
            }

            return property;
        }
    }
}
