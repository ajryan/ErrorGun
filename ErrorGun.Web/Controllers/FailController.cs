using System;
using System.Net;
using System.Web.Mvc;
using ErrorGun.Web.Services;

namespace ErrorGun.Web.Controllers
{
    public class FailController : Controller
    {
        public ActionResult NotFound()
        {
            const int notFound = (int) HttpStatusCode.NotFound;

            LogCurrentServerError(notFound);
            Response.StatusCode = notFound;

            return View();
        }

        public ActionResult ServerError()
        {
            const int serverError = (int) HttpStatusCode.InternalServerError;

            LogCurrentServerError(serverError);
            Response.StatusCode = serverError;

            return View();
        }

        public ActionResult ThrowError()
        {
            throw new NotImplementedException("This error is expected.");
        }

        private void LogCurrentServerError(int statusCode)
        {
            string message = String.Format(
                "FailController invoked due to error {0} at path {1}", statusCode, Request.RawUrl);

            // if there is an exception, log it.
            var lastException = Server.GetLastError();
            if (lastException != null)
            {
                LoggingService.LogException(message, lastException);
            }
            else
            {
                LoggingService.LogInfo(message);
            }
        }
    }
}
