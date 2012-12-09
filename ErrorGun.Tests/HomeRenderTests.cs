using System;
using System.Web.Mvc;
using ErrorGun.Web.Controllers;
using ErrorGun.Web.Models;
using ErrorGun.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RazorGenerator.Testing;

namespace ErrorGun.Tests
{
    [TestClass]
    public class HomeRenderTests
    {
        [TestMethod]
        public void HomeRender_Post_Create_Success()
        {
            // Arrange

            var testModel = new AppModel
            {
                Name = "App Name",
                ContactEmails = {"a@a.com", "b@b.com"}
            };

            var mockAppService = new Mock<IAppService>();
            mockAppService
                .Setup(service => service.CreateApp(testModel))
                .Returns(() => new AppModel
                {
                    Id = "apps/1",
                    Name = "App Name",
                    ContactEmails = {"a@a.com", "b@b.com"},
                    ApiKey = "apiKey"
                });

            var appsController = new AppsController(mockAppService.Object);
            WebApiControllerHelper.MakeTestable(appsController, "apps");
            // Act

            var response = appsController.Post(testModel);
            //WebViewPage<AppModel> complete = new ErrorGun.Web.Views.App.Complete();
            //var doc = complete.RenderAsHtml((AppModel) response.Content.ReadAsStringAsync());

            //// Assert

            //var h1 = doc.DocumentNode.SelectSingleNode("//h1");
            //Assert.AreEqual("App Created", h1.InnerHtml.Trim());

            // TODO: assert model properties correctly rendered
        }
        [TestMethod]
        public void HomeRender_Get_ConfirmEmail_Confirmed_NotAlreadyConfirmed()
        {
            // Arrange

            var mockAppService = new Mock<IAppService>();
            mockAppService
                .Setup(service => service.ConfirmEmail("test_confirm_code"))
                .Returns(() => new ConfirmEmailModel
                {
                    EmailAddress = "a@a.com",
                    AlreadyConfirmed = false,
                    Confirmed = true,
                    ErrorMessage = null
                });
            var homeController = new HomeController(mockAppService.Object);

            // Act

            var viewResult = (ViewResult)homeController.ConfirmEmail("test_confirm_code");
            WebViewPage<ConfirmEmailModel> confirmEmail = new ErrorGun.Web.Views.Home.ConfirmEmail();
            var doc = confirmEmail.RenderAsHtml((ConfirmEmailModel)viewResult.ViewData.Model);

            // Assert
            var text = doc.DocumentNode.InnerText;
            Assert.IsTrue(text.Contains("Email Confirmation Succeeded for a@a.com"));
            Assert.IsFalse(text.Contains("already confirmed"));
            Assert.IsFalse(text.Contains("Email Confirmation Failed"));
            Assert.IsFalse(text.Contains("Error"));
        }

        // TODO: fail

        // TODO: already confirmed
    }
}
