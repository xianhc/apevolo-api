using ApeVolo.Common.DI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApeVolo.Common.WebApp;

namespace ApeVolo.Common.Extention
{
    /// <summary>
    /// HTTP上下文扩展
    /// </summary>
    public static partial class ExtObject
    {
        public static string MapPath(this HttpContext httpContext, string virtualPath)
        {
            UrlHelper urlHelper = new UrlHelper(AutofacHelper.GetScopeService<IActionContextAccessor>().ActionContext);
            virtualPath = urlHelper.Content(virtualPath);

            return
                $"{Path.Combine(new List<string> {HttpContextCore.WebRootPath}.Concat(virtualPath.Split('/')).ToArray())}";
        }
    }
}