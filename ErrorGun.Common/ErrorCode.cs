using System;

namespace ErrorGun.Common
{
    // ReSharper disable InconsistentNaming
    public enum ErrorCode
    {
        App_MissingName,
        App_MissingContactEmail,
        App_InvalidEmailFormat,
        App_DuplicateContactEmails,

        ErrorReport_MissingAppId,
        ErrorReport_AppDoesNotExist,
        ErrorReport_MissingMessage,
        ErrorReport_InvalidUserEmail,

        ConfirmEmail_EmailDoesNotExist,
        
    }
    // ReSharper restore InconsistentNaming
}
