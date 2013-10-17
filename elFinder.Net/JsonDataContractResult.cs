using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace System.Web.Mvc
{
    internal class JsonDataContractResult : JsonResult
    {
        public JsonDataContractResult(object data)
        {
            Data = data;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Get is not allowed");
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (!String.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }
            if (Data != null)
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(response.Output, Data);
            }
        }
    }
}