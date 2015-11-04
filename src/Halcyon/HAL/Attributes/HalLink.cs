using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Tavis.UriTemplates;

namespace Halcyon.HAL.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class HalLink : Attribute
    {
        private static readonly Regex isTemplatedRegex = new Regex(@"{.+}", RegexOptions.Compiled);

        public HalLink(string rel, string href = "", string title = null, string method = null, string type = null, bool hideIfNotRoot = false)
        {
            Rel = rel;
            Href = href;
            Title = title;
            Method = method;
            Type = type;
            HideIfNotRoot = hideIfNotRoot;
        }

        [JsonIgnore]
        public bool HideIfNotRoot { get; private set; }

        [JsonIgnore]
        public string Rel { get; internal set; }

        [JsonProperty("href")]
        public string Href { get; private set; }

        [JsonProperty("templated", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Templated {
            get {
                return !string.IsNullOrEmpty(Href) && isTemplatedRegex.IsMatch(Href) ? (bool?)true : null;
            }
        }

        [JsonProperty("method", NullValueHandling = NullValueHandling.Ignore)]
        public string Method { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("deprecation", NullValueHandling = NullValueHandling.Ignore)]
        public string Deprecation { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("profile", NullValueHandling = NullValueHandling.Ignore)]
        public string Profile { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("hreflang", NullValueHandling = NullValueHandling.Ignore)]
        public string HrefLang { get; set; }
        
        public string[] GetHrefParameters()
        {
            return new UriTemplate(Href, true, false)
                .GetParameterNames()
                .ToArray();
        }

        public HalLink ResolveFor(object Dto, IHalModelConfig config, JsonSerializer serializer, object memberValue = null)
        {
            var clone = MemberwiseClone() as HalLink;
            var dtoType = Dto?.GetType();

            UriTemplate template;
            if (string.IsNullOrEmpty(Href))
            {
                template = new UriTemplate(memberValue as string, true, false);
            }
            else
            {
                template = new UriTemplate(Href, true, false);

                foreach (string param in template.GetParameterNames())
                {
                    object value;
                    if (param == "value")
                        value = memberValue;
                    else
                        value = dtoType?.GetProperty(param)?.GetValue(Dto);

                    if (value != null)
                    {
                        if (value is string == false)
                        {
                            var token = JToken.FromObject(value, serializer);
                            value = token.Type == JTokenType.String ?
                                token.Value<string>() : value;
                        }

                        template.SetParameter(param, value);
                    }
                }
            }

            var linkUri = new Uri(template.Resolve(), UriKind.RelativeOrAbsolute);

            if (!linkUri.IsAbsoluteUri)
            {
                var baseUri = new Uri(config.RequestPathBase);
                string basePath = baseUri.GetLeftPart(UriPartial.Authority) +
                    VirtualPathUtility.ToAbsolute(config.RelativePathBase, baseUri.AbsolutePath);
                linkUri = new Uri(new Uri(basePath), linkUri);
            }

            clone.Href = linkUri.ToString();

            return clone;
        }
    }
}