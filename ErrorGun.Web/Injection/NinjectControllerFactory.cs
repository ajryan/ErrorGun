using System;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;

namespace ErrorGun.Web.Injection
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
                return null;

            return (IController) ErrorGunWebServicesModule.GlobalKernel.Get(controllerType);
        }
    }
}