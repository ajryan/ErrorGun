using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
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

            var exception = Server.GetLastError(); // TODO: will this get logged already by apphb?

            return View();
        }

        public ActionResult ThrowError()
        {
            throw new NotImplementedException("This error is expected.");
        }
    }
}
