using System;
using System.Web.Http.Dependencies;

namespace ErrorGun.Web.Injection
{
    public class NinjectDependencyResolver : NinjectDependencyScope, IDependencyResolver
    {
        public NinjectDependencyResolver() : 
            base(ErrorGunWebServicesModule.GlobalKernel)
        {
        }

        public IDependencyScope BeginScope()
        {
            return new NinjectDependencyScope(ErrorGunWebServicesModule.GlobalKernel);//.BeginBlock());
        }
    }
}