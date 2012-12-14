/// <reference path="knockout-2.2.d.ts" />

module ErrorGun {
    export module ViewModels {

        export class App {
            public Id = ko.observable("");
            public Name = ko.observable("");
            public ContactEmails = ko.observableArray([]);
            public ApiKey = ko.observable("");
            public CreatedTimestampUtc = ko.observable("");
            public ErrorMessage = ko.observable("");
        }

    }
}