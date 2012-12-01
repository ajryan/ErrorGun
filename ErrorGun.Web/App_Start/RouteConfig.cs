using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace ErrorGun.Web
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            routes.MapRoute(
                name: RouteNames.DefaultAdmin,
                url: "Admin/{action}",
                defaults: new { controller = "Admin", action = "Index" });

            routes.MapRoute(
                name: RouteNames.Default,
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "App", action = "Create", id = UrlParameter.Optional }
            );
        }
    }
}