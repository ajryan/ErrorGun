using System;
using System.Web;
using System.Web.Management;
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
            new LogEvent("RazorGeneratorMvcStart Begin").Raise();

            try
            {
                var engine = new PrecompiledMvcEngine(typeof (RazorGeneratorMvcStart).Assembly)
                {
                    UsePhysicalViewsIfNewer = HttpContext.Current.Request.IsLocal
                };

                ViewEngines.Engines.Insert(0, engine);

                // StartPage lookups are done by WebPages. 
                VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);
            }
            catch (Exception ex)
            {
                new LogEvent(ex).Raise();
                throw;
            }

            new LogEvent("RazorGeneratorMvcStart End").Raise();
        }

        private class LogEvent : WebRequestErrorEvent
        {
            public LogEvent(string message)
                : this(new Exception(message))
            {
            }

            public LogEvent(Exception exception)
                : base(null, null, 100001, exception)
            {
            }
        }
    }
}
