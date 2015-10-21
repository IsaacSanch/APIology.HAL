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
            if (model.Dto != null)
                output = JToken.FromObject(model.Dto) as JObject;
            else
                output = new JObject();

            if (model.Links.Count > 0)
            {
                var allLinks = model.Links
                    .Select(l => l.ResolveFor(model.Config.LinkBase, model.Dto, serializer))
                    .GroupBy(r => r.Rel)
                    .ToDictionary(k => k.Key,
                        k => k.Count() == 1 ? k.SingleOrDefault() as object : k.AsEnumerable() as object
                    );

                output.Add("_links", JObject.FromObject(allLinks, serializer));
            }

            if (model.Embeds.Count > 0)
            {
                output.Add("_embedded", JToken.FromObject(model.Embeds, serializer));
            }

            output.WriteTo(writer);
        }
    }
}
