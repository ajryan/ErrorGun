/// <reference path="knockout-2.2.d.ts" />
/// <reference path="jquery-1.8.d.ts" />
/// <reference path="ErrorCodes.ts" />
"use strict";

module ErrorGun {
    export module AppCreate {
        
        export class ViewModel {

            // properties
            public Id = ko.observable("");
            public ApiKey = ko.observable("");
            public CreatedTimestampUtc = ko.observable("");
            public Name = ko.observable("");
            public NewContactEmail = ko.observable("");
            public ContactEmails = ko.observableArray([]);
            public ErrorMessage = ko.observable("");
            public NewContactEmailValid: KnockoutComputed;

            // methods
            public Create: () => void;
            public AddContactEmail: () => void;
            public AddContactEmailOnEnter: (data, event) => void;
            public RemoveContactEmail: (email) => void;
            public toJSON: () => Object;

            constructor() {
                this.NewContactEmailValid = ko.computed(() => {
                    var emailRegex: RegExp = /^[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i;
                    return emailRegex.test(this.NewContactEmail());
                });

                this.Create = () => {
                    var $regButton = $('#registerButton');
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
                            errorMessage = ErrorGun.ErrorCodes.GetErrorMessages(responseJson.ErrorCodes);
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
                        $('input, button').attr('disabled', true);
                        this.ErrorMessage("");
                        this.ApiKey(ajaxData.ApiKey);
                        this.Id(ajaxData.Id);
                    });
                }

                this.AddContactEmail = () => {
                    if (!this.NewContactEmailValid())
                        return;

                    this.ContactEmails.push(this.NewContactEmail());
                    this.NewContactEmail("");
                    $('#emailAddress').focus();
                }

                this.AddContactEmailOnEnter = (data, event) => {
                    var keyCode = (event.which ? event.which : event.keyCode);
                    if (keyCode === 13) {
                        this.AddContactEmail();
                        return false;
                    }
                    return true;
                }

                this.RemoveContactEmail = (email) => {
                    this.ContactEmails.remove(email);
                    this.NewContactEmail(email);
                    $('#emailAddress').focus();
                }

                this.toJSON = () => {
                    return {
                        Name: this.Name(),
                        ContactEmails: this.ContactEmails()
                    };
                }
            }
        }
    }
}