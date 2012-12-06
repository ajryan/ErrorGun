using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
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
                IDictionary<string, Type> _mappings = 
                    (from type in assembly.GetTypes()
                         where typeof(WebPageRenderingBase).IsAssignableFrom(type)
                         let pageVirtualPath = type.GetCustomAttributes(inherit: false).OfType<PageVirtualPathAttribute>().FirstOrDefault()
                         where pageVirtualPath != null
                         select new KeyValuePair<string, Type>(CombineVirtualPaths(_baseVirtualPath, pageVirtualPath.VirtualPath), type)
                         ).ToDictionary(t => t.Key, t => t.Value, StringComparer.OrdinalIgnoreCase);
                new LogEvent("Available views count " + _mappings.Count).Raise();
                         
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
