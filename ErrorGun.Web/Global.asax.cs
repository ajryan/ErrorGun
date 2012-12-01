using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using ErrorGun.Web.Controllers;
using ErrorGun.Web.Injection;

namespace ErrorGun.Web
{
    public class MvcApplication : HttpApplication
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

        private static readonly Dictionary<int,string> _StatusFailActionMap = new Dictionary<int, string>
        {
            {404, "NotFound"},{500,"ServerError"}
        };

        protected void Application_EndRequest()
        {
            if (!_StatusFailActionMap.ContainsKey(Context.Response.StatusCode))
                return;

            if (!Context.Request.IsLocal)
                Response.Clear();

            var routeData = new RouteData();
            routeData.Values["controller"] = "Fail";
            routeData.Values["action"] = _StatusFailActionMap[Context.Response.StatusCode];

            IController controller = new FailController();
            controller.Execute(
                new RequestContext(new HttpContextWrapper(Context), routeData));
        }
    }
}