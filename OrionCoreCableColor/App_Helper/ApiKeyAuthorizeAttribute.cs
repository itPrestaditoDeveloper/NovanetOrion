using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.App_Helper
{
    public class ApiKeyAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthenticated = base.AuthorizeCore(httpContext);
            var apiKey = httpContext.Request.Headers["ApiKey"];
            var configuredApiKey = MemoryLoadManager.ApiKey;

            var isInternalRequest = httpContext.Request.UserHostAddress == "127.0.0.1"; // Dirección IP local

            if (isInternalRequest || (!string.IsNullOrEmpty(apiKey) && apiKey == configuredApiKey) || isAuthenticated)
            {
                return true;
            }

            return false;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!AuthorizeCore(filterContext.HttpContext))
            {
                filterContext.Result = new HttpStatusCodeResult(403, "Unauthorized access - Invalid API Key");
            }
        }
    }
}