using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Rollvolet.CRM.API.Middleware.UrlRewrite
{
    public static class RoutePrefixMvcOptions
    {
        public static void UseCentralRoutePrefix(this MvcOptions opts, IRouteTemplateProvider routeAttribute)
        {
            opts.Conventions.Insert(0, new RoutePrefixConvention(routeAttribute));
        }
    }
}