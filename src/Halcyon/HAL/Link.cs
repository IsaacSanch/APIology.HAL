using Halcyon.Templates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Tavis.UriTemplates;

namespace Halcyon.HAL {
    public class Link {
        public const string RelForSelf = "self";

        private static readonly Regex isTemplatedRegex = new Regex(@"{.+}", RegexOptions.Compiled);

        public Link(string rel, string href, string title = null, string method = null, string type = null) {
            this.Rel = rel;
            this.Href = href;
            this.Title = title;
            this.Method = method;
            this.Type = type;
        }

        [JsonIgnore]
        public string Rel { get; private set; }

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

        public Link ResolveFor(string linkBase, object dto, JsonSerializer serializer)
        {
            var dtoType = dto.GetType();
            var clone = MemberwiseClone() as Link;
            var uri = new UriTemplate(clone.Href);

            foreach (string param in uri.GetParameterNames())
            {
                object value;
                if (!ReferenceEquals(value = dtoType.GetProperty(param)?.GetValue(dto), null))
                {
                    var token = JToken.FromObject(value, serializer);
                    uri.SetParameter(param, token.Value<string>());
                }
            }

            var linkUri = new Uri(uri.Resolve(), UriKind.RelativeOrAbsolute);

            if (!linkUri.IsAbsoluteUri) {
                var baseUri = new Uri(linkBase, UriKind.RelativeOrAbsolute);
                linkUri = new Uri(baseUri, linkUri);
            }

            clone.Href = linkUri.ToString();

            return clone;
        }
    }
}