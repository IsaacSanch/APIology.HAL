using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Halcyon.HAL
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class HALModelConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var model = value as HALModel;
            if (model == null) return;

            JObject output;
            if (model.dto != null)
                output = JToken.FromObject(model.dto) as JObject;
            else
                output = new JObject();

            if (model.links.Count > 0)
            {
                var allLinks = model.links
                    .Select(l => l.ResolveFor(model.baseUri, model.dto, serializer))
                    .GroupBy(r => r.Rel)
                    .ToDictionary(k => k.Key,
                        k => k.Count() == 1 ? k.SingleOrDefault() as object : k.AsEnumerable() as object
                    );

                output.Add("_links", JObject.FromObject(allLinks, serializer));
            }

            if (model.embedded.Count > 0)
            {
                output.Add("_embedded", JToken.FromObject(model.embedded, serializer));
            }

            output.WriteTo(writer);
        }
    }
}
