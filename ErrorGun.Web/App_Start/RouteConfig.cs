using System;
using System.Web.Mvc;
using System.Web.Routing;

[assembly: WebActivator.PreApplicationStartMethod(typeof(ErrorGun.Web.RouteConfig), "PreRegisterErrorRoutes")]

namespace ErrorGun.Web
{
    public static class RouteConfig
    {
        public static void PreRegisterErrorRoutes()
        {
            RouteTable.Routes.Insert(
                0,
                new Route(
                    "ServerError", 
                    new RouteValueDictionary(new { controller = "Fail", action = "ServerError" }), new MvcRouteHandler()));

            RouteTable.Routes.Insert(
                0,
                new Route(
                    "NotFound",
                    new RouteValueDictionary(new { controller = "Fail", action = "NotFound" }), new MvcRouteHandler()));
        }

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