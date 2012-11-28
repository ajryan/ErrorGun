using System;
using System.Text.RegularExpressions;

namespace ErrorGun.Web.Services
{
    public static class EmailValidator
    {
        private static readonly Regex _EmailRegex = new Regex(
            @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
            RegexOptions.Compiled);

        public static bool Validate(string emailAddress)
        {
            return 
                !String.IsNullOrWhiteSpace(emailAddress) && 
                _EmailRegex.IsMatch(emailAddress);
        }
    }
}