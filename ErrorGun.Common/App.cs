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

        public override string ToString()
        {
            return String.Format(
                "Id: {0};\r\nApiKey:{1};\r\nName: {2};\r\nContactEmailIds: {3};\r\nCreatedTimestampUtc: {4}",
                Id,
                ApiKey,
                Name,
                String.Join(", ", ContactEmailIds),
                CreatedTimestampUtc);
        }
    }
}
