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
        [WebApiThrottle(Name = "AppsPost", Milliseconds = 30000)]
        public HttpResponseMessage Post(AppModel app)
        {
            var savedApp = _appService.CreateApp(app);

            var response = Request.CreateResponse(HttpStatusCode.Created, savedApp);

            string link = Url.Link(RouteNames.DefaultApi, new { controller = "apps", id = 1 });
            response.Headers.Location = new Uri(link);

            return response;
        }

        // GET api/apps
        [WebApiThrottle(Name = "AppsGet", Milliseconds = 5000)]
        public HttpResponseMessage Get(string apiKey)
        {
            var app = _appService.LoadApp(apiKey);
            var response = Request.CreateResponse(HttpStatusCode.OK, app);
            return response;
        }

        // DELETE api/apps
        [WebApiThrottle(Name = "AppsDelete", Milliseconds = 10000)]
        public HttpResponseMessage Delete(string appId, string apiKey)
        {
            _appService.DeleteApp(appId, apiKey);
            var response = Request.CreateResponse(HttpStatusCode.OK, appId + " has been deleted."); // TODO: correct REST response code?
            return response;
        }
    }
}
