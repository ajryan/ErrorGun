using System;

namespace ErrorGun.Web.Models
{
    public class AppModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ContactEmails { get; set; }
        public string ApiKey { get; set; }

        public string ErrorMessage { get; set; }
    }
}