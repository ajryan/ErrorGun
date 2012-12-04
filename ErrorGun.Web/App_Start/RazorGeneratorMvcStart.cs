using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using RazorGenerator.Mvc;

[assembly: WebActivator.PostApplicationStartMethod(typeof(ErrorGun.Web.App_Start.RazorGeneratorMvcStart), "Start")]

namespace ErrorGun.Web.App_Start
{
    public static class RazorGeneratorMvcStart
    {
        public static void Start()
        {
            ViewEngines.Engines.Clear();

            var engine = new PrecompiledMvcEngine(typeof(RazorGeneratorMvcStart).Assembly)
            {
                UsePhysicalViewsIfNewer = HttpContext.Current.Request.IsLocal
            };

            ViewEngines.Engines.Insert(0, engine);

            // StartPage lookups are done by WebPages. 
            VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);
        }
    }
}
