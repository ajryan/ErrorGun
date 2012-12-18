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

        public ErrorReport ReportError(ErrorReport errorReport, string apiKey)
        {
            using (var session = _documentStore.OpenSession())
            {
                // load app
                App app = null;
                if (errorReport != null && !String.IsNullOrWhiteSpace(errorReport.AppId))
                {
                    app = session
                        .Include<App>(a => a.ContactEmailIds)
                        .Load<App>(errorReport.AppId);
                }

                // validate
                ThrowOnInvalidErrorReport(errorReport, apiKey, app);

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
                    _emailService.SendErrorReports(app, errorReportToStore, emailsToContact);
                }
                else
                {
                    LoggingService.LogInfo("No emails to contact for " + app.Id);
                }

                return errorReportToStore;
            }
        }

        private const int MAX_PAGE_SIZE = 50;

        public ErrorReports GetErrorReports(string apiKey, int pageOffset, int pageSize)
        {
            if (pageSize > MAX_PAGE_SIZE)
                throw new ServiceValidationException(ErrorCode.ErrorReport_PageSizeTooLarge);

            if (pageOffset < 0)
                throw new ServiceValidationException(ErrorCode.ErrorReport_PageOffsetInvalid);

            if (apiKey == null)
                throw new ServiceValidationException(ErrorCode.ErrorReport_InvalidApiKey);

            using (var session = _documentStore.OpenSession())
            {
                var appId = session
                    .Query<App>()
                    .Where(a => a.ApiKey == apiKey)
                    .Select(a => a.Id)
                    .SingleOrDefault();

                if (appId == null)
                    throw new ServiceValidationException(ErrorCode.ErrorReport_InvalidApiKey);

                RavenQueryStatistics stats;

                var errors = session
                    .Query<ErrorReport>()
                    .Statistics(out stats)
                    .Where(er => er.AppId == appId)
                    .Skip(pageOffset * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(er => er.ReportedTimestampUtc)
                    .ToArray();

                int pageCount = stats.TotalResults / pageSize;
                if (stats.TotalResults % pageSize > 0)
                    pageCount++;

                return new ErrorReports
                {
                    PageSize = pageSize,
                    PageCount = pageCount,
                    Items = errors
                };
            }
        }

        private static void ThrowOnInvalidErrorReport(ErrorReport errorReport, string apiKey, App app)
        {
            var errorCodes = new List<ErrorCode>();

            if (errorReport == null)
            {
                errorCodes.Add(ErrorCode.ErrorReport_MissingErrorReport);
            }
            else
            {
                if (String.IsNullOrWhiteSpace(errorReport.AppId))
                    errorCodes.Add(ErrorCode.ErrorReport_MissingAppId);

                if (app == null)
                {
                    errorCodes.Add(ErrorCode.ErrorReport_AppDoesNotExist);
                }
                else
                {
                    if (!String.Equals(app.ApiKey, apiKey, StringComparison.Ordinal))
                        errorCodes.Add(ErrorCode.ErrorReport_InvalidApiKey);
                }

                if (String.IsNullOrWhiteSpace(errorReport.Message))
                    errorCodes.Add(ErrorCode.ErrorReport_MissingMessage);

                if (!String.IsNullOrWhiteSpace(errorReport.UserEmail) &&
                    !EmailValidator.Validate(errorReport.UserEmail))
                {
                    errorCodes.Add(ErrorCode.ErrorReport_InvalidUserEmail);
                }
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