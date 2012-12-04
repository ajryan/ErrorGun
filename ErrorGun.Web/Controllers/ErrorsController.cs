using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ErrorGun.Common;
using ErrorGun.Web.Services;

namespace ErrorGun.Web.Controllers
{
    [ServiceExceptionFilter]
    public class ErrorsController : ApiController
    {
        private readonly IErrorService _errorService;

        public ErrorsController(IErrorService errorService)
        {
            _errorService = errorService;
        }

        // POST api/errors
        public HttpResponseMessage Post(
            [FromBody] ErrorReport error,
            [FromUri] string apiKey)
        {
            var savedError = _errorService.ReportError(error, apiKey);

            var response = Request.CreateResponse(HttpStatusCode.Created, savedError);

            string link = Url.Link(RouteNames.DefaultApi, new { controller = "errors", id = 1 });
            response.Headers.Location = new Uri(link);

            return response;
        }
    }
}
