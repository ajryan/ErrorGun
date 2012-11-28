using System;
using System.Collections.Generic;
using ErrorGun.Common;

namespace ErrorGun.Web.Services
{
    public class ServiceValidationException : Exception
    {
        public List<ErrorCode> ErrorCodes { get; private set; }

        public ServiceValidationException(List<ErrorCode> errorCodes)
            : base(
                "A service validation exception occurred with error codes: " +
                String.Join(", ", errorCodes))
        {
            ErrorCodes = errorCodes;
        }

        public ServiceValidationException(IEnumerable<ErrorCode> errorCodes)
            : this(new List<ErrorCode>(errorCodes))
        {
        }

        public ServiceValidationException(ErrorCode errorCode)
            : this(new List<ErrorCode>{ errorCode })
        {
        }
    }
}