/// <reference path="defs\jquery-1.8.d.ts" />

module ErrorGun {
    export class ErrorCodes {
        public static MessageMap = {
            "App_MissingApiKey": "Please provide an API Key.",
            "App_AppDoesNotExist": "Ap app with the provided API Key does not exist.",
            "App_MissingAppModel": "Missing application data.",
            "App_MissingName": "Please provide an application name.",
            "App_MissingContactEmail": "Please provide at least one contact email.",
            "App_InvalidEmailFormat": "A contact email was provided in an invalid format.",
            "App_DuplicateContactEmails": "A duplicate contact email was provided.",

            "ErrorReport_MissingErrorReport": "No error report was submitted.",
            "ErrorReport_MissingAppId": "AppId is required.",
            "ErrorReport_AppDoesNotExist": "An App with the provided API Key does not exist.",
            "ErrorReport_InvalidApiKey": "The ApiKey is invalid.",
            "ErrorReport_MissingMessage": "ErrorMessage is required.",
            "ErrorReport_InvalidUserEmail": "UserEmail format is invalid.",

            "ConfirmEmail_EmailDoesNotExist": "There is no app contact email with the provided address."
        };

        public static GetErrorMessages(responseText: string) : string {
            try {
            // attempt to parse out our custom errorcode
                var responseJson = JSON.parse(responseText);
                var errorMessages = [];
                $.each(responseJson.ErrorCodes, (i, errorCode) => {
                    errorMessages.push(ErrorGun.ErrorCodes.MessageMap[errorCode]);
                });
                return errorMessages.join("\n");
            }
            catch (error) {
                return "An unexpected server error occurred.";
            }
        }
    }
}