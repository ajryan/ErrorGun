using System;
using System.Web.Http;
using ErrorGun.Web.Injection;
using Ninject;

namespace ErrorGun.Web.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config, IKernel kernel)
        {
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;

            config.Routes.MapHttpRoute(
                name: RouteNames.DefaultApi,
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.DependencyResolver = new NinjectDependencyResolver(kernel);
        }
    }
}
