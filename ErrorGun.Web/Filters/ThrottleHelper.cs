using System;
using System.Web;
using System.Web.Caching;

namespace ErrorGun.Web.Filters
{
    public static class ThrottleHelper
    {
        public static bool CheckThrottle(string name, int milliSeconds)
        {
            var key = String.Concat(name, "-", GetClientAddress());
            bool allowExecute = false;

            if (HttpRuntime.Cache[key] == null)
            {
                HttpRuntime.Cache.Add(
                    key,
                    true,
                    null,
                    DateTime.Now.AddMilliseconds(milliSeconds),
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.Low,
                    null);

                allowExecute = true;
            }

            return allowExecute;
        }

        private static string GetClientAddress()
        {
            var request = HttpContext.Current.Request;

            var userHostAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (String.IsNullOrWhiteSpace(userHostAddress))
                userHostAddress = request.ServerVariables["REMOTE_ADDR"];
            if (String.IsNullOrWhiteSpace(userHostAddress))
                userHostAddress = "UNKNOWN";

            return userHostAddress;
        }
    }
}