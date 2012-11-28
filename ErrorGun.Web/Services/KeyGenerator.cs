using System;

namespace ErrorGun.Web.Services
{
    public static class KeyGenerator
    {
        public static string Generate()
        {
            // TODO: really random
            return Guid.NewGuid().ToString();
        }
    }
}