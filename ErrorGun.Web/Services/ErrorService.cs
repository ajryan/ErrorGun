using System;
using System.Collections.Generic;
using System.Linq;
using ErrorGun.Common;
using Raven.Client;

namespace ErrorGun.Web.Services
{
    public class ErrorService : IErrorService
    {
        private readonly IDocumentStore _documentStore;
        private readonly IEmailService _emailService;

        public ErrorService(IDocumentStore documentStore, IEmailService emailService)
        {
            _documentStore = documentStore;
            _emailService = emailService;
        }

        public ErrorReport ReportError(ErrorReport errorReport)
        {
            using (var session = _documentStore.OpenSession())
            {
                // load app
                App app = null;
                if (!String.IsNullOrWhiteSpace(errorReport.AppId))
                {
                    app = session
                        .Include<App>(a => a.ContactEmailIds)
                        .Load<App>(errorReport.AppId);
                }

                // validate
                ThrowOnInvalidErrorReport(errorReport, app);

                // store
                var errorReportToStore = new ErrorReport
                {
                    AppId = errorReport.AppId,
                    Category = errorReport.Category,
                    Detail = errorReport.Detail,
                    Message = errorReport.Message,
                    Source = errorReport.Source,
                    ReportedTimestampUtc = GetReportedTimestampUtc(errorReport.ReportedTimestampUtc),
                    UserEmail = errorReport.UserEmail
                };
                session.Store(errorReportToStore);
                session.SaveChanges();

                // email
                var emailsToContact = session
                    .Load<ContactEmail>(app.ContactEmailIds)
                    .Where(ce => ce.Confirmed)
                    .Select(ce => ce.EmailAddress)
                    .ToList();

                if (emailsToContact.Count > 0)
                {
                    _emailService.SendErrorReports(app, errorReport, emailsToContact);
                }
                else
                {
                    LoggingService.LogInfo("No emails to contact for " + app.Id);
                }

                return errorReportToStore;
            }
        }

        private static void ThrowOnInvalidErrorReport(ErrorReport errorReport, App app)
        {
            var errorCodes = new List<ErrorCode>();

            if (String.IsNullOrWhiteSpace(errorReport.AppId))
                errorCodes.Add(ErrorCode.ErrorReport_MissingAppId);

            if (app == null)
                errorCodes.Add(ErrorCode.ErrorReport_AppDoesNotExist);

            if (String.IsNullOrWhiteSpace(errorReport.Message))
                errorCodes.Add(ErrorCode.ErrorReport_MissingMessage);

            if (!String.IsNullOrWhiteSpace(errorReport.UserEmail) &&
                !EmailValidator.Validate(errorReport.UserEmail))
            {
                errorCodes.Add(ErrorCode.ErrorReport_InvalidUserEmail);
            }

            if (errorCodes.Count > 0)
                throw new ServiceValidationException(errorCodes);
        }

        private static DateTime GetReportedTimestampUtc(DateTime reportedTimestampUtc)
        {
            return 
                reportedTimestampUtc == DateTime.MinValue
                    ? DateTime.UtcNow
                    : reportedTimestampUtc.ToUniversalTime();
        }
    }
}