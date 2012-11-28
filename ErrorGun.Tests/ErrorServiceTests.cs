using System;
using System.Collections.Generic;
using ErrorGun.Common;
using ErrorGun.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Raven.Client.Embedded;

namespace ErrorGun.Tests
{
    [TestClass]
    public class ErrorServiceTests
    {
        private static EmbeddableDocumentStore _DocumentStore;
        private static Mock<IEmailService> _MockEmailService;

        private static string _TestAppId;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _DocumentStore = new EmbeddableDocumentStore
            {
                RunInMemory = true
            };
            _DocumentStore.Initialize();

            using (var session = _DocumentStore.OpenSession())
            {
                var contactEmail = new ContactEmail { EmailAddress = "a@b.com", Confirmed = true };
                session.Store(contactEmail);

                var testApp = new App
                {
                    ApiKey = "apikey",
                    ContactEmailIds = new List<string>{ contactEmail.Id },
                    Name = "test app"
                };
                session.Store(testApp);

                session.SaveChanges();
                _TestAppId = testApp.Id;
            }

            _MockEmailService = new Mock<IEmailService>();
        }

        [TestMethod]
        public void ErrorService_EmptyModelValidation()
        {
            var testError = new ErrorReport();
            try
            {
                new ErrorService(_DocumentStore, _MockEmailService.Object).ReportError(testError);
            }
            catch (ServiceValidationException serviceEx)
            {
                Assert.IsTrue(serviceEx.ErrorCodes.Contains(ErrorCode.ErrorReport_MissingAppId));
                Assert.IsTrue(serviceEx.ErrorCodes.Contains(ErrorCode.ErrorReport_AppDoesNotExist));
                Assert.IsTrue(serviceEx.ErrorCodes.Contains(ErrorCode.ErrorReport_MissingMessage));
                Assert.AreEqual(3, serviceEx.ErrorCodes.Count);
            }
        }

        [TestMethod]
        public void ErrorService_InvalidUserEmailValidation()
        {
            var testError = new ErrorReport
            {
                AppId = _TestAppId,
                Message = "test report",
                UserEmail = "bad_email"
            };
            try
            {
                new ErrorService(_DocumentStore, _MockEmailService.Object).ReportError(testError);
            }
            catch (ServiceValidationException serviceEx)
            {
                Assert.IsTrue(serviceEx.ErrorCodes.Contains(ErrorCode.ErrorReport_InvalidUserEmail));
            }
        }

        [TestMethod]
        public void ErrorService_AppDoesNotExistValidation()
        {
            var testError = new ErrorReport
            {
                AppId = "bad_app_id",
                Message = "test report",
                UserEmail = "x@y.com"
            };
            try
            {
                new ErrorService(_DocumentStore, _MockEmailService.Object).ReportError(testError);
            }
            catch (ServiceValidationException serviceEx)
            {
                Assert.IsTrue(serviceEx.ErrorCodes.Contains(ErrorCode.ErrorReport_AppDoesNotExist));
            }
        }

        [TestMethod]
        public void ErrorService_ReportError()
        {
            var error = new ErrorReport
            {
                AppId = _TestAppId,
                Category = "category",
                Detail = "detail",
                Message = "message",
                Source = "source",
                ReportedTimestampUtc = DateTime.UtcNow,
                UserEmail = "user@email.com"
            };

            var service = new ErrorService(_DocumentStore, _MockEmailService.Object);
            var reportedError = service.ReportError(error);

            Assert.IsFalse(String.IsNullOrEmpty(reportedError.Id));
            _MockEmailService.Verify(s => s.SendErrorReports(It.IsAny<App>(), It.IsAny<ErrorReport>(), It.IsAny<IEnumerable<string>>()));
        }
    }
}
