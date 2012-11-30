using System;
using System.Linq;
using ErrorGun.Common;
using Raven.Client.Indexes;

namespace ErrorGun.Web.Raven
{
    public class ContactEmailsByConfirmationCode : AbstractIndexCreationTask<ContactEmail>
    {
        public ContactEmailsByConfirmationCode()
        {
            Map = contactEmails => 
                from email in contactEmails
                select new { ConfirmationCode = email.ConfirmationCode };
        }
    }
}