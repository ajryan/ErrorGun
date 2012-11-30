using System;
using System.Web.Mvc;

namespace ErrorGun.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ForwardAwareRequireHttpsAttribute : RequireHttpsAttribute
	{
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext == null)
				throw new ArgumentNullException("filterContext");

			if (filterContext.HttpContext.Request.IsSecureConnection)
				return;

			var forwardedProtocol = filterContext.HttpContext.Request.Headers["X-Forwarded-Proto"];
            if (String.Equals(forwardedProtocol, "https", StringComparison.InvariantCultureIgnoreCase))
				return;

			base.HandleNonHttpsRequest(filterContext);
		}
	}
}