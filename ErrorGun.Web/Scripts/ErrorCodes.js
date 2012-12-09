var ErrorGun;
(function (ErrorGun) {
    var ErrorCodes = (function () {
        function ErrorCodes() { }
        ErrorCodes.MessageMap = {
            "App_MissingAppModel": "Missing application data.",
            "App_MissingName": "Please provide an application name.",
            "App_MissingContactEmail": "Please provide at least one contact email.",
            "App_InvalidEmailFormat": "A contact email was provided in an invalid format.",
            "App_DuplicateContactEmails": "A duplicate contact email was provided.",
            "ErrorReport_MissingAppId": "AppId is required.",
            "ErrorReport_AppDoesNotExist": "An App with the provided AppId does not exist.",
            "ErrorReport_InvalidApiKey": "The ApiKey is invalid.",
            "ErrorReport_MissingMessage": "ErrorMessage is required.",
            "ErrorReport_InvalidUserEmail": "UserEmail format is invalid.",
            "ConfirmEmail_EmailDoesNotExist": "There is no app contact email with the provided address."
        };
        return ErrorCodes;
    })();
    ErrorGun.ErrorCodes = ErrorCodes;    
})(ErrorGun || (ErrorGun = {}));
