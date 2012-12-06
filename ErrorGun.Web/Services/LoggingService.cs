using System;
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

            WriteNLog(logger => logger.ErrorException(message, exception));

            // write to NewRelic in non-Debug environment only
            if (!MvcApplication.DebugEnvironment)
            {
                NewRelic.Api.Agent.NewRelic.NoticeError(exception);
            }
        }

        public static void LogInfo(string message)
        {
            WriteNLog(logger => logger.Info(message));
        }

        private static void WriteNLog(Action<Logger> logAction)
        {
            // write to NLog in Debug environment only
            if (!MvcApplication.DebugEnvironment)
                return;

            string controller = GetRouteValue("controller");
            string action = GetRouteValue("action");

            string loggerName = (controller.HasValue() && action.HasValue())
                                    ? controller + "." + action
                                    : "Generic Logger";

            var logger = LogManager.GetLogger(loggerName);
            logAction(logger);
        }

        private static string GetRouteValue(string key)
        {
            try
            {
                if (HttpContext.Current == null ||
                    HttpContext.Current.Request.RequestContext.RouteData == null ||
                    !HttpContext.Current.Request.RequestContext.RouteData.Values.ContainsKey(key))
                {
                    return null;
                }

                return HttpContext.Current.Request.RequestContext.RouteData.Values[key].ToString();
            }
            catch (HttpException)
            {
                return null;
            }
        }
    }
}