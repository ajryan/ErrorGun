/// <reference path="knockout-2.2.d.ts" />
/// <reference path="jquery-1.8.d.ts" />
/// <reference path="ErrorCodes.ts" />
"use strict";

module ErrorGun {
    export module AppCreate {
        
        export class ContactEmail {
            public Address: KnockoutObservableString; 
            public Focused: KnockoutObservableBool;
            public CanRemove: KnockoutObservableBool;

            constructor(address: string = "", focused: bool = false, canRemove: bool = true) {
                this.Address = ko.observable(address);
                this.Focused = ko.observable(focused);
                this.CanRemove= ko.observable(canRemove);
            }
        }

        export class ViewModel {

            // properties
            public Id = ko.observable("");
            public ApiKey = ko.observable("");
            public CreatedTimestampUtc = ko.observable("");
            public Name = ko.observable("");
            public ContactEmails = ko.observableArray([new ContactEmail("", false, false)]);
            public ErrorMessage = ko.observable("");

            // methods
            public Create: () => void;
            public AddContactEmail: () => void;
            public RemoveContactEmail: (email) => void;
            public toJSON: () => Object;

            constructor() {
                this.Create = () => {
                    var $regButton = $('#registerButton');
                    $regButton.attr('disabled', true);

                    var removeEmails = $.grep(this.ContactEmails(), (item: ContactEmail) => {
                        return (item.CanRemove() && !/\S/.test(item.Address()));
                    });
                    this.ContactEmails.removeAll(removeEmails);

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
                        $('input').attr('disabled', true);
                        this.ErrorMessage("");
                        this.ApiKey(ajaxData.ApiKey);
                        this.Id(ajaxData.Id);
                    });
                }

                this.AddContactEmail = () => {
                    this.ContactEmails.push(new ContactEmail("", true, true));
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