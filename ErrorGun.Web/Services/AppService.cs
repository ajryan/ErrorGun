using System;
using System.Collections.Generic;
using System.Linq;
using ErrorGun.Common;
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
                var contactEmails = SplitEmails(appModel.ContactEmails)
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
                    ContactEmails = String.Join(", ", contactEmails.Select(ce => ce.EmailAddress)),
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
                    .SingleOrDefault(ce => ce.ConfirmationCode == confirmationCode);

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

        private static void ThrowOnInvalidAppModel(AppModel appModel)
        {
            var errorCodes = new List<ErrorCode>();

            if (string.IsNullOrWhiteSpace(appModel.Name))
                errorCodes.Add(ErrorCode.App_MissingName);

            if (string.IsNullOrWhiteSpace(appModel.ContactEmails))
            {
                errorCodes.Add(ErrorCode.App_MissingContactEmail);
            }
            else
            {
                var emails = SplitEmails(appModel.ContactEmails);
                if (emails.Any(email => !EmailValidator.Validate(email)))
                    errorCodes.Add(ErrorCode.App_InvalidEmailFormat);
            }

            if (errorCodes.Count > 0)
                throw new ServiceValidationException(errorCodes);
        }

        private static IEnumerable<string> SplitEmails(string emails)
        {
            var tokens = emails.Split(',');
            return tokens.Select(t => t.Trim());
        }
    }
}