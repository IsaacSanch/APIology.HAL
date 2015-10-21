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

        public static IHttpActionResult HAL(this ApiController controller, IEnumerable<Link> links, string relativeLinkBase = "~/") {
            return new HALModel()
                .AddLinks(links)
                .ToActionResult(controller, relativeLinkBase);
        }

        public static IHttpActionResult HAL<T>(this ApiController controller, HALModel hyperMedia, string relativeLinkBase = "~/") {
            return hyperMedia.ToActionResult(controller, relativeLinkBase);
        }

        public static IHttpActionResult HAL<T>(this ApiController controller, T model, Link link, string relativeLinkBase = "~/") {
            return controller.HAL(model, new Link[] { link }, relativeLinkBase);
        }

        public static IHttpActionResult HAL<T>(this ApiController controller, T model, IEnumerable<Link> links, string relativeLinkBase = "~/") {
            if(!links.Any()) {
                return new OkNegotiatedContentResult<T>(model, controller);
            }

            return new HALModel(model)
                .AddLinks(links)
                .ToActionResult(controller, relativeLinkBase);
        }

        public static IHttpActionResult HAL<T, E>(this ApiController controller, T model, Link modelLink, string embeddedName, IEnumerable<E> embeddedModel, Link embeddedLink, string relativeLinkBase = "~/") {
            return controller.HAL(model, new Link[] { modelLink }, embeddedName, embeddedModel, new Link[] { embeddedLink }, relativeLinkBase);
        }

        public static IHttpActionResult HAL<T, E>(this ApiController controller, T model, Link modelLink, string embeddedName, IEnumerable<E> embeddedModel, IEnumerable<Link> embeddedLinks, string relativeLinkBase = "~/") {
            return controller.HAL(model, new Link[] { modelLink }, embeddedName, embeddedModel, embeddedLinks, relativeLinkBase);
        }

        public static IHttpActionResult HAL<T, E>(this ApiController controller, T model, IEnumerable<Link> modelLinks, string embeddedName, IEnumerable<E> embeddedModel, IEnumerable<Link> embeddedLinks, string relativeLinkBase = "~/")
        {
            return new HALModel(model)
                .AddLinks(modelLinks)
                .AddEmbeddedCollection(embeddedName, embeddedModel, embeddedLinks)
                .ToActionResult(controller, relativeLinkBase);
        }
    }
}