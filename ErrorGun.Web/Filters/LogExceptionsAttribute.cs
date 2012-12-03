using System.Web.Mvc;
using ErrorGun.Web.Services;

namespace ErrorGun.Web.Filters
{
    public class LogExceptionsAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null || filterContext.Exception == null)
                return;

            LoggingService.LogException("Exception handled in LogExceptionsAttribute", filterContext.Exception);
        }
    }
}