using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;

namespace ErrorGun.Tests
{
    public static class WebApiControllerHelper
    {
        public static void MakeTestable(ApiController apiController, string controllerName)
        {
            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/" + controllerName);

            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", controllerName } });

            apiController.ControllerContext = new HttpControllerContext(config, routeData, request);
            apiController.Request = request;
            apiController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
            apiController.Request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;
        }
    }
}
