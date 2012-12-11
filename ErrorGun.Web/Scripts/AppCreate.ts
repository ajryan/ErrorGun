/// <reference path="knockout-2.2.d.ts" />
/// <reference path="jquery-1.8.d.ts" />
/// <reference path="ErrorCodes.ts" />
"use strict";

module ErrorGun {
    export module AppCreate {
        export class ViewModel {
            // TODO: ErrorMessage to array

            // properties
            public Id = ko.observable("");
            public ApiKey = ko.observable("");
            public CreatedTimestampUtc = ko.observable("");
            public Name = ko.observable("");
            public ContactEmails = ko.observableArray([{Address: ko.observable(""), Focused: false}]);
            public ErrorMessage = ko.observable("");

            // methods
            public Create: () => void;
            public AddContactEmail: () => void;
            public RemoveContactEmail: (email) => void;
            public toJSON: () => Object;

            // fields
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
                        var errorMessage = "";
                        try {
                            // attempt to parse out our custom errorcode
                            var responseJson = JSON.parse(jqXHR.responseText);
                            var errorMessages = [];
                            $.each(responseJson.ErrorCodes, (i, errorCode) => {
                                errorMessages.push(ErrorGun.ErrorCodes.MessageMap[errorCode]);
                            });
                            errorMessage = errorMessages.join("\n");
                        }
                        catch (error) {
                            errorMessage = "An unexpected server error occurred.";
                        }
                        this.ErrorMessage(errorMessage);
                        $regButton.removeAttr('disabled');
                    })
                    .done((ajaxData) => {
                        // TODO: modal "created" with app details
                        // TODO: ko bindings for disabling everything
                        $('input').attr('disabled', true);
                        this.ApiKey(ajaxData.ApiKey);
                        this.Id(ajaxData.Id);
                    });
                }

                this.AddContactEmail = () => {
                    var contactCount = this.ContactEmails().length;
                    if (this.ContactEmails()[contactCount - 1].Address() === '') {
                        return false;
                    }
                    this.ContactEmails.push({Address: ko.observable(""), Focused: true});
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