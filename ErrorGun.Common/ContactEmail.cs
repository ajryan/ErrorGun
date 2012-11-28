namespace ErrorGun.Common
{
    public class ContactEmail
    {
        public string Id { get; set; }
        public string EmailAddress { get; set; }
        public string ConfirmationCode { get; set; }
        public bool Confirmed { get; set; }
    }
}