using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using ErrorGun.Web.Controllers;
using ErrorGun.Web.Injection;
using ErrorGun.Web.Services;
using NLog;
using Ninject;

namespace ErrorGun.Web
{
    public class MvcApplication : HttpApplication
    {
        private static readonly Lazy<bool> _DebugEnvironment = new Lazy<bool>(
            () =>
            {
                string environment = ConfigurationManager.AppSettings["Environment"];
                return String.Equals(environment, "Debug", StringComparison.OrdinalIgnoreCase);
            });

        public static bool DebugEnvironment
        {
            get { return _DebugEnvironment.Value; }
        }

        protected void Application_Start()
        {
            if (!DebugEnvironment)
            {
                LogManager.DisableLogging();
            }

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var kernel = new StandardKernel(new ErrorGunWebServicesModule());

            WebApiConfig.Register(GlobalConfiguration.Configuration, kernel);
            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory(kernel));
        }

        private static readonly Dictionary<int,string> _StatusFailActionMap = new Dictionary<int, string>
        {
            {404, "NotFound"},
            {500, "ServerError"}
        };

        protected void Application_EndRequest()
        {
            // if we don't have special handling for the current status code,
            // nothing to do
            if (!_StatusFailActionMap.ContainsKey(Context.Response.StatusCode))
                return;

            // for non-local requests, do not serve the built-in
            // error page
            if (!Context.Request.IsLocal)
                Response.Clear();

            // if there is an exception, log it.
            var lastException = Server.GetLastError();
            LoggingService.LogException("Displaying custom error for Application_EndRequest", lastException);

            var routeData = new RouteData();
            routeData.Values["controller"] = "Fail";
            routeData.Values["action"] = _StatusFailActionMap[Context.Response.StatusCode];

            IController controller = new FailController();
            controller.Execute(
                new RequestContext(new HttpContextWrapper(Context), routeData));
        }
    }
}