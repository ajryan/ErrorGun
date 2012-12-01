using System.Web.Mvc;
using NLog;

namespace ErrorGun.Web
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LogExceptionsAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }

    public class LogExceptionsAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null || filterContext.Exception == null)
                return;

            string controller = filterContext.RouteData.Values["controller"].ToString();
            string action = filterContext.RouteData.Values["action"].ToString();

            // for a local request, log to NLog
            if (filterContext.HttpContext.Request.IsLocal)
            {
                LogManager.GetLogger(controller + "." + action).ErrorException(
                    "Exception occured.", filterContext.Exception);
            }
            // remote requests go to New Relic
            else
            {
                // TODO: may already be caught
                NewRelic.Api.Agent.NewRelic.NoticeError(filterContext.Exception);
            }
        }
    }
}