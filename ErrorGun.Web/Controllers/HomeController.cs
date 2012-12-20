using System;
using System.Web.Mvc;
using ErrorGun.Web.Filters;
using ErrorGun.Web.Models;
using ErrorGun.Web.Services;

namespace ErrorGun.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAppService _appService;

        public HomeController(IAppService appService)
        {
            _appService = appService;
        }

        public ActionResult Index()
        {
            ViewBag.IsHomePage = true;
            ViewBag.Title = "Welcome";
            return View();
        }

        public ActionResult Register()
        {
            ViewBag.Title = "Register";
            return View();
        }

        public ActionResult ViewApp()
        {
            ViewBag.Title = "View App";
            return View();
        }

        [MvcThrottle(Name = "HomeConfirmEmail", Milliseconds = 5000)]
        public ActionResult ConfirmEmail(string confirmationCode)
        {
            try
            {
                var confirmedEmail = _appService.ConfirmEmail(confirmationCode);
                ViewBag.Title = "Email Confirmed";
                return View(confirmedEmail);
            }
            catch (ServiceValidationException ex)
            {
                ViewBag.Title = "Email Confirmation Failed";
                return View(
                    new ConfirmEmailModel
                    {
                        Confirmed = false,
                        ErrorMessage = ex.Message
                    });
            }
        }
    }
}
