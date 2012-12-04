using System;

namespace ErrorGun.Web.Models
{
    public class AppModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ContactEmails { get; set; }
        public string ApiKey { get; set; }
        public DateTime CreatedTimestampUtc { get; set; }

        public string ErrorMessage { get; set; }

        public override string ToString()
        {
            return String.Format(
                "Id: {0};\r\nApiKey:{1};\r\nName: {2};\r\nContactEmails: {3};\r\nCreatedTimestampUtc: {4}",
                Id,
                ApiKey,
                Name,
                ContactEmails,
                CreatedTimestampUtc);
        }
    }
}