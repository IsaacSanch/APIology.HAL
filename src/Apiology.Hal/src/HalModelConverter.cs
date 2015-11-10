using Apiology.Hal.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Apiology.Hal
{
    public class HalModelConverter : JsonConverter
    {
        private class PLPair
        {
            public PropertyInfo property { get; set; }
            public HalLink link { get; set; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HalModel);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var model = value as HalModel;
            if (model == null) return;

            JObject output;
            if (model.Dto != null)
                output = JToken.FromObject(model.Dto, serializer) as JObject;
            else
                output = new JObject();

            var propertiesOfDto = model.Dto?.GetType().GetProperties() ?? new PropertyInfo[] { };

            foreach (var prop in propertiesOfDto)
            {
                var attr = prop.GetCustomAttribute<HalEmbeddedValuesAttribute>();
                if (attr != null)
                {
                    var items = prop.GetValue(model.Dto) as IEnumerable<object>;
                    if (items == null || !items.Any())
                        continue;

                    string resolvedPropertyName = null;
                    var resolver = serializer.ContractResolver as DefaultContractResolver;
                    if (resolver != null) {
                        resolvedPropertyName = resolver.GetResolvedPropertyName(prop.Name);
                    }

                    var itemProps = items.First().GetType().GetProperties();

                    if (itemProps.Length > 0 && model.Config.ExpandMap?.ContainsKey(resolvedPropertyName ?? prop.Name) == true)
                    {
                        model.AddEmbeddedCollection(prop.Name, items, null, model.Config.ExpandMap[resolvedPropertyName ?? prop.Name]);
                    }
                    else
                    {
                        PLPair pair = null;

                        if (attr.LinkTemplate != null)
                        {
                            pair = new PLPair {
                                property = itemProps.SingleOrDefault(p =>
                                    attr.LinkTemplate.GetHrefParameters().Contains(p.Name)
                                ),
                                link = attr.LinkTemplate
                            };
                        }

                        if (pair == null || (pair.property == null && itemProps.Length > 0))
                        {
                            pair = itemProps
                                .Select(p => new PLPair {
                                    property = p,
                                    link = p.GetCustomAttribute<HalLink>()
                                })
                                .Where(p => p.link != null && p.link.Rel == "self")
                                .Select(p => {
                                    if (pair?.link != null) {
                                        p.link = pair.link;
                                    }
                                    return p;
                                })
                                .SingleOrDefault();
                        }

                        if (pair == null)
                            continue;
                        
                        model.AddLinks(
                            items.Select(i =>
                            {
                                var link = pair.link.ResolveFor(i, model.Config, serializer, pair.property?.GetValue(i) ?? i);
                                link.Rel = prop.Name;
                                return link;
                            })
                        );
                    }
                }
            }

            HalModelExtensions
                .GetFlattenedModelList(model)
                .ForEach(m => m.SetRequestPath(model.Config.RequestPathBase));

            var allLinks =
                model.Links.Select(l => l.ResolveFor(model.Dto, model.Config, serializer))
                .Union(
                    propertiesOfDto
                    .SelectMany(prop => {
                        List<HalLink> links = new List<HalLink>();
                        var link = prop.GetCustomAttribute<HalLink>();

                        if (link != null && (model.Config.IsRoot | !link.HideIfNotRoot) && prop.PropertyType.IsArray)
                        {
                            var items = prop.GetValue(model.Dto) as IEnumerable;
                            foreach (var item in items)
                            {
                                links.Add(link.ResolveFor(model.Dto, model.Config, serializer, item));
                            }
                        }
                        else if (link != null && (model.Config.IsRoot | !link.HideIfNotRoot))
                        {
                            links.Add(link.ResolveFor(model.Dto, model.Config, serializer, prop.GetValue(model.Dto)));
                        }

                        return links;
                    })
                    .Where(prop => prop != null)
                )
                .GroupBy(r => r.Rel)
                .ToDictionary(k => k.Key,
                    k => k.Count() == 1 ? k.SingleOrDefault() as object : k.AsEnumerable() as object
                );

            if (allLinks.Count > 0)
            {
                output.Add("_links", JObject.FromObject(allLinks, serializer));
            }

            if (model.Embeds.Count > 0)
            {
                output.Add("_embedded", JToken.FromObject(model.Embeds, serializer));
            }

            output?.WriteTo(writer);
        }
    }
}
