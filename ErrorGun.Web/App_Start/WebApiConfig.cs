using System;
using System.Web.Http;
using ErrorGun.Web.Injection;

namespace ErrorGun.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;

            config.Routes.MapHttpRoute(
                name: RouteNames.DefaultApi,
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.DependencyResolver = new NinjectDependencyResolver();
        }
    }
}
