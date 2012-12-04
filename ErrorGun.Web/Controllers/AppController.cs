using System;
using System.Web.Mvc;
using ErrorGun.Web.Models;
using ErrorGun.Web.Services;

namespace ErrorGun.Web.Controllers
{
    public class AppController : Controller
    {
        private readonly IAppService _appService;

        public AppController(IAppService appService)
        {
            _appService = appService;
        }

        public ActionResult Create()
        {
            return View(new AppModel());
        }

        [HttpPost]
        public ActionResult Create(AppModel model)
        {
            try
            {
                var newAppModel = _appService.CreateApp(model);
                return View("Complete", newAppModel);
            }
            catch (ServiceValidationException ex)
            {
                model.ErrorMessage = ex.Message;
                return View(model);
            }
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
