using System;
using System.Net;
using System.Web.Mvc;

namespace ErrorGun.Web.Controllers
{
    public class FailController : Controller
    {
        public ActionResult NotFound()
        {
            Response.StatusCode = (int) HttpStatusCode.NotFound;
            return View();
        }

        public ActionResult ServerError()
        {
            Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            return View();
        }

        public ActionResult ThrowError()
        {
            throw new NotImplementedException("This error is expected.");
        }
    }
}
