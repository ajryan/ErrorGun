using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Ninject.Parameters;
using Ninject.Syntax;

namespace ErrorGun.Web.Injection
{
    public class NinjectDependencyScope : IDependencyScope
    {
        protected IResolutionRoot ResolutionRoot;

        public NinjectDependencyScope(IResolutionRoot resolutionRoot)
        {
            ResolutionRoot = resolutionRoot;
        }

        public void Dispose()
        {
            //var disposable = ResolutionRoot as IDisposable;
            //if (disposable != null)
            //{
            //    disposable.Dispose();
            //}
            //ResolutionRoot = null;
        }

        public object GetService(Type serviceType)
        {
            var request = ResolutionRoot.CreateRequest(serviceType, null, new Parameter[0], true, true);
            return ResolutionRoot.Resolve(request).SingleOrDefault();
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            var request = ResolutionRoot.CreateRequest(serviceType, null, new Parameter[0], true, true);
            return ResolutionRoot.Resolve(request).ToList(); // force immediate enumeration
        }
    }
}