using System;
using System.Configuration;
using System.Web;
using ErrorGun.Web.Extensions;
using NLog;

namespace ErrorGun.Web.Services
{
    public static class LoggingService
    {
        private const string EXCEPTION_LOGGED = "LOGGINGSERVICE_EXCEPTION_LOGGED";

        public static void LogException(string message, Exception exception)
        {
            if (HttpContext.Current.Items.Contains(EXCEPTION_LOGGED))
                return;
            HttpContext.Current.Items[EXCEPTION_LOGGED] = true;

            // write to NLog in Debug environment only
            if (MvcApplication.DebugEnvironment)
            {
                string controller = GetRouteValue("controller");
                string action = GetRouteValue("action");

                string logger = (controller.HasValue() && action.HasValue())
                    ? controller + "." + action
                    : "Generic Error";

                LogManager.GetLogger(logger).ErrorException(message, exception);
            }
            // non-debug Environment goes to NewRelic
            else
            {
                NewRelic.Api.Agent.NewRelic.NoticeError(exception);
            }
        }

        private static string GetRouteValue(string key)
        {
            if (HttpContext.Current == null ||
                HttpContext.Current.Request.RequestContext.RouteData == null ||
                !HttpContext.Current.Request.RequestContext.RouteData.Values.ContainsKey(key))
            {
                return null;
            }

            return HttpContext.Current.Request.RequestContext.RouteData.Values[key].ToString();
        }
    }
}