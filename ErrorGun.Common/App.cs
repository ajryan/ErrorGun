using System;
using System.Collections.Generic;

namespace ErrorGun.Common
{
    public class App
    {
        public string Id { get; set; }
        public string ApiKey { get; set; }
        public string Name { get; set; }
        public List<string> ContactEmailIds { get; set; }
        public DateTime CreatedTimestampUtc { get; set; }
    }
}
