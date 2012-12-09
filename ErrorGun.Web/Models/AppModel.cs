using System;
using System.Collections.Generic;

namespace ErrorGun.Web.Models
{
    public class AppModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> ContactEmails { get; set; }
        public string ApiKey { get; set; }
        public DateTime CreatedTimestampUtc { get; set; }

        public string ErrorMessage { get; set; }

        public AppModel()
        {
            ContactEmails = new List<string>();
        }

        public override string ToString()
        {
            return String.Format(
                "Id: {0};\r\nApiKey: {1};\r\nName: {2};\r\nContactEmails: {3};\r\nCreatedTimestampUtc: {4}",
                Id,
                ApiKey,
                Name,
                "[" + String.Join(", ", ContactEmails) + "]",
                CreatedTimestampUtc);
        }
    }
}