using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using ErrorGun.Web.Services;

namespace ErrorGun.Web.Filters
{
    public class ServiceExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var serviceException = actionExecutedContext.Exception as ServiceValidationException;
            if (serviceException != null)
            {
                var response = new
                {
                    Message = serviceException.Message,
                    ErrorCodes = serviceException.ErrorCodes.Select(ec => ec.ToString())
                };

                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                    HttpStatusCode.Forbidden, response);

                actionExecutedContext.Response.ReasonPhrase = "Validation failed";
                actionExecutedContext.Response.Headers.Add("X-Status-Reason", "Validation failed");
            }
            else
            {
                LoggingService.LogException("Non-ServiceException in ServiceExceptionFilterAttribute", actionExecutedContext.Exception);
                base.OnException(actionExecutedContext);
            }
        }
    }
}