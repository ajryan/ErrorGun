/// <reference path="defs\knockout-2.2.d.ts" />
/// <reference path="defs\jquery-1.8.d.ts" />
/// <reference path="ErrorCodes.ts" />
/// <reference path="App.ts" />
"use strict";

module ErrorGun {
    export module ViewModels {
        
        export class AppCreate extends App {

            // properties
            public NewContactEmail = ko.observable("");
            public NewContactEmailValid: KnockoutComputed;

            // methods
            public Create: () => void;
            public AddContactEmail: () => void;
            public AddContactEmailOnEnter: (data, event) => void;
            public RemoveContactEmail: (email) => void;
            public toJSON: () => Object;

            constructor() {
                super();

                this.NewContactEmailValid = ko.computed(() => {
                    var newEmail = this.NewContactEmail();
                    if (newEmail == null || newEmail.length == 0)
                        return true;

                    var emailRegex: RegExp = /^[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i;
                    return emailRegex.test(newEmail);
                });

                this.Create = () => {
                    var $regButton = $('#registerButton');
                    $regButton.attr('disabled', true);
                    this.NewContactEmail("");

                    var json = ko.toJSON(this);
                    $.ajax({
                        url: "/api/apps",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: json
                    })
                    .fail((jqXHR, textStatus) => {
                        var errorMessage = ErrorGun.ErrorCodes.GetErrorMessages(jqXHR.responseText);
                        this.ErrorMessage(errorMessage);
                        $regButton.removeAttr('disabled');
                    })
                    .done((ajaxData) => {
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