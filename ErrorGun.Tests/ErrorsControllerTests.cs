﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
        public async Task ErrorsController_Post_Roundtrip()
        {
            var error = new ErrorReport
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
                    s => s.ReportError(It.IsAny<ErrorReport>()))
                .Returns(error);

            var controller = new ErrorsController(mockErrorService.Object);
            SetupTestableApiController(controller);

            var response = controller.Post(error);
            var responseError = await response.Content.ReadAsAsync<ErrorReport>();

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual("apps/1", responseError.AppId);
        }

        private static void SetupTestableApiController(ApiController apiController)
        {
            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/errors");

            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "errors" } });

            apiController.ControllerContext = new HttpControllerContext(config, routeData, request);
            apiController.Request = request;
            apiController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
            apiController.Request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;
        }
    }
}
