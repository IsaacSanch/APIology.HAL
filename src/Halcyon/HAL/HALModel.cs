using Halcyon.HAL.Attributes;
using Newtonsoft.Json;
using System.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Halcyon.HAL
{
    [JsonConverter(typeof(JsonHalModelConverter))]
    public class HalModel
    {
        internal object Dto { get; set; }
        internal readonly List<HalLink> Links = new List<HalLink>();
        internal readonly Dictionary<string, IEnumerable<HalModel>> Embeds = new Dictionary<string, IEnumerable<HalModel>>();

        public HalModel(object dto = null, IHalModelConfig config = null)
        {
            Dto = dto;
            _config = config;
        }

        private IHalModelConfig _config;
        public IHalModelConfig Config
        {
            get {
                if (_config == null)
                {
                    _config = new HalModelConfig(
                        Dto.GetType()
                            .GetCustomAttributes(false)
                            .OfType<HalModelAttribute>()
                            .SingleOrDefault()
                    );
                }
                return _config;
            }
        }

        public HalModel SetRelativePath(string uri)
        {
            Config.RelativePathBase = uri;
            return this;
        }

        public HalModel SetRequestPath(string uri)
        {
            Config.RequestPathBase = uri;
            return this;
        }

        public HalModel AddLinks(IEnumerable<HalLink> links)
        {
            Links.AddRange(links);
            return this;
        }

        public HalModel AddLinks(params HalLink[] links)
        {
            Links.AddRange(links);
            return this;
        }

        public HalModel AddEmbeddedModels(string name, IEnumerable<HalModel> objects, Dictionary<string, dynamic> expandMap = null)
        {
            foreach (var model in objects)
                model.Config.ExpandMap = expandMap;

            Embeds.Add(name, objects);
            return this;
        }
        public HalModel AddEmbeddedCollection<T>(string collectionName, IEnumerable<T> model, IEnumerable<HalLink> links = null, Dictionary<string, dynamic> expandMap = null)
        {
            if (links == null) {
                links = Enumerable.Empty<HalLink>();
            }

            var embedded = model
                .Select(m => new HalModel(m).AddLinks(links))
                .ToArray();

            AddEmbeddedModels(collectionName, embedded, expandMap);

            return this;
        }
    }
}