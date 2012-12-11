using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using ErrorGun.Common;
using ErrorGun.Web.Extensions;

namespace ErrorGun.Web.Services
{
    public class EmailService : IEmailService
    {
        private static readonly string _MailgunDomain;
        private static readonly string _MailgunServer;
        private static readonly int _MailgunPort;
        private static readonly string _MailgunLogin;
        private static readonly string _MailgunPassword;
        
        static EmailService()
        {
            _MailgunDomain = ConfigurationManager.AppSettings["MailgunDomain"];
            _MailgunServer = ConfigurationManager.AppSettings["MAILGUN_SMTP_SERVER"];
            _MailgunPort = Int32.Parse(ConfigurationManager.AppSettings["MAILGUN_SMTP_PORT"]);
            _MailgunLogin = ConfigurationManager.AppSettings["MAILGUN_SMTP_LOGIN"];
            _MailgunPassword = ConfigurationManager.AppSettings["MAILGUN_SMTP_PASSWORD"];
        }

        public void SendConfirmationEmails(IEnumerable<ContactEmail> contactEmails)
        {
            const string confirmationSubject = "ErrorGun Email Confirmation";
            string from = "ErrorGun@" + _MailgunDomain;

            foreach (var contactEmail in contactEmails)
            {
                string body = String.Format(
                    "Please click the following link to confirm your email address: {0}?confirmationCode={1}",
                    ConfigurationManager.AppSettings["EmailConfirmationUrl"], contactEmail.ConfirmationCode);

                SendEmail(from, contactEmail.EmailAddress, confirmationSubject, body);
            }
        }

        public void SendErrorReports(App app, ErrorReport errorReport, IEnumerable<string> emailAddresses)
        {
            string from = app.Name.Replace(' ', '_') + "-" + app.Id.Replace("/", String.Empty) + "@" + _MailgunDomain;
            string subject = "ErrorGun Error Report for " + app.Name;
            string body = String.Format(
                "Id: {0}, Category: {1}, Source: {2}, Reported At: {3} UTC, User Email: {4}\r\nMessage: {5}\r\nDetail: {6}",
                errorReport.Id,
                errorReport.Category.PlaceholderOrTrim(),
                errorReport.Source.PlaceholderOrTrim(),
                errorReport.ReportedTimestampUtc,
                errorReport.UserEmail.PlaceholderOrTrim(),
                errorReport.Message.PlaceholderOrTrim(),
                errorReport.Detail.PlaceholderOrTrim());

            foreach (var emailAddress in emailAddresses)
            {
                SendEmail(from, emailAddress, subject, body);
            }
        }

        private void SendEmail(string from, string toEmailAddress, string subject, string body)
        {
            using (var smtpClient = new SmtpClient(_MailgunServer, _MailgunPort))
            {
                smtpClient.EnableSsl = (_MailgunPort == 587);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_MailgunLogin, _MailgunPassword);

                smtpClient.Send(
                    new MailMessage(from, toEmailAddress, subject, body));
            }
        }
    }
}