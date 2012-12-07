using System;
using System.Web.Mvc;
using System.Web.WebPages;
using RazorGenerator.Mvc;

namespace ErrorGun.Web.App_Start
{
    public static class ViewEngineConfig
    {
        public static void RegisterViewEngines(ViewEngineCollection viewEngines)
        {
            var engine = new PrecompiledMvcEngine(typeof(ViewEngineConfig).Assembly)
            {
                UsePhysicalViewsIfNewer = MvcApplication.DebugEnvironment
            };

            viewEngines.Insert(0, engine);
            VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);
        }
    }
}