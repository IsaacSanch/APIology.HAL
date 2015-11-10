using Apiology.Hal.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Apiology.Hal
{
    public class HalMediaTypeFormatter : MediaTypeFormatter
    {
        private const string HalJsonType = "application/hal+json";

        private string RequestPathBase { get; set; }

        private string RequestPath { get; set; }

        private string Expands { get; set; }

        private JsonMediaTypeFormatter _baseFormatter = new JsonMediaTypeFormatter();

        public HalMediaTypeFormatter(string[] mediaTypes = null) : base()
        {
            if (mediaTypes == null) mediaTypes = new string[] { HalJsonType };

            foreach (var mediaType in mediaTypes) {
                SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
            }

            _baseFormatter.SerializerSettings = new HalSerializerSettings();
        }

        public HalSerializerSettings SerializerSettings
        {
            get
            {
                return _baseFormatter.SerializerSettings as HalSerializerSettings;
            }
            set
            {
                _baseFormatter.SerializerSettings = value;
            }
        }

        public override bool CanReadType(Type type) {
            return _baseFormatter.CanReadType(type);
        }

        public override bool CanWriteType(Type type) {
            return type == typeof(HalModel) || _baseFormatter.CanWriteType(type);
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger) {
            return _baseFormatter.ReadFromStreamAsync(type, readStream, content, formatterLogger);
        }

        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            var formatter = MemberwiseClone() as HalMediaTypeFormatter;
            formatter.RequestPathBase = request.RequestUri.GetLeftPart(UriPartial.Authority) + request.GetRequestContext()?.VirtualPathRoot;
            formatter.RequestPath = request.RequestUri.AbsolutePath;
            formatter.Expands = request.RequestUri.ParseQueryString()["expand"];
            return formatter;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            var valType = value.GetType();
            if (valType != typeof(HalModel))
            {
                if (valType.IsArray || value is IEnumerable<object>)
                {
                    var data = value as IEnumerable<object>;
                    value = new HalModel(new { Count = data.Count() })
                        .AddLinks(new HalLink("self", RequestPath))
                        .AddEmbeddedCollection("values", data);
                }
                else
                {
                    value = new HalModel(value);
                }
                type = typeof(HalModel);
            }

            var model = value as HalModel;
            model.Config.IsRoot = true;
            model.Config.RequestPathBase = RequestPathBase;
            model.Config.Expands = Expands;

            return _baseFormatter.WriteToStreamAsync(type, model, writeStream, content, transportContext);
        }
    }
}