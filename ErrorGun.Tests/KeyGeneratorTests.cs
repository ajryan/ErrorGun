using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ErrorGun.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ErrorGun.Tests
{
    [TestClass]
    public class KeyGeneratorTests
    {
        [TestMethod]
        public void KeyGenerator_GeneratesOneHundredUniqueKeys_WithValidFormat()
        {
            var hexRegex = new Regex(@"^[0-9a-z\-]*$", RegexOptions.Compiled);
            var set = new HashSet<string>();

            for (int i = 0; i < 100; i++)
            {
                var key = KeyGenerator.Generate();
                Assert.IsTrue(hexRegex.IsMatch(key));
                Assert.IsTrue(set.Add(key));
            }
        }
    }
}
