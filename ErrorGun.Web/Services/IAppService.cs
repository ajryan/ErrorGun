using System;
using ErrorGun.Web.Models;

namespace ErrorGun.Web.Services
{
    public interface IAppService
    {
        AppModel CreateApp(AppModel model);
        ConfirmEmailModel ConfirmEmail(string confirmationCode);
        AppModel LoadApp(string apiKey);
    }
}