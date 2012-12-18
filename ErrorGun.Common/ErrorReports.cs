using System;

namespace ErrorGun.Common
{
    public class ErrorReports
    {
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public ErrorReport[] Items { get; set; }
    }
}
