using System;

namespace ErrorGun.Web.Extensions
{
    public static class StringExtensions
    {
        public static string PlaceholderOrTrim(this string value)
        {
            return String.IsNullOrWhiteSpace(value)
                ? "<empty>"
                : value.Trim();
        }

        public static bool HasValue(this string value)
        {
            return !String.IsNullOrEmpty(value);
        }
    }
}