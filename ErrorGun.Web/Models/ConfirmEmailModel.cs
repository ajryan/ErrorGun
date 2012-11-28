using System;

namespace ErrorGun.Web.Models
{
    public class ConfirmEmailModel
    {
        public string EmailAddress { get; set; }
        public bool Confirmed { get; set; }
        public bool AlreadyConfirmed { get; set; }
        public string ErrorMessage { get; set; }
    }
}