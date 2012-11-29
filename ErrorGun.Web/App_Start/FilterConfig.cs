using System.Web.Mvc;

namespace ErrorGun.Web
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // Our custom error handling is taking care of this
            //filters.Add(new HandleErrorAttribute());
        }
    }
}