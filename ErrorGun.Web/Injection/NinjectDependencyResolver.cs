using System;
using System.Web.Http.Dependencies;
using Ninject;

namespace ErrorGun.Web.Injection
{
    public class NinjectDependencyResolver : NinjectDependencyScope, IDependencyResolver
    {
        public NinjectDependencyResolver(IKernel kernel) : 
            base(kernel)
        {
        }

        public IDependencyScope BeginScope()
        {
            return new NinjectDependencyScope(ResolutionRoot);
        }
    }
}