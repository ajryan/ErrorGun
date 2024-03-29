﻿using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ErrorGun.Common;
using ErrorGun.Web.Filters;
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

        // TODO: move api key to header, and implement auth attribute
        // POST api/errors
        [WebApiThrottle(Name = "ErrorsPost", Milliseconds = 1000)]
        public HttpResponseMessage Post(
            [FromBody] ErrorReport error,
            [FromUri] string apiKey)
        {
            var savedError = _errorService.ReportError(error, apiKey);

            var response = Request.CreateResponse(HttpStatusCode.Created, savedError);

            string link = Url.Link(RouteNames.DefaultApi, new {controller = "errors", id = 1});
            response.Headers.Location = new Uri(link);

            return response;
        }

        // GET api/errors
        [WebApiThrottle(Name = "ErrorsGet", Milliseconds = 1000)]
        public ErrorReports Get(string apiKey, int pageIndex = 0, int pageSize = 5)
        {
            var errorReports = _errorService.GetErrorReports(apiKey, pageIndex, pageSize);
            return errorReports;
        }
    }
}
