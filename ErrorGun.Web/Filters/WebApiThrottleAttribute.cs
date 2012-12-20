using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ErrorGun.Web.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class WebApiThrottleAttribute: ActionFilterAttribute
    {
        public string Name { get; set; }
        public int Milliseconds { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!ThrottleHelper.CheckThrottle(Name, Milliseconds))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Conflict, 
                    String.Format("The {0} action may only be performed every {1} seconds.", Name, Milliseconds));
            }
        }
    }
}