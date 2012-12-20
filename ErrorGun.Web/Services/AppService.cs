using System;
using System.Collections.Generic;
using System.Linq;
using ErrorGun.Common;
using ErrorGun.Web.Extensions;
using ErrorGun.Web.Models;
using Raven.Client;

namespace ErrorGun.Web.Services
{
    public class AppService : IAppService
    {
        private readonly IDocumentStore _documentStore;
        private readonly IEmailService _emailService;

        public AppService(IDocumentStore documentStore, IEmailService emailService)
        {
            _documentStore = documentStore;
            _emailService = emailService;
        }

        public AppModel CreateApp(AppModel appModel)
        {
            ThrowOnInvalidAppModel(appModel);

            using (var session = _documentStore.OpenSession())
            {
                var contactEmails = TrimmedEmails(appModel.ContactEmails)
                    .Select(
                        address => new ContactEmail
                        {
                            EmailAddress = address,
                            ConfirmationCode = KeyGenerator.Generate(),
                            Confirmed = false
                        })
                    .ToList();
                foreach (var contactEmail in contactEmails)
                    session.Store(contactEmail);

                var newApp = new App
                {
                    Name = appModel.Name.Trim(),
                    ApiKey = KeyGenerator.Generate(),
                    CreatedTimestampUtc = DateTime.UtcNow,
                    ContactEmailIds = contactEmails.Select(ce => ce.Id).ToList()
                };
                session.Store(newApp);

                session.SaveChanges();

                // send confirmation emails
                _emailService.SendConfirmationEmails(contactEmails);

                // output new model
                return new AppModel
                {
                    Id = newApp.Id,
                    Name = newApp.Name,
                    ContactEmails = contactEmails.Select(ce => ce.EmailAddress).ToList(),
                    ApiKey = newApp.ApiKey
                };
            }
        }

        public ConfirmEmailModel ConfirmEmail(string confirmationCode)
        {
            using (var session = _documentStore.OpenSession())
            {
                var contactEmail = session
                    .Query<ContactEmail>()
                    .Where(ce => ce.ConfirmationCode == confirmationCode)
                    .SingleOrDefault();

                if (contactEmail == null)
                    throw new ServiceValidationException(ErrorCode.ConfirmEmail_EmailDoesNotExist);

                bool alreadyConfirmed = contactEmail.Confirmed;
                if (!alreadyConfirmed)
                {
                    contactEmail.Confirmed = true;
                    session.Store(contactEmail);
                    session.SaveChanges();
                }
                
                return new ConfirmEmailModel
                {
                    Confirmed = true,
                    AlreadyConfirmed = alreadyConfirmed,
                    EmailAddress = contactEmail.EmailAddress
                };
            }
        }

        public AppModel LoadApp(string apiKey)
        {
            if (String.IsNullOrWhiteSpace(apiKey))
                throw new ServiceValidationException(ErrorCode.App_MissingApiKey);

            using (var session = _documentStore.OpenSession())
            {
                var loadedApp = session
                    .Query<App>()
                    .Include<App>(a => a.ContactEmailIds)
                    .Where(app => app.ApiKey == apiKey)
                    .SingleOrDefault();

                if (loadedApp == null)
                    throw new ServiceValidationException(ErrorCode.ErrorReport_AppDoesNotExist);

                var emails = session
                    .Load<ContactEmail>(loadedApp.ContactEmailIds)
                    .Select(ce => ce.EmailAddress)
                    .ToList();

                return new AppModel
                {
                    Id = loadedApp.Id,
                    Name = loadedApp.Name,
                    ApiKey = loadedApp.ApiKey,
                    CreatedTimestampUtc = loadedApp.CreatedTimestampUtc,
                    ContactEmails = emails
                };
            }
        }

        public void DeleteApp(string appId, string apiKey)
        {
            if (String.IsNullOrWhiteSpace(appId))
                throw new ServiceValidationException(ErrorCode.App_MissingAppId);

            if (String.IsNullOrWhiteSpace(apiKey))
                throw new ServiceValidationException(ErrorCode.App_MissingApiKey);

            using (var session = _documentStore.OpenSession())
            {
                var app = session.Load<App>(appId);
                if (app == null || app.ApiKey != apiKey)
                    throw new ServiceValidationException(ErrorCode.App_AppDoesNotExist);

                session.Delete(app);
                session.SaveChanges();
            }
        }


        private static void ThrowOnInvalidAppModel(AppModel appModel)
        {
            var errorCodes = new List<ErrorCode>();

            if (appModel == null)
            {
                errorCodes.Add(ErrorCode.App_MissingAppModel);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(appModel.Name))
                    errorCodes.Add(ErrorCode.App_MissingName);

                var emails = TrimmedEmails(appModel.ContactEmails);
                if (emails.Count == 0)
                {
                    errorCodes.Add(ErrorCode.App_MissingContactEmail);
                }
                else
                {
                    if (emails.Any(email => !EmailValidator.Validate(email)))
                        errorCodes.Add(ErrorCode.App_InvalidEmailFormat);

                    if (emails.HasDuplicate())
                        errorCodes.Add(ErrorCode.App_DuplicateContactEmails);
                }
            }
            if (errorCodes.Count > 0)
                throw new ServiceValidationException(errorCodes);
        }

        private static List<string> TrimmedEmails(List<string> emails)
        {
            return emails
                .Select(e => e.Trim())
                .Where(e => !String.IsNullOrEmpty(e)).ToList();
        }
    }
}