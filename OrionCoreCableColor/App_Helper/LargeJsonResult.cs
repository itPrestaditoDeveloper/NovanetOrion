using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace OrionCoreCableColor.App_Helper
{
    public class LargeJsonResult : JsonResult
    {
        public LargeJsonResult(object data)
        {
            Data = data;
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            MaxJsonLength = int.MaxValue;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;

            response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/json" : ContentType;

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data != null)
            {
                var serializer = new JavaScriptSerializer
                {
                    MaxJsonLength = (int)MaxJsonLength
                };
                response.Write(serializer.Serialize(Data));
            }
        }
    }

}