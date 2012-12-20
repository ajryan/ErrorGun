using System;
using System.Net;
using System.Web.Mvc;

namespace ErrorGun.Web.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MvcThrottleAttribute : ActionFilterAttribute
    {
        public string Name { get; set; }
        public int Milliseconds { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!ThrottleHelper.CheckThrottle(Name, Milliseconds))
            {
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

                filterContext.Result = new ContentResult
                {
                    Content = String.Format("The {0} action may only be performed every {1} seconds.", Name, Milliseconds)
                };
                filterContext.HttpContext.Response.StatusCode = (int) HttpStatusCode.Conflict;
            }
        }
    }
}