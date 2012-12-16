using System;
using ErrorGun.Common;

namespace ErrorGun.Web.Services
{
    public interface IErrorService
    {
        ErrorReport ReportError(ErrorReport errorReport, string apiKey);
        ErrorReport[] GetErrorReports(string apiKey, int pageOffset, int pageSize);
    }
}