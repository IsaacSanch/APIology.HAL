using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Halcyon.HAL
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Results;

    public static class HALModelExtensions
    {
        private static List<HALModel> GetFlattenedModelList(HALModel model, List<HALModel> stack = null)
        {
            if (stack == null)
                stack = new List<HALModel>();

            foreach (var embedded in model.Embeds.Values.SelectMany(m => m)) {
                GetFlattenedModelList(embedded, stack);
            }
            stack.Add(model);
            return stack;
        }

        public static IHttpActionResult ToActionResult(this HALModel model, ApiController controller, string baseUri = "~/", HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            if (!String.IsNullOrWhiteSpace(baseUri))
            {
                baseUri = controller.Url.Content(baseUri);
                var models = GetFlattenedModelList(model);
                models.ForEach(m => m.SetLinkBaseUri(baseUri));
            }
            
            return new NegotiatedContentResult<HALModel>(statusCode, model, controller);
        }

        public static JObject ToJObject(this HALModel model)
        {
            var baseObject = JObject.FromObject(model.Dto);

            foreach (var embedPair in model.Embeds)
            {
                baseObject.Add(embedPair.Key,
                    new JArray(embedPair.Value.Select(ToJObject))
                );
            }

            return baseObject;
        }
    }
}
