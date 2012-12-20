using System;

namespace ErrorGun.Common
{
    // Keep in sync with ErrorCodes.ts

    // ReSharper disable InconsistentNaming
    public enum ErrorCode
    {
        App_MissingApiKey,
        App_AppDoesNotExist,
        App_MissingAppModel,
        App_MissingName,
        App_MissingContactEmail,
        App_InvalidEmailFormat,
        App_DuplicateContactEmails,
        App_MissingAppId,

        ErrorReport_PageSizeTooLarge,
        ErrorReport_PageOffsetInvalid,
        ErrorReport_MissingAppId,
        ErrorReport_AppDoesNotExist,
        ErrorReport_InvalidApiKey,
        ErrorReport_MissingMessage,
        ErrorReport_InvalidUserEmail,
        ErrorReport_MissingErrorReport,

        ConfirmEmail_EmailDoesNotExist
    }
    // ReSharper restore InconsistentNaming
}
