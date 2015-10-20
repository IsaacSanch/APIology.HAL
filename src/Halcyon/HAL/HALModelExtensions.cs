using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Halcyon.HAL
{
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Results;

    public static class HALModelExtensions
    {
        private static void TraverseModel(List<HALModel> stack, HALModel model)
        {
            foreach (var embedded in model.embedded.Values.SelectMany(m => m)) {
                TraverseModel(stack, embedded);
            }
            stack.Add(model);
        }

        public static IHttpActionResult ToResult(this HALModel model, ApiController controller, string baseUri = "~/", HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            if (!String.IsNullOrWhiteSpace(baseUri))
            {
                baseUri = controller.Url.Content(baseUri);
                var models = new List<HALModel>();
                TraverseModel(models, model);
                models.ForEach(m => m.baseUri = baseUri);
            }
            
            return new NegotiatedContentResult<HALModel>(statusCode, model, controller);
        }
    }
}
