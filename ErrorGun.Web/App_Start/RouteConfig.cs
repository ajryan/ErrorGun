using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace ErrorGun.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: RouteNames.Default,
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "App", action = "Create", id = UrlParameter.Optional }
            );
        }
    }
}