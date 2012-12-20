/// <reference path="defs\jquery-1.8.d.ts" />

module ErrorGun {

    if (!console) console = <any>{log: function() {}};

    export class ErrorCodes {
        private static HTTP_FORBIDDEN: number = 403;
        private static HTTP_CONFLICT: number = 409;

        public static MessageMap = {
            "App_MissingApiKey": "Please provide an API Key.",
            "App_AppDoesNotExist": "Ap app matching your request does not exist.",
            "App_MissingAppModel": "Missing application data.",
            "App_MissingName": "Please provide an application name.",
            "App_MissingContactEmail": "Please provide at least one contact email.",
            "App_InvalidEmailFormat": "A contact email was provided in an invalid format.",
            "App_DuplicateContactEmails": "A duplicate contact email was provided.",
            "App_MissingAppId": "Please provide an App ID.",

            "ErrorReport_PageSizeTooLarge": "The selected page size was too large.",
            "ErrorReport_PageOffsetInvalid": "The selected page offset is invalid.",
            "ErrorReport_MissingErrorReport": "No error report was submitted.",
            "ErrorReport_MissingAppId": "AppId is required.",
            "ErrorReport_AppDoesNotExist": "An App with the provided API Key does not exist.",
            "ErrorReport_InvalidApiKey": "The ApiKey is invalid.",
            "ErrorReport_MissingMessage": "ErrorMessage is required.",
            "ErrorReport_InvalidUserEmail": "UserEmail format is invalid.",

            "ConfirmEmail_EmailDoesNotExist": "There is no app contact email with the provided confirmation code    ."
        };

        public static GetErrorMessages(jqXHR: JQueryXHR) : string {
            try {
                var responseJson = JSON.parse(jqXHR.responseText);

                switch (jqXHR.status) {
                    case HTTP_FORBIDDEN: {
                        // attempt to parse out our custom errorcode
                        var errorMessages = [];
                        $.each(responseJson.ErrorCodes, (i, errorCode) => {
                            errorMessages.push(ErrorGun.ErrorCodes.MessageMap[errorCode]);
                        });
                        return errorMessages.join("\n");
                    }
                    case HTTP_CONFLICT: {
                        return responseJson.Message;
                    }
                }
            }
            catch (error) {
                console.log("ErrorCodes cannot parse responseText: " + jqXHR.responseText);
                return "An unexpected server error occurred.";
            }
        }
    }

}