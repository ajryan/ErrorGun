/// <reference path="knockout-2.2.d.ts" />
/// <reference path="jquery-1.8.d.ts" />
/// <reference path="ErrorCodes.ts" />

module ErrorGun {
    export module AppCreate {
        export class ViewModel {
            // TODO: ErrorMessage to array

            // properties
            public Id = ko.observable("");
            public ApiKey = ko.observable("");
            public CreatedTimestampUtc = ko.observable("");
            public Name = ko.observable("");
            public ContactEmails = ko.observableArray([{Address: ko.observable("")}]);
            public ErrorMessage = ko.observable("");

            // methods
            public Create: () => void;
            public AddContactEmail: () => void;
            public RemoveContactEmail: (email) => void;
            public toJSON: () => Object;

            // fields
            private _errorCodes = new ErrorGun.ErrorCodes();
            private _completeUrl: string;

            constructor(completeUrl: string) {
                this._completeUrl = completeUrl;

                this.Create = () => {
                    var $regButton = $('#registerButton');

                    this.ErrorMessage("");
                    $regButton.attr('disabled', true);

                    var json = ko.toJSON(this);
                    $.ajax({
                        url: "/api/apps",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: json
                    })
                    .fail((jqXHR, textStatus) => {
                        // TODO: special 403 handling, generic for other
                        // TODO: class that takes jqXHR.responseText on ctor and
                        //       a ko observable for pushing message 
                        var responseJson = JSON.parse(jqXHR.responseText);
                        var errorMessages = [];
                        responseJson.ErrorCodes.forEach(function (errorCode) {
                            errorMessages.push(ErrorGun.ErrorCodes.MessageMap[errorCode]);
                        });
                        var errorMessage = errorMessages.join("\n");
                        this.ErrorMessage(errorMessage);
                        $regButton.removeAttr('disabled');
                    })
                    .done((ajaxData) => {
                        // TODO: modal "created" with app details
                        this.ApiKey(ajaxData.ApiKey);
                        this.Id(ajaxData.Id);
                    });
                }

                this.AddContactEmail = () => {
                    this.ContactEmails.push({Address: ko.observable("")});
                }

                this.RemoveContactEmail = (email) => {
                    if (this.ContactEmails().length > 1)
                        this.ContactEmails.remove(email);
                }

                this.toJSON = () => {
                    return {
                        Name: this.Name(),
                        ContactEmails: ko.utils.arrayMap(this.ContactEmails(), (ce) => { return ce.Address() })
                    };
                }
            }
        }
    }
}