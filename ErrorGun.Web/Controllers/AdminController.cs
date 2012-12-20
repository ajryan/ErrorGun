using System;
using System.Web.Mvc;
using ErrorGun.Web.Filters;
using ErrorGun.Web.Models;
using Raven.Client;

namespace ErrorGun.Web.Controllers
{
    [ForwardAwareRequireHttps]
    public class AdminController : Controller
    {
        private readonly IDocumentStore _documentStore;

        public AdminController(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        [MvcThrottle(Name = "AdminIndex", Milliseconds = 10000)]
        public ActionResult Index(string password, int appPage = 1)
        {
            return View(new AdminModel(password, appPage, _documentStore));
        }
    }
}
