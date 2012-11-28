using System;

namespace ErrorGun.Common
{
    public class ErrorReport
    {
        public string Id { get; set; }
        public string AppId { get; set; }
        public DateTime ReportedTimestampUtc { get; set; }
        public string Category { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }
        public string UserEmail { get; set; }
    }
}
