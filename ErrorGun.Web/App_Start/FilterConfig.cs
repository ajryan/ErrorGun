using System.Web.Mvc;
using ErrorGun.Web.Filters;

namespace ErrorGun.Web
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LogExceptionsAttribute(), 0);
            filters.Add(new HandleErrorAttribute(), 99);
        }
    }
}