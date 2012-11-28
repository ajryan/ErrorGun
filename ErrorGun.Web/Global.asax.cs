using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using ErrorGun.Web.Injection;

namespace ErrorGun.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var ninjectControllerFactory = new NinjectControllerFactory();
            ControllerBuilder.Current.SetControllerFactory(ninjectControllerFactory);
        }
    }
}