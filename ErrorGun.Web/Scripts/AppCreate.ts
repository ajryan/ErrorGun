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
            public Working = ko.observable(false);
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
                    this.Working(true);
                    this.NewContactEmail("");

                    var json = ko.toJSON(this);
                    $.ajax({
                        url: "/api/apps",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: json
                    })
                    .fail((jqXHR: JQueryXHR, textStatus) => {
                        var errorMessage = ErrorGun.ErrorCodes.GetErrorMessages(jqXHR);
                        this.ErrorMessage(errorMessage);
                    })
                    .done((ajaxData) => {
                        this.ErrorMessage("");
                        this.ApiKey(ajaxData.ApiKey);
                        this.Id(ajaxData.Id);
                    })
                    .always(() => {
                        this.Working(false);
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