using Halcyon.HAL.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;

namespace Halcyon.HAL {
    public static class ControllerExtensions {

        public static IHttpActionResult HAL(this ApiController controller, IEnumerable<HalLink> links, string relativeLinkBase = "~/") {
            return new HalModel()
                .SetRelativePath(relativeLinkBase)
                .AddLinks(links)
                .ToActionResult(controller);
        }

        public static IHttpActionResult HAL<T>(this ApiController controller, HalModel hyperMedia, string relativeLinkBase = "~/") {
            return hyperMedia
                .SetRelativePath(relativeLinkBase)
                .ToActionResult(controller);
        }

        public static IHttpActionResult HAL<T>(this ApiController controller, T model, HalLink link, string relativeLinkBase = "~/") {
            return controller.HAL(model, new HalLink[] { link }, relativeLinkBase);
        }

        public static IHttpActionResult HAL<T>(this ApiController controller, T model, IEnumerable<HalLink> links, string relativeLinkBase = "~/") {
            if(!links.Any()) {
                return new OkNegotiatedContentResult<T>(model, controller);
            }

            return new HalModel(model)
                .SetRelativePath(relativeLinkBase)
                .AddLinks(links)
                .ToActionResult(controller);
        }

        public static IHttpActionResult HAL<T, E>(this ApiController controller, T model, HalLink modelLink, string embeddedName, IEnumerable<E> embeddedModel, HalLink embeddedLink, string relativeLinkBase = "~/") {
            return controller.HAL(model, new HalLink[] { modelLink }, embeddedName, embeddedModel, new HalLink[] { embeddedLink }, relativeLinkBase);
        }

        public static IHttpActionResult HAL<T, E>(this ApiController controller, T model, HalLink modelLink, string embeddedName, IEnumerable<E> embeddedModel, IEnumerable<HalLink> embeddedLinks, string relativeLinkBase = "~/") {
            return controller.HAL(model, new HalLink[] { modelLink }, embeddedName, embeddedModel, embeddedLinks, relativeLinkBase);
        }

        public static IHttpActionResult HAL<T, E>(this ApiController controller, T model, IEnumerable<HalLink> modelLinks, string embeddedName, IEnumerable<E> embeddedModel, IEnumerable<HalLink> embeddedLinks, string relativeLinkBase = "~/")
        {
            return new HalModel(model)
                .SetRelativePath(relativeLinkBase)
                .AddLinks(modelLinks)
                .AddEmbeddedCollection(embeddedName, embeddedModel, embeddedLinks)
                .ToActionResult(controller);
        }
    }
}