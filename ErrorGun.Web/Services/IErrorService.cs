using System;
using ErrorGun.Common;

namespace ErrorGun.Web.Services
{
    public interface IErrorService
    {
        ErrorReport ReportError(ErrorReport errorReport);
    }
}