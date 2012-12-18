using System;
using System.Linq;
using ErrorGun.Common;
using Raven.Client.Indexes;

namespace ErrorGun.Web.Raven
{
    public class ErrorReportsByAppId : AbstractIndexCreationTask<ErrorReport>
    {
        public ErrorReportsByAppId()
        {
            Map = errorReports => 
                from errorReport in errorReports
                select new { AppId = errorReport.AppId };
        }
    }
}