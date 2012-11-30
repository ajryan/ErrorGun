using System;
using System.Collections.Generic;
using System.Linq;
using ErrorGun.Common;
using ErrorGun.Web.Models;
using ErrorGun.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ErrorGun.Tests
{
    [TestClass]
    public class AppServiceTests : RavenDbTestBase
    {
        public TestContext TestContext { get; set; }

        private static readonly Mock<IEmailService> _MockEmailService = new Mock<IEmailService>();

        [TestMethod, ExpectedException(typeof(ServiceValidationException))]
        public void AppService_EmptyModelValidation()
        {
            var badApp = new AppModel();

            try
            {
                var service = new AppService(DocumentStore, _MockEmailService.Object);
                service.CreateApp(badApp);
            }
            catch (ServiceValidationException serviceEx)
            {
                Assert.IsTrue(serviceEx.ErrorCodes.Contains(ErrorCode.App_MissingName));
                Assert.IsTrue(serviceEx.ErrorCodes.Contains(ErrorCode.App_MissingContactEmail));
                throw;
            }
        }

        [TestMethod, ExpectedException(typeof(ServiceValidationException))]
        public void AppService_SingleBadEmailValidation()
        {
            var badApp = new AppModel
            {
                Name = "MyApp", 
                ContactEmails = "not_an_email_address"
            };

            try
            {
                var service = new AppService(DocumentStore, _MockEmailService.Object);
                service.CreateApp(badApp);
            }
            catch (ServiceValidationException serviceEx)
            {
                Assert.IsTrue(serviceEx.ErrorCodes.Contains(ErrorCode.App_InvalidEmailFormat));
                throw;
            }
        }

        [TestMethod, ExpectedException(typeof(ServiceValidationException))]
        public void AppService_OneGoodOneBadEmailValidation()
        {
            var badApp = new AppModel
            {
                Name = "MyApp",
                ContactEmails = "ok@good.com, not_an_email_address"
            };

            try
            {
                var service = new AppService(DocumentStore, _MockEmailService.Object);
                service.CreateApp(badApp);
            }
            catch (ServiceValidationException serviceEx)
            {
                Assert.IsTrue(serviceEx.ErrorCodes.Contains(ErrorCode.App_InvalidEmailFormat));
                throw;
            }
        }

        [TestMethod, ExpectedException(typeof(ServiceValidationException))]
        public void AppService_ConfirmEmailBadCodeValidation()
        {
            try
            {
                var service = new AppService(DocumentStore, _MockEmailService.Object);
                service.ConfirmEmail("invalid_confirmation_code");
            }
            catch (ServiceValidationException serviceEx)
            {
                Assert.IsTrue(serviceEx.ErrorCodes.Contains(ErrorCode.ConfirmEmail_EmailDoesNotExist));
                throw;
            }
        }

        [TestMethod]
        public void AppService_ZZCompleteRoundTripWithConfirmation()
        {
            var appService = new AppService(DocumentStore, _MockEmailService.Object);

            // Save via service
            var validApp = new AppModel { Name = "MyApp", ContactEmails = "ok@good.com, alsoOk@email.com" };
            var savedApp = appService.CreateApp(validApp);
            string savedAppId = savedApp.Id;

            // Verify app and contact emails are saved
            List<string> emailConfirmCodes;

            using (var session = DocumentStore.OpenSession())
            {
                App loadedApp = session
                    .Include<App>(a => a.ContactEmailIds)
                    .Load<App>(savedAppId);

                Assert.IsNotNull(loadedApp);
                Assert.AreEqual("MyApp", loadedApp.Name);
                Assert.IsNotNull(loadedApp.ApiKey);
                Assert.AreNotEqual(default(DateTime), loadedApp.CreatedTimestampUtc);

                var loadedEmails = session.Load<ContactEmail>(loadedApp.ContactEmailIds);
                foreach (var email in loadedEmails)
                {
                    TestContext.WriteLine("Loaded email with confirm code " + email.ConfirmationCode);
                }

                Assert.AreEqual(2, loadedEmails.Length);
                Assert.IsTrue(loadedEmails.Any(ce => ce.EmailAddress == "ok@good.com"));
                Assert.IsTrue(loadedEmails.Any(ce => ce.EmailAddress == "alsoOk@email.com"));
                Assert.IsFalse(loadedEmails.Any(ce => ce.Confirmed));
                Assert.IsFalse(loadedEmails.Any(ce => String.IsNullOrEmpty(ce.ConfirmationCode)));

                // Verify confirmation emails were sent
                _MockEmailService.Verify(s => s.SendConfirmationEmails(It.IsAny<IEnumerable<ContactEmail>>()));

                emailConfirmCodes = loadedEmails.Select(e => e.ConfirmationCode).ToList();
            }

            // Confirm the contact emails: should not be already confirmed
            foreach (var confirmCode in emailConfirmCodes)
            {
                TestContext.WriteLine("Confirming email with code " + confirmCode);

                var confirmModel = appService.ConfirmEmail(confirmCode);
                Assert.IsTrue(confirmModel.Confirmed);
                Assert.IsFalse(confirmModel.AlreadyConfirmed);
            }

            // Re-confirm the contact emails: should be already confirmed
            foreach (var confirmCode in emailConfirmCodes)
            {
                var confirmModel = appService.ConfirmEmail(confirmCode);
                Assert.IsTrue(confirmModel.Confirmed);
                Assert.IsTrue(confirmModel.AlreadyConfirmed);
            }
        }
    }
}
