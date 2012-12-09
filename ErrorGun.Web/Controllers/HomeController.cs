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
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult AppComplete(AppModel model)
        {
            return View(model);
        }

        public ActionResult ConfirmEmail(string confirmationCode)
        {
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
