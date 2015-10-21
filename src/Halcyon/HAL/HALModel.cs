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
        internal object Dto { get; set; }
        internal readonly List<Link> Links = new List<Link>();
        internal readonly Dictionary<string, IEnumerable<HALModel>> Embeds = new Dictionary<string, IEnumerable<HALModel>>();
        private readonly IHALModelConfig config;

        public HALModel() : this(null)
        {
        }

        public HALModel(IHALModelConfig config)
        {
            this.config = config ?? new HALModelConfig();
        }

        public HALModel(object dto, IHALModelConfig config = null) : this(config)
        {
            Dto = dto;
        }

        public IHALModelConfig Config
        {
            get { return config; }
        }

        public HALModel SetLinkBaseUri(string uri)
        {
            config.LinkBase = uri;
            return this;
        }

        public HALModel AddLinks(IEnumerable<Link> links)
        {
            this.Links.AddRange(links);
            return this;
        }

        public HALModel AddLinks(params Link[] links)
        {
            this.Links.AddRange(links);
            return this;
        }

        public HALModel AddEmbeddedCollection(string name, IEnumerable<HALModel> objects)
        {
            Embeds.Add(name, objects);
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