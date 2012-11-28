using System;
using System.Collections.Generic;
using System.Configuration;
using ErrorGun.Common;
using ErrorGun.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ErrorGun.Tests
{
    [TestClass]
    public class EmailServiceTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void EmailService_SendConfirmation()
        {
            string environment = ConfigurationManager.AppSettings["Environment"].ToUpper();
            if (environment == "TEST") // set by appharbour
            {
                Assert.Inconclusive("Not running SMTP-dependent test on AppHarbour");
                return;
            }

            TestContext.WriteLine("This test requires a local SMTP server (like http://papercut.codeplex.com) listening on port 25");

            var service = new EmailService();
            service.SendConfirmationEmails(new List<ContactEmail>
            {
                new ContactEmail { ConfirmationCode = "confirm_code", EmailAddress = "a@b.com" }
            });

            TestContext.WriteLine("Email successfully sent.");
        }

        [TestMethod]
        public void EmailService_SendErrorReports()
        {
            string environment = ConfigurationManager.AppSettings["Environment"].ToUpper();
            if (environment == "TEST") // set by appharbour
            {
                Assert.Inconclusive("Not running SMTP-dependent test on AppHarbour");
                return;
            }

            TestContext.WriteLine("This test requires a local SMTP server (like http://papercut.codeplex.com) listening on port 25");

            var service = new EmailService();
            service.SendErrorReports(
                new App { Id = "appId", Name = "appName" },
                new ErrorReport { Message = "message" },
                new List<string>{ "x@y.com" });

            TestContext.WriteLine("Email successfully sent.");
        }
    }
}
