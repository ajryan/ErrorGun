using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;

namespace ErrorGun.Web.Injection
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            IController controller = null;
            if (controllerType != null)
                controller = (IController) ErrorGunWebServicesModule.GlobalKernel.Get(controllerType);

            if (controller == null)
            {
                string controllerTypeName = controllerType == null
                    ? "<null controller type>"
                    : controllerType.ToString();

                throw new HttpException(
                    (int) HttpStatusCode.NotFound,
                    String.Format("A controller for type {0} was not found.", controllerTypeName));
            }

            return controller;
        }
    }
}