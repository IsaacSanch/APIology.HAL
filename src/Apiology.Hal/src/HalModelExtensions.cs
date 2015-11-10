using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apiology.Hal
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Results;

    public static class HalModelExtensions
    {
        internal static List<HalModel> GetFlattenedModelList(HalModel model, List<HalModel> stack = null)
        {
            if (stack == null)
                stack = new List<HalModel>();

            foreach (var embedded in model.Embeds.Values.SelectMany(m => m)) {
                GetFlattenedModelList(embedded, stack);
            }
            stack.Add(model);
            return stack;
        }

        public static IHttpActionResult ToActionResult(this HalModel model, ApiController controller, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var baseUri = controller.Url.Content("~/");
            var models = GetFlattenedModelList(model);
            models.ForEach(m => m.SetRequestPath(baseUri));

            return new NegotiatedContentResult<HalModel>(statusCode, model, controller);
        }

        public static JObject ToJObject(this HalModel model, JsonSerializer serializer)
        {
            JObject output;
            if (model.Dto != null)
                output = JObject.FromObject(model.Dto, serializer);
            else
                output = new JObject();

            Func<HalModel, JObject> ToJObject = (HalModel m) => m.ToJObject(serializer);

            foreach (var embedPair in model.Embeds)
            {
                output.Add(embedPair.Key,
                    new JArray(embedPair.Value.Select(ToJObject))
                );
            }

            return output;
        }
    }
}
