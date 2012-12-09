using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ErrorGun.Web.Filters;
using ErrorGun.Web.Models;
using ErrorGun.Web.Services;

namespace ErrorGun.Web.Controllers
{
    [ServiceExceptionFilter]
    public class AppsController : ApiController
    {
        private readonly IAppService _appService;

        public AppsController(IAppService appService)
        {
            _appService = appService;
        }

        // POST api/apps
        public HttpResponseMessage Post(AppModel app)
        {
            var savedApp = _appService.CreateApp(app);

            var response = Request.CreateResponse(HttpStatusCode.Created, savedApp);

            string link = Url.Link(RouteNames.DefaultApi, new { controller = "apps", id = 1 });
            response.Headers.Location = new Uri(link);

            return response;
        }
    }
}
