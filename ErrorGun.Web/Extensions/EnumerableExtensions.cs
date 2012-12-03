using System;
using System.Collections.Generic;

namespace ErrorGun.Web.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool HasDuplicate<T>(this IEnumerable<T> values)
        {
            var set = new HashSet<T>();
            foreach (T item in values)
            {
                if (!set.Add(item))
                    return true;
            }

            return false;
        }
    }
}