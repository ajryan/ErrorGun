﻿using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using ErrorGun.Common;
using ErrorGun.Web.Controllers;
using ErrorGun.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ErrorGun.Tests
{
    [TestClass]
    public class ErrorsControllerTests
    {
        [TestMethod]
        public void ErrorsController_Post_Roundtrip()
        {
            var errorReport = new ErrorReport
            {
                AppId = "apps/1",
                Category = "category",
                Detail = "detail",
                Message = "message",
                Source = "source",
                ReportedTimestampUtc = DateTime.UtcNow,
                UserEmail = "user@domain.com"
            };

            var mockErrorService = new Mock<IErrorService>();
            mockErrorService
                .Setup(
                    s => s.ReportError(It.IsAny<ErrorReport>(), "api-key"))
                .Returns(errorReport);

            var controller = new ErrorsController(mockErrorService.Object);
            WebApiControllerHelper.MakeTestable(controller, "errors");

            var response = controller.Post(errorReport, "api-key");
            var responseErrorTask = response.Content.ReadAsAsync<ErrorReport>();
            responseErrorTask.Wait();
            var responseError = responseErrorTask.Result;

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual("apps/1", responseError.AppId);
        }
    }
}
