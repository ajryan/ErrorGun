using System;
using System.Web.Mvc;
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
            ViewBag.Title = "Welcome";
            return View();
        }

        public ActionResult Register()
        {
            ViewBag.Title = "Register";
            return View();
        }

        public ActionResult Edit()
        {
            ViewBag.Title = "Edit App";
            return View();
        }

        public ActionResult AppComplete(AppModel model)
        {
            ViewBag.Title = "App Created";
            return View(model);
        }

        public ActionResult ConfirmEmail(string confirmationCode)
        {
            ViewBag.Title = "Confirm Email";

            try
            {
                var confirmedEmail = _appService.ConfirmEmail(confirmationCode);
                return View(confirmedEmail);
            }
            catch (ServiceValidationException ex)
            {
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
