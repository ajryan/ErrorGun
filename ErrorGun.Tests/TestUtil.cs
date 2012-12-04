using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace ErrorGun.Tests
{
    public static class TestUtil
    {
        public static ControllerContext GetMvcControllerContext(string controller, string action)
        {
            var routeData = new RouteData();
            routeData.Values.Add("controller", controller);
            routeData.Values.Add("action", action);
            var context = new ControllerContext()
            {
                RouteData = routeData
            };
            return context;
        }
    }
}
