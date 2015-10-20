using System;
using System.Collections.Generic;
using System.Linq;

namespace Halcyon.HAL
{
    using Newtonsoft.Json;
    using System.Web.Http;

    [JsonConverter(typeof(HALModelConverter))]
    public class HALModel
    {
        internal object dto { get; set; }
        internal readonly List<Link> links = new List<Link>();
        internal readonly Dictionary<string, IEnumerable<HALModel>> embedded = new Dictionary<string, IEnumerable<HALModel>>();
        internal string baseUri;

        public HALModel(object dto)
        {
            this.dto = dto;
        }

        public HALModel AddLinks(IEnumerable<Link> links)
        {
            this.links.AddRange(links);
            return this;
        }

        public HALModel AddLinks(params Link[] links)
        {
            this.links.AddRange(links);
            return this;
        }

        public HALModel AddEmbeddedCollection(string name, IEnumerable<HALModel> objects)
        {
            embedded.Add(name, objects);
            return this;
        }
        public HALModel AddEmbeddedCollection<T>(string collectionName, IEnumerable<T> model, IEnumerable<Link> links = null)
        {
            if (links == null) {
                links = Enumerable.Empty<Link>();
            }

            var embedded = model
                .Select(m => new HALModel(m).AddLinks(links))
                .ToArray();

            this.AddEmbeddedCollection(collectionName, embedded);

            return this;
        }
    }
}