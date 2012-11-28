using System;
using System.Collections.Generic;
using System.Linq;
using ErrorGun.Common;
using ErrorGun.Web.Models;
using ErrorGun.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Raven.Client;
using Raven.Client.Embedded;

namespace ErrorGun.Tests
{
    [TestClass]
    public class AppServiceTests
    {
        private static EmbeddableDocumentStore _DocumentStore;
        private static Mock<IEmailService> _MockEmailService;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _DocumentStore = new EmbeddableDocumentStore
            {
                RunInMemory = true
            };
            _DocumentStore.Initialize();

            _MockEmailService = new Mock<IEmailService>();
        }

        [TestMethod, ExpectedException(typeof(ServiceValidationException))]
        public void AppService_EmptyModelValidation()
        {
            var badApp = new AppModel();

            try
            {
                var service = new AppService(_DocumentStore, _MockEmailService.Object);
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
                var service = new AppService(_DocumentStore, _MockEmailService.Object);
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
                var service = new AppService(_DocumentStore, _MockEmailService.Object);
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
                var service = new AppService(_DocumentStore, _MockEmailService.Object);
                service.ConfirmEmail("invalid_confirmation_code");
            }
            catch (ServiceValidationException serviceEx)
            {
                Assert.IsTrue(serviceEx.ErrorCodes.Contains(ErrorCode.ConfirmEmail_EmailDoesNotExist));
                throw;
            }
        }

        [TestMethod]
        public void AppService_CompleteRoundTripWithConfirmation()
        {
            var appService = new AppService(_DocumentStore, _MockEmailService.Object);

            // Save via service
            var validApp = new AppModel { Name = "MyApp", ContactEmails = "ok@good.com, alsoOk@email.com" };
            var savedApp = appService.CreateApp(validApp);
            string savedAppId = savedApp.Id;

            // Verify app and contact emails are saved
            using (IDocumentSession session = _DocumentStore.OpenSession())
            {
                App loadedApp = session
                    .Include<App>(a => a.ContactEmailIds)
                    .Load<App>(savedAppId);

                Assert.IsNotNull(loadedApp);
                Assert.AreEqual("MyApp", loadedApp.Name);
                Assert.IsNotNull(loadedApp.ApiKey);
                Assert.AreNotEqual(default(DateTime), loadedApp.CreatedTimestampUtc);

                var loadedEmails = session.Load<ContactEmail>(loadedApp.ContactEmailIds);

                Assert.AreEqual(2, loadedEmails.Length);
                Assert.IsTrue(loadedEmails.Any(ce => ce.EmailAddress == "ok@good.com"));
                Assert.IsTrue(loadedEmails.Any(ce => ce.EmailAddress == "alsoOk@email.com"));
                Assert.IsFalse(loadedEmails.Any(ce => ce.Confirmed));
                Assert.IsFalse(loadedEmails.Any(ce => String.IsNullOrEmpty(ce.ConfirmationCode)));

                // Verify confirmation emails were sent
                _MockEmailService.Verify(s => s.SendConfirmationEmails(It.IsAny<IEnumerable<ContactEmail>>()));

                // Confirm the contact emails: should not be already confirmed
                foreach (var email in loadedEmails)
                {
                    var confirmModel = appService.ConfirmEmail(email.ConfirmationCode);
                    Assert.IsTrue(confirmModel.Confirmed);
                    Assert.IsFalse(confirmModel.AlreadyConfirmed);
                }

                // Re-confirm the contact emails: should be already confirmed
                foreach (var email in loadedEmails)
                {
                    var confirmModel = appService.ConfirmEmail(email.ConfirmationCode);
                    Assert.IsTrue(confirmModel.Confirmed);
                    Assert.IsTrue(confirmModel.AlreadyConfirmed);
                }
            }
        }
    }
}
