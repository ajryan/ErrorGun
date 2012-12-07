using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using ErrorGun.Web.App_Start;
using ErrorGun.Web.Controllers;
using ErrorGun.Web.Injection;
using ErrorGun.Web.Services;
using NLog;
using Ninject;
using RazorGenerator.Mvc;

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

            // Register RazorGenerator view engine
            var engine = new PrecompiledMvcEngine(typeof(MvcApplication).Assembly) { UsePhysicalViewsIfNewer = DebugEnvironment };
            ViewEngines.Engines.Insert(0, engine);
            VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);

            AreaRegistration.RegisterAllAreas();
            
            var kernel = new StandardKernel(new ErrorGunWebServicesModule());

            WebApiConfig.Register(GlobalConfiguration.Configuration, kernel);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
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

            // if we have already executed a fail action, nothing to do
            if (Context.Request.Path.StartsWith("/Fail/NotFound", StringComparison.OrdinalIgnoreCase) ||
                Context.Request.Path.StartsWith("/Fail/ServerError", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // for non-local requests, do not serve the built-in
            // error page
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