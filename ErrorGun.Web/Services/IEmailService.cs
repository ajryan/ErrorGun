using System.Collections.Generic;
using ErrorGun.Common;

namespace ErrorGun.Web.Services
{
    public interface IEmailService
    {
        void SendConfirmationEmails(IEnumerable<ContactEmail> contactEmails);
        void SendErrorReports(App app, ErrorReport errorReport, IEnumerable<string> emailAddresses);
    }
}